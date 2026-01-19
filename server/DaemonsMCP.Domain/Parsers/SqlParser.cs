using System;
using System.Collections.Generic;
using System.Linq;
using DaemonsMCP.Domain.Enums;

namespace DaemonsMCP.Domain.Parsers {

  public class ParsedTable {
    public string Name { get; set; } = "";
    public string Schema { get; set; } = "dbo";
    public List<ParsedColumn> Columns { get; set; } = new();
  }

  public class ParsedColumn {
    public string Name { get; set; } = "";
    public string ColumnType { get; set; } = "";  // Raw SQL type like "int", "nvarchar(100)"
    public int ColumnTypeId { get; set; }  // Mte enum value
    public bool IsPrimaryKey { get; set; } = false;
    public bool IsIdentity { get; set; } = false;
    public bool IsNullable { get; set; } = true;
    public string MaxLength { get; set; } = "";
  }

  public class ParsedSqlResult {
    public List<ParsedTable> Tables { get; set; } = new();
  }

  public static class SqlParser {

    public static ParsedSqlResult ParseCreateTable(string sql) {
      var tokens = SqlTokenizer.Tokenize(sql);
      var table = new ParsedTable();
      var result = new ParsedSqlResult();

      int i = 0;
      bool inCreateState = false;
      bool inCreateTableState = false;
      bool hasTableName = false;      
      bool inTableCreateNameCols = false;
      bool inProcCreateNameParam = false;
      string typeStr = "";

      ParsedColumn currentColumn = null;
      bool isConstraint = false;

      bool NeedsAdvance = true;
      int tokenCount = tokens.Count;
      while (i < tokenCount) {
        var token = tokens[i];

        if ((!inTableCreateNameCols) && (!inProcCreateNameParam)) {

          // Handle table and procedure creation name parameters
          if (i <= tokenCount && !inCreateState && token.Type == SqlTokenType.Keyword && token.Content.ToUpper() == "CREATE") {
            inCreateState = true;
            inCreateTableState = false;
            hasTableName = false;            
            i++;
            if (i < tokens.Count) {
              token = tokens[i];
            }
            NeedsAdvance = false;
          }

          // TABLE keyword
          if (i <= tokenCount && inCreateState && !inCreateTableState && token.Type == SqlTokenType.Keyword && token.Content.ToUpper() == "TABLE") {
            inCreateTableState = true;            
            i++;
            if (i < tokens.Count) {
              token = tokens[i];
            }
            NeedsAdvance = false;
          }

          // Table name (might have schema prefix like dbo.Users)
          if (i <= tokenCount && inCreateTableState && inCreateState && !hasTableName && token.Type == SqlTokenType.Identifier) {
            var parts = token.Content.Split('.');
            if (parts.Length == 2) {
              table.Schema = parts[0];
              table.Name = parts[1];
            } else {
              table.Name = token.Content;
            }
            hasTableName = true;
            i++; 
            if (i < tokens.Count) {
              token = tokens[i];
            }
            NeedsAdvance = false;
          }

          if (i <= tokenCount && hasTableName && !inTableCreateNameCols && token.Type == SqlTokenType.ParanStart) {
            inTableCreateNameCols = true;
            i++; 
            if (i < tokens.Count) {
              token = tokens[i];
            }
            NeedsAdvance = false;
            inCreateState = false;
            inCreateTableState = false;
          }
        }     

        // Inside column definitions
        if (inTableCreateNameCols) {

          // Check for CONSTRAINT keyword - skip constraint definitions
          if ((i <= tokenCount)
            && token.Type == SqlTokenType.Keyword && token.Content.ToUpper() == "CONSTRAINT") {
            isConstraint = true;
            i++; 
            if (i < tokens.Count) {
              token = tokens[i];
            }
            NeedsAdvance = false;
          }

          // Column name
          if ((i <= tokenCount) && token.Type == SqlTokenType.Identifier && currentColumn == null && !isConstraint) {
            currentColumn = new ParsedColumn { Name = token.Content };
            i++; 
            if (i < tokens.Count) {
              token = tokens[i];
            }
            NeedsAdvance = false;
          }

          // Column type
          if ((i <= tokenCount) && currentColumn != null && token.Type == SqlTokenType.ColType) {
            typeStr = token.Content.ToLower();
            currentColumn.ColumnType = typeStr;
            currentColumn.ColumnTypeId = MapSqlTypeToMte(typeStr);
            i++; 
            if (i < tokens.Count) {
              token = tokens[i];
            }
            NeedsAdvance = false;
          }

          // Check for IDENTITY keyword before size/parens
          if ((i <= tokenCount) && token.Type == SqlTokenType.Keyword && currentColumn != null) {
            string upper = token.Content.ToUpper();
            if (upper == "IDENTITY") {
              currentColumn.IsIdentity = true;
              currentColumn.IsPrimaryKey = true;  // Usually IDENTITY implies PK
              currentColumn.IsNullable = false;
            }
            i++; 
            if (i < tokens.Count) {
              token = tokens[i];
            }
            NeedsAdvance = false;
          }

          // Check for size in next token (e.g., (100) or (18,2))
          if (i < tokenCount && currentColumn != null && tokens[i].Type == SqlTokenType.ParanStart) {
            string sizeStr = "(";
            i++; // skip (
            int depth = 1;
            while (i < tokens.Count && depth > 0) {
              if (tokens[i].Type == SqlTokenType.ParanStart) depth++;
              if (tokens[i].Type == SqlTokenType.ParanEnd) depth--;
              sizeStr += tokens[i].Content;
              i++;
              if (i < tokens.Count) {
                token = tokens[i];
              }
              NeedsAdvance = false;
            }

            // Extract numeric size for varchar/nvarchar/char types
            if ((!currentColumn.IsPrimaryKey) && (typeStr.Contains("varchar") || typeStr.Contains("char"))) {
              currentColumn.MaxLength = sizeStr;              
            }
          }          

          // Keywords: IDENTITY, NOT NULL, NULL, PRIMARY KEY
          if (i < tokenCount && currentColumn != null && token.Type == SqlTokenType.Keyword && currentColumn != null) {
            string upper = token.Content.ToUpper();
            
            if (upper == "PRIMARY") {
              currentColumn.IsPrimaryKey = true;
              currentColumn.IsNullable = false;
            }
            if (upper == "NOT") {
              // Look ahead for NULL
              if (i + 1 < tokens.Count && tokens[i + 1].Content.ToUpper() == "NULL") {
                currentColumn.IsNullable = false;
                i++; // skip NULL token
              }
            }
            if (upper == "NULL" && i > 0 && tokens[i - 1].Content.ToUpper() != "NOT") {
              currentColumn.IsNullable = true;
            }

            i++;
            if (i < tokens.Count) {
              token = tokens[i];
            }
            NeedsAdvance = false;            
          }

          // Comma - end of current column
          if (i < tokenCount && currentColumn != null && token.Type == SqlTokenType.Comma) {
            if (currentColumn != null && !isConstraint) {
              table.Columns.Add(currentColumn);
              currentColumn = null;
            }
            isConstraint = false;
            i++;
            if (i < tokens.Count) {
              token = tokens[i];
            }
            NeedsAdvance = false;
          }

          // Closing paren - end of table definition
          if (i < tokenCount && token.Type == SqlTokenType.ParanEnd) {
            if (currentColumn != null && !isConstraint) {
              table.Columns.Add(currentColumn);
            }
            result.Tables.Add(table);
            currentColumn = null;
            table = new ParsedTable();
            i++;
            if (i < tokens.Count) {
              token = tokens[i];
            }
            NeedsAdvance = false;
            inTableCreateNameCols = false;                       
          }
        }

        if (NeedsAdvance) {
          i++;
        } else {
          NeedsAdvance = true;
        }
      }

      return result;
    }

    private static int MapSqlTypeToMte(string sqlType) {
      return sqlType.ToLower() switch {
        "bit" => (int)Mte.SqlBitType,
        "smallint" => (int)Mte.SqlSmallIntType,
        "int" => (int)Mte.SqlIntType,
        "bigint" => (int)Mte.SqlBigIntType,
        "uniqueidentifier" => (int)Mte.SqlUniqueIdentifierType,
        "varchar" => (int)Mte.SqlVarcharType,
        "nvarchar" => (int)Mte.SqlNVarcharType,
        "decimal" => (int)Mte.SqlDecimalType,
        "numeric" => (int)Mte.SqlDecimalType,
        "datetime" => (int)Mte.SqlDateTimeType,
        "datetime2" => (int)Mte.SqlDateTimeType,
        "date" => (int)Mte.SqlDateType,
        "time" => (int)Mte.SqlTimeType,
        _ => (int)Mte.SqlNVarcharType  // Default fallback
      };
    }
  }
}
