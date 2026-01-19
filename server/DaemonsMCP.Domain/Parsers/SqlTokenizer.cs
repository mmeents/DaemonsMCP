using DaemonsMCP.Domain.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DaemonsMCP.Domain.Parsers {

  public enum SqlTokenType {
    Keyword, Identifier, ColType, ParanStart, ParanEnd, Comma, Number, Comment, NewLine
  }

  public class SqlToken {
    public SqlTokenType Type { get; set; }
    public string Content { get; set; }
  }

  public static class SqlTokenizer {

    public static List<SqlToken> Tokenize(string sql) {
      var tokens = new List<SqlToken>();
      var lines = sql.Parse(Environment.NewLine);

      foreach (var line in lines) {
        string noComment = line.Contains("--") ? line.Substring(0, line.IndexOf("--")) : line;
        var raw = noComment.Replace(",", " , ").Replace("(", " (").Replace(")", " )").Parse(" ");
        foreach (var op in raw) {
          string wop = op;
          while (wop.Length > 0) {

            if (wop[0] == '(') {
              tokens.Add(new SqlToken { Type = SqlTokenType.ParanStart, Content = "(" });
              wop = wop.Substring(1);
            } else if (wop[0] == ')') {
              tokens.Add(new SqlToken { Type = SqlTokenType.ParanEnd, Content = ")" });
              wop = wop.Substring(1);
            } else if (wop[0] == ',') {
              tokens.Add(new SqlToken { Type = SqlTokenType.Comma, Content = "," });
              wop = wop.Substring(1);
            } else if (IsNumber(wop)) {
              string num = ReadNumbers(wop);
              if (num.Length > 0) {
                tokens.Add(new SqlToken { Type = SqlTokenType.Number, Content = num });
                wop = wop.Substring(num.Length);
              }
            } else if (IsKeyword(wop)) {
              tokens.Add(new SqlToken { Type = SqlTokenType.Keyword, Content = wop });
              wop = "";
            } else if (IsColType(wop)) {
              string colType = ReadColType(wop);
              var wopl = colType.Length;
              colType = colType.Trim().Replace("[", "").Replace("]", "");
              if (colType.Length > 0) {
                tokens.Add(new SqlToken { Type = SqlTokenType.ColType, Content = colType });
                wop = wop.Substring(wopl);
              }
            } else if (IsIdentifier(wop)) {
              string identifier = ReadIdentifier(wop);
              if (identifier.Length > 0) {
                string clean = identifier.Trim().Replace("[", "").Replace("]", "");
                tokens.Add(new SqlToken { Type = SqlTokenType.Identifier, Content = clean });
                wop = wop.Substring(identifier.Length);
              }
            } else {
              wop = ""; // Skip unknown
            }
          }
        }
       // tokens.Add(new SqlToken { Type = SqlTokenType.NewLine, Content = Environment.NewLine });
      }
      return tokens;
    }

    private static bool IsNumber(string content) {
      return content.Length > 0 && "0123456789.-".Contains(content[0]);
    }

    private static string ReadNumbers(string content) {
      if (content == null) return "";
      if (content.Length > 0 && "0123456789.-".Contains(content[0])) {
        int i = 0;
        string result = "";
        while (i < content.Length && "0123456789.-".Contains(content[i])) {
          result += content[i];
          i++;
        }
        return result;
      }
      return "";
    }

    private static bool IsKeyword(string content) {
      if (content == null) return false;
      var upper = content.ToUpper();
      return new HashSet<string> {
        "ADD","ALL","ALTER","AND","ANY","AS","ASC","AUTHORIZATION","BACKUP",
        "BEGIN","BETWEEN","BREAK","BROWSE","BULK","BY","CASCADE","CASE","CHECK",
        "CHECKPOINT","CLOSE","CLUSTERED","COALESCE","COLLATE","COLUMN","COMMIT","COMPUTE","CONSTRAINT",
        "CONTAINS","CONTAINSTABLE","CONTINUE","CONVERT","CREATE","CROSS","CURRENT","CURRENT_DATE","CURRENT_TIME",
        "CURRENT_TIMESTAMP","CURRENT_USER","CURSOR","DATABASE","DBCC","DEALLOCATE","DECLARE","DEFAULT","DELETE",
        "DENY","DESC","DISK","DISTINCT","DISTRIBUTED","DOUBLE","DROP","DUMP","ELSE",
        "END","ERRLVL","ESCAPE","EXCEPT","EXEC","EXECUTE","EXISTS","EXIT","EXTERNAL",
        "FETCH","FILE","FILLFACTOR","FOR","FOREIGN","FREETEXT","FREETEXTTABLE","FROM","FULL",
        "FUNCTION","GOTO","GRANT","GROUP","HAVING","HOLDLOCK","IDENTITY","IDENTITY_INSERT","IDENTITYCOL",
        "IF","IN","INDEX","INNER","INSERT","INTERSECT","INTO","IS","JOIN",
        "KEY","KILL","LEFT","LIKE","LINENO","LOAD","MERGE","NATIONAL","NOCHECK",
        "NONCLUSTERED","NOT","NULL","NULLIF","OF","OFF","OFFSETS","ON","OPEN",
        "OPENDATASOURCE","OPENQUERY","OPENROWSET","OPENXML","OPTION","OR","ORDER","OUTER","OVER",
        "PROCEDURE","PUBLIC","RAISERROR","READ","READTEXT","RECONFIGURE","REFERENCES","REPLICATION","RESTORE",
        "RESTRICT","RETURN","REVERT","REVOKE","RIGHT","ROLLBACK","ROWCOUNT","ROWGUIDCOL","RULE",
        "SAVE","SCHEMA","SECURITYAUDIT","SELECT","SEMANTICKEYPHRASETABLE","SEMANTICSIMILARITYDETAILSTABLE","SEMANTICSIMILARITYTABLE","SESSION_USER","SET",
        "SETUSER","SHUTDOWN","SOME","STATISTICS","SYSTEM_USER","TABLE","TABLESAMPLE","TEXTSIZE","THEN",
        "TO","TOP","TRAN","TRANSACTION","TRIGGER","TRUNCATE","TRY_CONVERT","TSEQUAL","UNION",
        "UNIQUE","UNPIVOT","UPDATE","UPDATETEXT","USE","USER","VALUES","VARYING","VIEW",
        "WAITFOR","PERCENT","PIVOT","PLAN","PRECISION","PRIMARY","PRINT","PROC","WHEN",
        "WHERE","WHILE","WITH","WITHIN","WRITETEXT"
      }.Contains(upper);
    }

    private static bool IsColType(string content) {
      if (content == null) return false;
      var lower = content.ToLower().Replace('[', ' ').Replace(']', ' ').Replace('(', ' ').ParseFirst(" ,");
      return new HashSet<string> {
        "char","varchar","int","bigint","binary","bit","datetime","decimal","float","image",
        "money","numeric","nchar","ntext","nvarchar","real","smallint","smallmoney","smalldatetime",
        "text","timestamp","tinyint","uniqueidentifier","varbinary"
      }.Contains(lower);
    }

    private static string ReadColType(string content) {
      if (content == null) return "";
      string result = content.Replace("(", " ").Split(' ', ',')[0];
      return IsColType(result) ? result : "";
    }

    private static bool IsIdentifier(string content) {
      if (content.Length == 0) return false;
      if (content[0] == '(' || content[0] == ')' || content[0] == ',') return false;
      if (IsNumber(content)) return false;
      if (IsKeyword(content)) return false;
      if (IsColType(content)) return false;
      return true;
    }

    private static string ReadIdentifier(string content) {
      if (content == null) return "";
      return content.Replace('(', ' ').Replace(')', ' ').ParseFirst(" ,");
    }
  }
}
