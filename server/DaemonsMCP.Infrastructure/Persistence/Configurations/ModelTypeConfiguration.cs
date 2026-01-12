using DaemonsMCP.Domain.Entities;
using DaemonsMCP.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Infrastructure.Persistence.Configurations {
  public class ModelTypeConfiguration : IEntityTypeConfiguration<ModelType> {
    public void Configure(EntityTypeBuilder<ModelType> builder) {
      builder.ToTable("ModelTypes");

      builder.HasKey(mt => mt.Id);

      builder.Property(mt => mt.OwnerTypeId)
          .IsRequired(false);

      builder.Property(mt => mt.CategoryTypeId)
          .IsRequired(false);

      builder.Property(mt => mt.EditorTypeId)
          .IsRequired(false);

      builder.Property(mt => mt.TypeRank)
          .IsRequired()
          .HasDefaultValue(0);

      builder.Property(mt => mt.Name)
          .IsRequired()
          .HasMaxLength(100);

      builder.Property(mt => mt.Description)
          .IsRequired()
          .HasMaxLength(500)
          .HasDefaultValue(string.Empty);

      builder.Property(mt => mt.IsVisible)
          .IsRequired()
          .HasDefaultValue(false);

      builder.Property(mt => mt.IsReadonly)
          .IsRequired()
          .HasDefaultValue(false);

      builder.Property(mt => mt.IconName)
          .IsRequired()
          .HasMaxLength(50)
          .HasDefaultValue(string.Empty);

      // Self-referencing relationship for hierarchy      
      builder.HasOne(mt => mt.Owner)
          .WithMany(mt => mt.Children)
          .HasForeignKey(mt => mt.OwnerTypeId)
          .OnDelete(DeleteBehavior.Restrict)
          .IsRequired(false);

      builder.HasOne(builder => builder.Category)
          .WithMany()
          .HasForeignKey(mt => mt.CategoryTypeId)
          .OnDelete(DeleteBehavior.Restrict)
          .IsRequired(false);

      builder.HasOne(mt => mt.Editor)
          .WithMany()
          .HasForeignKey(mt => mt.EditorTypeId)
          .OnDelete(DeleteBehavior.Restrict)
          .IsRequired(false);

      // Indexes
      builder.HasIndex(mt => mt.OwnerTypeId);
      builder.HasIndex(mt => mt.CategoryTypeId);
      builder.HasIndex(mt => mt.EditorTypeId);
      builder.HasIndex(mt => mt.Name);
      builder.HasIndex(mt => new { mt.OwnerTypeId, mt.TypeRank });

      // Seed data - system model types
      builder.HasData(
          new ModelType {
              Id = (int)Mte.TypeRoot,
              OwnerTypeId = null,
              CategoryTypeId = null,
              EditorTypeId = null,
              TypeRank = 0,
              Name = "Root Internal Owner Type Model",
              Description = "Base root type model",
              IsVisible = false,
              IsReadonly = true,
              IconName = "pi-cog"
          }, // 1 root 
          new ModelType {
              Id = (int)Mte.CategoryRoot,
              OwnerTypeId = (int)Mte.TypeRoot,
              CategoryTypeId = (int)Mte.CategoryRoot,
              EditorTypeId = (int)Mte.CategoryRoot,
              TypeRank = 1,
              Name = "Categories",
              Description = "Adds node for categories dimension",
              IsVisible = false,
              IsReadonly = true,
              IconName = "pi-folder"
          }, // 2 Categories Types
          new ModelType {
              Id = (int)Mte.EditorList,
              OwnerTypeId = (int)Mte.TypeRoot,
              CategoryTypeId = (int)Mte.CategoryRoot,
              EditorTypeId = (int)Mte.CategoryRoot,
              TypeRank = 2,
              Name = "Editor Types",
              Description = "Adds node for editor types dimension",
              IsVisible = false,
              IsReadonly = true,
              IconName = "pi-pencil"
          }, // 3 Editor Types

      #region Editor types actual. 
          new ModelType {  
              Id = (int)Mte.HiddenEditor,
              OwnerTypeId = (int)Mte.EditorList,
              CategoryTypeId = (int)Mte.EditorList,
              EditorTypeId = (int)Mte.HiddenEditor,
              TypeRank = 1,
              Name = "Hidden",
              Description = "editor is hidden",
              IsVisible = false,
              IsReadonly = true,
              IconName = "pi-eye-slash"
          }, // 10 Hidden editor type
          new ModelType {
              Id = (int)Mte.BooleanEditor,
              OwnerTypeId = (int)Mte.EditorList,
              CategoryTypeId = (int)Mte.EditorList,
              EditorTypeId = (int)Mte.BooleanEditor,
              TypeRank = 2,
              Name = "Boolean",
              Description = "boolean editor",
              IsVisible = true,
              IsReadonly = false,
              IconName = "pi-check"
          }, // Boolean editor type
          new ModelType {
              Id = (int)Mte.IntegerEditor,
              OwnerTypeId = (int)Mte.EditorList,
              CategoryTypeId = (int)Mte.EditorList,
              EditorTypeId = (int)Mte.IntegerEditor,
              TypeRank = 3,
              Name = "Integer",
              Description = "integer editor",
              IsVisible = true,
              IsReadonly = false,
              IconName = "pi-pencil"
          }, // 12 Integer editor type
          new ModelType {
              Id = (int)Mte.StringEditor,
              OwnerTypeId = (int)Mte.EditorList,
              CategoryTypeId = (int)Mte.EditorList,
              EditorTypeId = (int)Mte.StringEditor,
              TypeRank = 4,
              Name = "String",
              Description = "string editor",
              IsVisible = true,
              IsReadonly = false,
              IconName = "pi-pencil"
          }, // 13 String editor type
          new ModelType {
              Id = (int)Mte.FilenameEditor,
              OwnerTypeId = (int)Mte.EditorList,
              CategoryTypeId = (int)Mte.EditorList,
              EditorTypeId = (int)Mte.FilenameEditor,
              TypeRank = 5,
              Name = "Filename",
              Description = "filename editor",
              IsVisible = true,
              IsReadonly = false,
              IconName = "pi-file"
          }, // Filename editor type
          new ModelType {
              Id = (int)Mte.DateEditor,
              OwnerTypeId = (int)Mte.EditorList,
              CategoryTypeId = (int)Mte.EditorList,
              EditorTypeId = (int)Mte.DateEditor,
              TypeRank = 6,
              Name = "Date",
              Description = "date editor",
              IsVisible = true,
              IsReadonly = false,
              IconName = "pi-calendar"
          }, // Date editor type
          new ModelType {
              Id = (int)Mte.TimeEditor,
              OwnerTypeId = (int)Mte.EditorList,
              CategoryTypeId = (int)Mte.EditorList,
              EditorTypeId = (int)Mte.TimeEditor,
              TypeRank = 7,
              Name = "Time",
              Description = "time editor",
              IsVisible = true,
              IsReadonly = false,
              IconName = "pi-clock"
          }, // Time editor type
          new ModelType {
              Id = (int)Mte.DecimalEditor,
              OwnerTypeId = (int)Mte.EditorList,
              CategoryTypeId = (int)Mte.EditorList,
              EditorTypeId = (int)Mte.DecimalEditor,
              TypeRank = 8,
              Name = "Decimal",
              Description = "decimal editor",
              IsVisible = true,
              IsReadonly = false,
              IconName = "pi-dollar"
          }, // 17 Decimal editor type
          new ModelType {
              Id = (int)Mte.PasswordEditor,
              OwnerTypeId = (int)Mte.EditorList,
              CategoryTypeId = (int)Mte.EditorList,
              EditorTypeId = (int)Mte.PasswordEditor,
              TypeRank = 9,
              Name = "Password",
              Description = "password editor",
              IsVisible = true,
              IsReadonly = false,
              IconName = "pi-lock"
          }, // Password editor type
          new ModelType {
              Id = (int)Mte.LookupTypeEditor,
              OwnerTypeId = (int)Mte.EditorList,
              CategoryTypeId = (int)Mte.EditorList,
              EditorTypeId = (int)Mte.LookupTypeEditor,
              TypeRank = 10,
              Name = "LookupTypeEditor",
              Description = "lookup editor",
              IsVisible = true,
              IsReadonly = false,
              IconName = "pi-search"
          }, // 19 Lookup editor for types
          new ModelType {
            Id = (int)Mte.LookupModelEditor,
            OwnerTypeId = (int)Mte.EditorList,
            CategoryTypeId = (int)Mte.EditorList,
            EditorTypeId = (int)Mte.LookupModelEditor,
            TypeRank = 11,
            Name = "LookupModelEditor",
            Description = "lookup on model editor",
            IsVisible = true,
            IsReadonly = false,
            IconName = "pi-search"
          }, // 20 Lookup editor for models
      #endregion
      #region HTTP Method Types
          new ModelType {
            Id = (int)Mte.HttpMethod,
            OwnerTypeId = (int)Mte.EditorList,
            CategoryTypeId = (int)Mte.EditorList,
            EditorTypeId = null,
            TypeRank = 11,
            Name = "HTTP Method Types",
            Description = "HTTP method types editor",
            IsVisible = true,
            IsReadonly = true,
            IconName = "pi-pencil"
          }, // 50 HTTP Method Types editor type
          new ModelType {
            Id = (int)Mte.HttpGetMethod,
            OwnerTypeId = (int)Mte.HttpMethod,
            CategoryTypeId = (int)Mte.HttpMethod,
            EditorTypeId = null,
            TypeRank = 1,
            Name = "GET",
            Description = "HTTP GET method",
            IsVisible = true,
            IsReadonly = true,
            IconName = "pi-eye"
          }, //   52 HTTP GET method
          new ModelType {
            Id = (int)Mte.HttpPostMethod,
            OwnerTypeId = (int)Mte.HttpMethod,
            CategoryTypeId = (int)Mte.HttpMethod,
            EditorTypeId = null,
            TypeRank = 2,
            Name = "POST",
            Description = "HTTP POST method",
            IsVisible = true,
            IsReadonly = true,
            IconName = "pi-pencil"
          }, //   53 HTTP POST method
          new ModelType {
            Id = (int)Mte.HttpPutMethod,
            OwnerTypeId = (int)Mte.HttpMethod,
            CategoryTypeId = (int)Mte.HttpMethod,
            EditorTypeId = null,
            TypeRank = 3,
            Name = "PUT",
            Description = "HTTP PUT method",
            IsVisible = true,
            IsReadonly = true,
            IconName = "pi-pencil"
          }, //   54 HTTP PUT method
          new ModelType {
            Id = (int)Mte.HttpDeleteMethod,
            OwnerTypeId = (int)Mte.HttpMethod,
            CategoryTypeId = (int)Mte.HttpMethod,
            EditorTypeId = null,
            TypeRank = 4,
            Name = "DELETE",
            Description = "HTTP DELETE method",
            IsVisible = true,
            IsReadonly = true,
            IconName = "pi-delete-left"
          }, //   55 HTTP DELETE method
          new ModelType {
            Id = (int)Mte.HttpPatchMethod,
            OwnerTypeId = (int)Mte.HttpMethod,
            CategoryTypeId = (int)Mte.HttpMethod,
            EditorTypeId = null,
            TypeRank = 5,
            Name = "PATCH",
            Description = "HTTP PATCH method",
            IsVisible = true,
            IsReadonly = true,
            IconName = "pi-pencil"
          }, //   56 HTTP PATCH method
          #endregion
      #region Accessibility types 
          new ModelType {
            Id = (int)Mte.CSharpAccessModifiers,
            OwnerTypeId = (int)Mte.EditorList,
            CategoryTypeId = (int)Mte.EditorList,
            EditorTypeId = (int)Mte.EditorList,
            TypeRank = 1,
            Name = "C# Accessibility Types",
            Description = "Accessibility types list",
            IsVisible = false,
            IsReadonly = true,
            IconName = ""
          }, // 60 C# Accessibility Types list
          new ModelType {
            Id = (int)Mte.CSharpPublicModifier,
            OwnerTypeId = (int)Mte.CSharpAccessModifiers,
            CategoryTypeId = (int)Mte.EditorList,
            EditorTypeId = null,
            TypeRank = 1,
            Name = "public",
            Description = "public accessibility",
            IsVisible = true,
            IsReadonly = true,
            IconName = ""
          }, //   62 public accessibility
          new ModelType {
            Id = (int)Mte.CSharpPrivateModifier,
            OwnerTypeId = (int)Mte.CSharpAccessModifiers,
            CategoryTypeId = (int)Mte.EditorList,
            EditorTypeId = null,
            TypeRank = 2,
            Name = "private",
            Description = "private accessibility",
            IsVisible = true,
            IsReadonly = true,
            IconName = ""
          }, //   64 private accessibility
          new ModelType {
            Id = (int)Mte.CSharpProtectedModifier,
            OwnerTypeId = (int)Mte.CSharpAccessModifiers,
            CategoryTypeId = (int)Mte.EditorList,
            EditorTypeId = null,
            TypeRank = 3,
            Name = "protected",
            Description = "protected accessibility",
            IsVisible = true,
            IsReadonly = true,
            IconName = ""
          }, //   66 protected accessibility
          new ModelType {
            Id = (int)Mte.CSharpInternalModifier,
            OwnerTypeId = (int)Mte.CSharpAccessModifiers,
            CategoryTypeId = (int)Mte.EditorList,
            EditorTypeId = null,
            TypeRank = 4,
            Name = "internal",
            Description = "internal accessibility",
            IsVisible = true,
            IsReadonly = true,
            IconName = ""
          }, //   68 internal accessibility
      #endregion
      #region SQL Data Types
          new ModelType {
            Id = (int)Mte.SqlTypes,
            OwnerTypeId = (int)Mte.CategoryRoot,
            CategoryTypeId = (int)Mte.TableModel,
            EditorTypeId = (int)Mte.LookupTypeEditor,
            TypeRank = 1,
            Name = "Sql Types",
            Description = "Sql data types list",
            IsVisible = false,
            IsReadonly = true,
            IconName = ""
          }, // 100 Sql Types list
          new ModelType {
              Id = (int)Mte.SqlBitType,
              OwnerTypeId = (int)Mte.SqlTypes,
              CategoryTypeId = (int)Mte.EditorList,
              EditorTypeId = (int)Mte.BooleanEditor,
              TypeRank = 1,
              Name = "bit",
              Description = "SQL bit type",
              IsVisible = true,
              IsReadonly = false,
              IconName = ""
          }, //   101 Sql bit type
          new ModelType {
              Id = (int)Mte.SqlSmallIntType,
              OwnerTypeId = (int)Mte.SqlTypes,
              CategoryTypeId = (int)Mte.EditorList,
              EditorTypeId = (int)Mte.IntegerEditor,
              TypeRank = 2,
              Name = "smallint",
              Description = "SQL Small Integer type",
              IsVisible = true,
              IsReadonly = false,
              IconName = ""
          }, //   102 Sql Small Integer type
          new ModelType {
              Id = (int)Mte.SqlIntType,
              OwnerTypeId = (int)Mte.SqlTypes,
              CategoryTypeId = (int)Mte.EditorList,
              EditorTypeId = (int)Mte.IntegerEditor,
              TypeRank = 3,
              Name = "int",
              Description = "SQL Integer type",
              IsVisible = true,
              IsReadonly = false,
              IconName = ""
          }, //   103 Sql Integer type
          new ModelType {
              Id = (int)Mte.SqlBigIntType,
              OwnerTypeId = (int)Mte.SqlTypes,
              CategoryTypeId = (int)Mte.EditorList,
              EditorTypeId = (int)Mte.IntegerEditor,
              TypeRank = 4,
              Name = "bigint",
              Description = "SQL bigint type",
              IsVisible = true,
              IsReadonly = false,
              IconName = ""
          }, //   104 Sql bigint type                  
          new ModelType {
            Id = (int)Mte.SqlUniqueIdentifierType,
            OwnerTypeId = (int)Mte.SqlTypes,
            CategoryTypeId = (int)Mte.EditorList,
            EditorTypeId = (int)Mte.StringEditor,
            TypeRank = 5,
            Name = "uniqueidentifier",
            Description = "SQL uniqueidentifier type",
            IsVisible = true,
            IsReadonly = false,
            IconName = ""
          }, //   106 Sql uniqueidentifier type
          new ModelType {
            Id = (int)Mte.SqlVarcharType,
            OwnerTypeId = (int)Mte.SqlTypes,
            CategoryTypeId = (int)Mte.EditorList,
            EditorTypeId = (int)Mte.StringEditor,
            TypeRank = 6,
            Name = "varchar",
            Description = "SQL Variable Character type",
            IsVisible = true,
            IsReadonly = false,
            IconName = ""
          }, //   108 Sql Varchar type
          new ModelType {
            Id = (int)Mte.SqlNVarcharType,
            OwnerTypeId = (int)Mte.SqlTypes,
            CategoryTypeId = (int)Mte.EditorList,
            EditorTypeId = (int)Mte.StringEditor,
            TypeRank = 7,
            Name = "nvarchar",
            Description = "SQL National Variable Character type",
            IsVisible = true,
            IsReadonly = false,
            IconName = ""
          }, //   110 Sql NVarchar type
          new ModelType {
            Id = (int)Mte.SqlDecimalType,
            OwnerTypeId = (int)Mte.SqlTypes,
            CategoryTypeId = (int)Mte.EditorList,
            EditorTypeId = (int)Mte.DecimalEditor,
            TypeRank = 8,
            Name = "decimal",
            Description = "SQL Decimal type",
            IsVisible = true,
            IsReadonly = false,
            IconName = ""
          }, //   112 Sql Decimal type
          new ModelType {
            Id = (int)Mte.SqlDateTimeType,
            OwnerTypeId = (int)Mte.SqlTypes,
            CategoryTypeId = (int)Mte.EditorList,
            EditorTypeId = (int)Mte.DateEditor,
            TypeRank = 9,
            Name = "datetime",
            Description = "SQL DateTime type",
            IsVisible = true,
            IsReadonly = false,
            IconName = ""
          }, //   114 Sql DateTime type           
          new ModelType {
            Id = (int)Mte.SqlDateType,
            OwnerTypeId = (int)Mte.SqlTypes,
            CategoryTypeId = (int)Mte.EditorList,
            EditorTypeId = (int)Mte.DateEditor,
            TypeRank = 10,
            Name = "date",
            Description = "SQL date type",
            IsVisible = true,
            IsReadonly = false,
            IconName = ""
          }, //   118 Sql date type
          new ModelType {
            Id = (int)Mte.SqlTimeType,
            OwnerTypeId = (int)Mte.SqlTypes,
            CategoryTypeId = (int)Mte.EditorList,
            EditorTypeId = (int)Mte.TimeEditor,
            TypeRank = 11,
            Name = "time",
            Description = "SQL time type",
            IsVisible = true,
            IsReadonly = false,
            IconName = ""
          }, //   120 Sql time type
      #endregion
      #region C# Types
          new ModelType {
              Id = (int)Mte.CSharpTypes,
              OwnerTypeId = (int)Mte.CategoryRoot,
              CategoryTypeId = (int)Mte.ClassModel,
              EditorTypeId = (int)Mte.LookupTypeEditor,
              TypeRank = 1,
              Name = "C# Types",
              Description = "C# Type Lookups",
              IsVisible = false,
              IsReadonly = true,
              IconName = ""
          }, // 150 Models category type
          new ModelType {
              Id = (int)Mte.CSharpClassType,
              OwnerTypeId = (int)Mte.CSharpTypes,
              CategoryTypeId = (int)Mte.EditorList,
              EditorTypeId = (int)Mte.StringEditor,
              TypeRank = 1,
              Name = "class",
              Description = "C# class model",
              IsVisible = false,
              IsReadonly = true,
              IconName = ""
          }, //   152 Class model
          new ModelType {
              Id = (int)Mte.CSharpRecordType,
              OwnerTypeId = (int)Mte.CSharpTypes,
              CategoryTypeId = (int)Mte.EditorList,
              EditorTypeId = (int)Mte.StringEditor,
              TypeRank = 2,
              Name = "record",
              Description = "C# record model",
              IsVisible = false,
              IsReadonly = true,
              IconName = ""
          }, //   154 record model
          new ModelType {
              Id = (int)Mte.CSharpStructType,
              OwnerTypeId = (int)Mte.CSharpTypes,
              CategoryTypeId = (int)Mte.EditorList,
              EditorTypeId = (int)Mte.StringEditor,
              TypeRank = 3,
              Name = "struct",
              Description = "C# struct model",
              IsVisible = false,
              IsReadonly = true,
              IconName = ""
          }, //   156 Struct model
          new ModelType {
              Id = (int)Mte.CSharpStringType,
              OwnerTypeId = (int)Mte.CSharpTypes,
              CategoryTypeId = (int)Mte.EditorList,
              EditorTypeId = (int)Mte.StringEditor,
              TypeRank = 4,
              Name = "string",
              Description = "C# string model",
              IsVisible = false,
              IsReadonly = true,
              IconName = ""
          }, //   158 string model
          new ModelType {
            Id = (int)Mte.CSharpBoolType,
            OwnerTypeId = (int)Mte.CSharpTypes,
            CategoryTypeId = (int)Mte.EditorList,
            EditorTypeId = (int)Mte.StringEditor,
            TypeRank = 5,
            Name = "bool",
            Description = "C# bool model",
            IsVisible = false,
            IsReadonly = true,
            IconName = ""
          }, //   160 bool model
          new ModelType {
            Id = (int)Mte.CSharpCharType,
            OwnerTypeId = (int)Mte.CSharpTypes,
            CategoryTypeId = (int)Mte.EditorList,
            EditorTypeId = (int)Mte.StringEditor,
            TypeRank = 6,
            Name = "char",
            Description = "C# char model",
            IsVisible = false,
            IsReadonly = true,
            IconName = ""
          }, //   162 char model
          new ModelType {
            Id = (int)Mte.CSharpIntType,
            OwnerTypeId = (int)Mte.CSharpTypes,
            CategoryTypeId = (int)Mte.EditorList,
            EditorTypeId = (int)Mte.IntegerEditor,
            TypeRank = 7,
            Name = "int",
            Description = "C# int model",
            IsVisible = false,
            IsReadonly = true,
            IconName = ""
          }, //   164 int model
          new ModelType {
            Id = (int)Mte.CSharpLongType,
            OwnerTypeId = (int)Mte.CSharpTypes,
            CategoryTypeId = (int)Mte.EditorList,
            EditorTypeId = (int)Mte.IntegerEditor,
            TypeRank = 8,
            Name = "long",
            Description = "C# long model",
            IsVisible = false,
            IsReadonly = true,
            IconName = ""
          }, //   166 long model
          new ModelType {
            Id = (int)Mte.CSharpShortType,
            OwnerTypeId = (int)Mte.CSharpTypes,
            CategoryTypeId = (int)Mte.EditorList,
            EditorTypeId = (int)Mte.IntegerEditor,
            TypeRank = 9,
            Name = "short",
            Description = "C# short model",
            IsVisible = false,
            IsReadonly = true,
            IconName = ""
          }, //   168 short model
          new ModelType {
            Id = (int)Mte.CSharpDecimalType,
            OwnerTypeId = (int)Mte.CSharpTypes,
            CategoryTypeId = (int)Mte.EditorList,
            EditorTypeId = (int)Mte.DecimalEditor,
            TypeRank = 10,
            Name = "decimal",
            Description = "C# decimal model",
            IsVisible = false,
            IsReadonly = true,
            IconName = ""
          }, //   170 decimal model
          new ModelType {
            Id = (int)Mte.CSharpDoubleType,
            OwnerTypeId = (int)Mte.CSharpTypes,
            CategoryTypeId = (int)Mte.EditorList,
            EditorTypeId = (int)Mte.DecimalEditor,
            TypeRank = 11,
            Name = "double",
            Description = "C# double model",
            IsVisible = false,
            IsReadonly = true,
            IconName = ""
          }, //   172 double model
          new ModelType {
            Id = (int)Mte.CSharpFloatType,
            OwnerTypeId = (int)Mte.CSharpTypes,
            CategoryTypeId = (int)Mte.EditorList,
            EditorTypeId = (int)Mte.DecimalEditor,
            TypeRank = 12,
            Name = "float",
            Description = "C# float model",
            IsVisible = false,
            IsReadonly = true,
            IconName = ""
          }, //   174 float model
          new ModelType {
            Id = (int)Mte.CSharpByteType,
            OwnerTypeId = (int)Mte.CSharpTypes,
            CategoryTypeId = (int)Mte.EditorList,
            EditorTypeId = (int)Mte.HiddenEditor,
            TypeRank = 13,
            Name = "byte",
            Description = "C# byte model",
            IsVisible = false,
            IsReadonly = true,
            IconName = ""
          }, //   176 byte model
          new ModelType { 
            Id = (int)Mte.CSharpDateTimeType,
            OwnerTypeId = (int)Mte.CSharpTypes,
            CategoryTypeId = (int)Mte.EditorList,
            EditorTypeId = (int)Mte.DateEditor,
            TypeRank = 14,
            Name = "DateTime",
            Description = "C# DateTime model",
            IsVisible = false,
            IsReadonly = true,
            IconName = ""
          }, //   178 DateTime model
          new ModelType {
            Id = (int)Mte.CSharpDateType,
            OwnerTypeId = (int)Mte.CSharpTypes,
            CategoryTypeId = (int)Mte.EditorList,
            EditorTypeId = (int)Mte.StringEditor,
            TypeRank = 15,
            Name = "Guid",
            Description = "C# Guid model",
            IsVisible = false,
            IsReadonly = true,
            IconName = ""
          }, //   180 Guid model
          new ModelType {
            Id = (int)Mte.CSharpTimeType,
            OwnerTypeId = (int)Mte.CSharpTypes,
            CategoryTypeId = (int)Mte.EditorList,
            EditorTypeId = (int)Mte.HiddenEditor,
            TypeRank = 16,
            Name = "object",
            Description = "C# object model",
            IsVisible = false,
            IsReadonly = true,
            IconName = ""
          }, //   182 object model
      #endregion
      #region UI Models
          new ModelType {
              Id = (int)Mte.ProjectModel,
              OwnerTypeId = (int)Mte.CategoryRoot,
              CategoryTypeId = (int)Mte.ProjectModel,
              EditorTypeId = (int)Mte.HiddenEditor,
              TypeRank = 1,
              Name = "Project Templates",
              Description = "",
              IsVisible = false,
              IsReadonly = true,
              IconName = "pi-microchip"
          }, // 200 Project Templates category type

          new ModelType {
              Id = (int)Mte.DatabaseModel,
              OwnerTypeId = (int)Mte.ProjectModel,
              CategoryTypeId = (int)Mte.DatabaseModel,
              EditorTypeId = (int)Mte.StringEditor,
              TypeRank = 1,
              Name = "Database",
              Description = "database model",
              IsVisible = true,
              IsReadonly = false,
              IconName = "pi-database"
          }, // 1 210 Database model
          new ModelType {
              Id = (int)Mte.TablesModel,
              OwnerTypeId = (int)Mte.DatabaseModel,
              CategoryTypeId = (int)Mte.TablesModel,
              EditorTypeId = (int)Mte.StringEditor,
              TypeRank = 1,
              Name = "Tables",
              Description = "database tables folder",
              IsVisible = true,
              IsReadonly = true,
              IconName = "pi-folder"
          }, // 1   220 Tables folder model
          new ModelType {
              Id = (int)Mte.TableModel,
              OwnerTypeId = (int)Mte.TablesModel,
              CategoryTypeId = (int)Mte.TableModel,
              EditorTypeId = (int)Mte.StringEditor,
              TypeRank = 1,
              Name = "Table",
              Description = "database table",
              IsVisible = true,
              IsReadonly = false,
              IconName = "pi-table"
          }, // 1     222 Table model
          new ModelType {
              Id = (int)Mte.TableColumnModel,
              OwnerTypeId = (int)Mte.TableModel,
              CategoryTypeId = (int)Mte.TableModel,
              EditorTypeId = (int)Mte.StringEditor,
              TypeRank = 1,
              Name = "TableColumn",
              Description = "database table column model",
              IsVisible = true,
              IsReadonly = false,
              IconName = "pi-stop"
          }, // 1       226 Table column model
          new ModelType {
              Id = (int)Mte.ViewsModel,
              OwnerTypeId = (int)Mte.DatabaseModel,
              CategoryTypeId = (int)Mte.ViewsModel,
              EditorTypeId = (int)Mte.StringEditor,
              TypeRank = 2,
              Name = "Views",
              Description = "database views folder",
              IsVisible = true,
              IsReadonly = true,
              IconName = "pi-folder"
          }, // 1   230 Views folder model
          new ModelType {
              Id = (int)Mte.ViewModel,
              OwnerTypeId = (int)Mte.ViewsModel,
              CategoryTypeId = (int)Mte.ViewModel,
              EditorTypeId = (int)Mte.StringEditor,
              TypeRank = 1,
              Name = "View",
              Description = "database View",
              IsVisible = true,
              IsReadonly = false,
              IconName = "pi-table"
          }, // 1     232 View model
          new ModelType {
              Id = (int)Mte.ViewColumnModel,
              OwnerTypeId = (int)Mte.ViewModel,
              CategoryTypeId = (int)Mte.ViewColumnModel,
              EditorTypeId = (int)Mte.StringEditor,
              TypeRank = 1,
              Name = "ViewColumn",
              Description = "database view column model",
              IsVisible = true,
              IsReadonly = false,
              IconName = "pi-stop"
          }, // 1       236 view column model
          new ModelType {
              Id = (int)Mte.FunctionsModel,
              OwnerTypeId = (int)Mte.DatabaseModel,
              CategoryTypeId = (int)Mte.FunctionsModel,
              EditorTypeId = (int)Mte.StringEditor,
              TypeRank = 3,
              Name = "Functions",
              Description = "database functions folder",
              IsVisible = true,
              IsReadonly = true,
              IconName = "pi-folder"
          }, // 1   240 Functions folder model
          new ModelType {
            Id = (int)Mte.FunctionModel,
            OwnerTypeId = (int)Mte.FunctionsModel,
            CategoryTypeId = (int)Mte.FunctionModel,
            EditorTypeId = (int)Mte.StringEditor,
            TypeRank = 1,
            Name = "Function",
            Description = "database function",
            IsVisible = true,
            IsReadonly = false,
            IconName = "pi-tablet"
          }, // 1     242 Function model
          new ModelType {
            Id = (int)Mte.FunctionParameterModel,
            OwnerTypeId = (int)Mte.FunctionModel,
            CategoryTypeId = (int)Mte.FunctionParameterModel,
            EditorTypeId = (int)Mte.StringEditor,
            TypeRank = 1,
            Name = "FunctionParameter",
            Description = "database function parameter model",
            IsVisible = true,
            IsReadonly = false,
            IconName = "pi-stop"
          }, // 1       246 Function parameter model
          new ModelType {
              Id = (int)Mte.ProceduresModel,
              OwnerTypeId = (int)Mte.DatabaseModel,
              CategoryTypeId = (int)Mte.ProceduresModel,
              EditorTypeId = (int)Mte.StringEditor,
              TypeRank = 4,
              Name = "Procedures",
              Description = "database procedures folder",
              IsVisible = true,
              IsReadonly = true,
              IconName = "pi-folder"
          }, // 1   250 Procedures folder model
          new ModelType {
              Id = (int)Mte.ProcedureModel,
              OwnerTypeId = (int)Mte.ProceduresModel,
              CategoryTypeId = (int)Mte.ProcedureModel,
              EditorTypeId = (int)Mte.StringEditor,
              TypeRank = 1,
              Name = "Procedure",
              Description = "database procedure",
              IsVisible = true,
              IsReadonly = false,
              IconName = "pi-microchip"
          }, // 1     252 Procedure model
          new ModelType {
              Id = (int)Mte.ProcedureParameterModel,
              OwnerTypeId = (int)Mte.ProcedureModel,
              CategoryTypeId = (int)Mte.ProcedureParameterModel,
              EditorTypeId = (int)Mte.StringEditor,
              TypeRank = 1,
              Name = "Parameter",
              Description = "database procedure parameter model",
              IsVisible = true,
              IsReadonly = false,
              IconName = "pi-stop"
          }, // 1       256 procedure parameter model


          new ModelType {
            Id = (int)Mte.ApiModel,
            OwnerTypeId = (int)Mte.DatabaseModel,
            CategoryTypeId = (int)Mte.ApiModel,
            EditorTypeId = (int)Mte.StringEditor,
            TypeRank = 2,
            Name = "Api",
            Description = "API model",
            IsVisible = true,
            IsReadonly = false,
            IconName = "pi-warehouse"
          }, // 2 300 Api model
          new ModelType {
            Id = (int)Mte.InterfaceModel,
            OwnerTypeId = (int)Mte.ApiModel,
            CategoryTypeId = (int)Mte.InterfaceModel,
            EditorTypeId = (int)Mte.StringEditor,
            TypeRank = 1,
            Name = "Interface",
            Description = "API interface model",
            IsVisible = true,
            IsReadonly = false,
            IconName = "pi-microchip"
          }, // 2   301 Api interface model
          new ModelType {
            Id = (int)Mte.InterfacePropertyModel,
            OwnerTypeId = (int)Mte.InterfaceModel,
            CategoryTypeId = (int)Mte.InterfacePropertyModel,
            EditorTypeId = (int)Mte.StringEditor,
            TypeRank = 1,
            Name = "Property",
            Description = "API interface property model",
            IsVisible = true,
            IsReadonly = false,
            IconName = "pi-tag"
          }, // 2     302 API interface property model
          new ModelType {
            Id = (int)Mte.InterfaceMethodModel,
            OwnerTypeId = (int)Mte.InterfaceModel,
            CategoryTypeId = (int)Mte.InterfaceMethodModel,
            EditorTypeId = (int)Mte.StringEditor,
            TypeRank = 1,
            Name = "Method",
            Description = "API interface method model",
            IsVisible = true,
            IsReadonly = false,
            IconName = "pi-stop"
          }, // 2     304 API interface method model
          new ModelType {
            Id = (int)Mte.InterfaceMethodParameterModel,
            OwnerTypeId = (int)Mte.InterfaceMethodModel,
            CategoryTypeId = (int)Mte.InterfaceMethodParameterModel,
            EditorTypeId = (int)Mte.StringEditor,
            TypeRank = 1,
            Name = "Parameter",
            Description = "API interface method parameter model",
            IsVisible = true,
            IsReadonly = false,
            IconName = "pi-tablet"
          }, // 2       306 API interface method parameter model
          new ModelType {
            Id = (int)Mte.ControllerModel,
            OwnerTypeId = (int)Mte.ApiModel,
            CategoryTypeId = (int)Mte.ControllerModel,
            EditorTypeId = (int)Mte.StringEditor,
            TypeRank = 1,
            Name = "Controller",
            Description = "API controller model",
            IsVisible = true,
            IsReadonly = false,
            IconName = "pi-microchip"
          }, // 2   310 Api controller model
          new ModelType {
            Id = (int)Mte.ControllerPropertyModel,
            OwnerTypeId = (int)Mte.ControllerModel,
            CategoryTypeId = (int)Mte.ControllerPropertyModel,
            EditorTypeId = (int)Mte.StringEditor,
            TypeRank = 1,
            Name = "Property",
            Description = "API controller property model",
            IsVisible = true,
            IsReadonly = false,
            IconName = "pi-tag"
          }, // 2     312 API controller property model
          new ModelType {
            Id = (int)Mte.ControllerMethodModel,
            OwnerTypeId = (int)Mte.ControllerModel,
            CategoryTypeId = (int)Mte.ControllerMethodModel,
            EditorTypeId = (int)Mte.StringEditor,
            TypeRank = 1,
            Name = "Method",
            Description = "API controller method model",
            IsVisible = true,
            IsReadonly = false,
            IconName = "pi-stop"
          }, // 2     314 API controller method model
          new ModelType {
            Id = (int)Mte.ControllerMethodParameterModel,
            OwnerTypeId = (int)Mte.ControllerMethodModel,
            CategoryTypeId = (int)Mte.ControllerMethodParameterModel,
            EditorTypeId = (int)Mte.StringEditor,
            TypeRank = 1,
            Name = "Parameter",
            Description = "API controller method parameter model",
            IsVisible = true,
            IsReadonly = false,
            IconName = "pi-tablet"
          }, // 2       316 API controller method parameter model
          new ModelType {
            Id = (int)Mte.ClassModel,
            OwnerTypeId = (int)Mte.ApiModel,
            CategoryTypeId = (int)Mte.ClassModel,
            EditorTypeId = (int)Mte.StringEditor,
            TypeRank = 1,
            Name = "Class",
            Description = "API class model",
            IsVisible = true,
            IsReadonly = false,
            IconName = "pi-microchip"
          }, // 2   320 Api class model
          new ModelType {
            Id = (int)Mte.ClassPropertyModel,
            OwnerTypeId = (int)Mte.ClassModel,
            CategoryTypeId = (int)Mte.ClassPropertyModel,
            EditorTypeId = (int)Mte.StringEditor,
            TypeRank = 1,
            Name = "Property",
            Description = "API class property model",
            IsVisible = true,
            IsReadonly = false,
            IconName = "pi-tag"
          }, // 2     322 API class property model
          new ModelType {
            Id = (int)Mte.ClassMethodModel,
            OwnerTypeId = (int)Mte.ClassModel,
            CategoryTypeId = (int)Mte.ClassMethodModel,
            EditorTypeId = (int)Mte.StringEditor,
            TypeRank = 1,
            Name = "Method",
            Description = "API class method model",
            IsVisible = true,
            IsReadonly = false,
            IconName = "pi-stop"
          }, // 2     324 API class method model
          new ModelType {
            Id = (int)Mte.ClassMethodParameterModel,
            OwnerTypeId = (int)Mte.ClassMethodModel,
            CategoryTypeId = (int)Mte.ClassMethodParameterModel,
            EditorTypeId = (int)Mte.StringEditor,
            TypeRank = 1,
            Name = "Parameter",
            Description = "API class method parameter model",
            IsVisible = true,
            IsReadonly = false,
            IconName = "pi-tablet"
          }, // 2       326 API class method parameter model
      #endregion
      #region Templates Models
          new ModelType {
            Id = (int)Mte.RootTemplate,
            OwnerTypeId = (int)Mte.ProjectModel,  // Owner drives the Menus for add.
            CategoryTypeId = (int)Mte.ProjectModel,
            EditorTypeId = (int)Mte.StringEditor,
            TypeRank = 3,
            Name = "RootTemplate",
            Description = "Template folder root",
            IsVisible = true,
            IsReadonly = false,
            IconName = "pi-file-edit"
          }, // 3 400 Templates root type
          new ModelType {
            Id = (int)Mte.FolderTemplate,
            OwnerTypeId = (int)Mte.RootTemplate,
            CategoryTypeId = (int)Mte.FolderTemplate,
            EditorTypeId = (int)Mte.StringEditor,
            TypeRank = 1,
            Name = "FolderTemplate",
            Description = "Template folder",
            IsVisible = true,
            IsReadonly = false,
            IconName = "pi-folder"
          }, //   402 Template folder type

          new ModelType {
              Id = (int)Mte.DatabaseTemplate,
              OwnerTypeId = (int)Mte.RootTemplate,
              CategoryTypeId = (int)Mte.DatabaseTemplate,
              EditorTypeId = (int)Mte.StringEditor,
              TypeRank = 1,
              Name = "DatabaseTemplate",
              Description = "Code generation database template",
              IsVisible = true,
              IsReadonly = false,
              IconName = "pi-database"
          }, //   410 DatabaseTemplate type
          new ModelType {
              Id = (int)Mte.ApiTemplate,
              OwnerTypeId = (int)Mte.RootTemplate,
              CategoryTypeId = (int)Mte.ApiTemplate,
              EditorTypeId = (int)Mte.StringEditor,
              TypeRank = 1,
              Name = "ApiTemplate",
              Description = "Code generation API template",
              IsVisible = true,
              IsReadonly = false,
              IconName = "pi-code"
          }, //   412 ApiTemplate type

          new ModelType {
            Id = (int)Mte.TablesTemplate,
            OwnerTypeId = (int)Mte.RootTemplate,
            CategoryTypeId = (int)Mte.TablesTemplate,
            EditorTypeId = (int)Mte.StringEditor,
            TypeRank = 1,
            Name = "TablesTemplate",
            Description = "Code generation table template",
            IsVisible = true,
            IsReadonly = false,
            IconName = "pi-table"
          }, //   420 TablesTemplate type
          new ModelType {
            Id = (int)Mte.ViewsTemplate,
            OwnerTypeId = (int)Mte.RootTemplate,
            CategoryTypeId = (int)Mte.ViewsTemplate,
            EditorTypeId = (int)Mte.StringEditor,
            TypeRank = 1,
            Name = "ViewsTemplate",
            Description = "Code generation views template",
            IsVisible = true,
            IsReadonly = false,
            IconName = "pi-table"
          }, //   422 ViewsTemplate type
          new ModelType {
            Id = (int)Mte.FunctionsTemplate,
            OwnerTypeId = (int)Mte.RootTemplate,
            CategoryTypeId = (int)Mte.FunctionsTemplate,
            EditorTypeId = (int)Mte.StringEditor,
            TypeRank = 1,
            Name = "FunctionsTemplate",
            Description = "Code generation functions template",
            IsVisible = true,
            IsReadonly = false,
            IconName = "pi-tablet"
          }, //   424 FunctionsTemplate type
          new ModelType {
            Id = (int)Mte.ProceduresTemplate,
            OwnerTypeId = (int)Mte.RootTemplate,
            CategoryTypeId = (int)Mte.ProceduresTemplate,
            EditorTypeId = (int)Mte.StringEditor,
            TypeRank = 1,
            Name = "ProceduresTemplate",
            Description = "Code generation procedures template",
            IsVisible = true,
            IsReadonly = false,
            IconName = "pi-microchip"
          }, //   424 ProceduresTemplate type

          new ModelType {
            Id = (int)Mte.TableTemplate,
            OwnerTypeId = (int)Mte.RootTemplate,
            CategoryTypeId = (int)Mte.TableTemplate,
            EditorTypeId = (int)Mte.StringEditor,
            TypeRank = 1,
            Name = "TableTemplate",
            Description = "Code generation table template",
            IsVisible = true,
            IsReadonly = false,
            IconName = "pi-table"
          }, //   430 TableTemplate type
          new ModelType {
              Id = (int)Mte.ViewTemplate,
              OwnerTypeId = (int)Mte.RootTemplate,
              CategoryTypeId = (int)Mte.ViewTemplate,
              EditorTypeId = (int)Mte.StringEditor,
              TypeRank = 1,
              Name = "ViewTemplate",
              Description = "Code generation view template",
              IsVisible = true,
              IsReadonly = false,
              IconName = "pi-table"
          }, //   432 ViewTemplate type
          new ModelType {
              Id = (int)Mte.FunctionTemplate,
              OwnerTypeId = (int)Mte.RootTemplate,
              CategoryTypeId = (int)Mte.FunctionTemplate,
              EditorTypeId = (int)Mte.StringEditor,
              TypeRank = 1,
              Name = "FunctionTemplate",
              Description = "Code generation function template",
              IsVisible = true,
              IsReadonly = false,
              IconName = "pi-tablet"
          }, //   434 FunctionTemplate type
          new ModelType {
              Id = (int)Mte.ProcedureTemplate,
              OwnerTypeId = (int)Mte.RootTemplate,
              CategoryTypeId = (int)Mte.ProcedureTemplate,
              EditorTypeId = (int)Mte.StringEditor,
              TypeRank = 1,
              Name = "ProcedureTemplate",
              Description = "Code generation procedure template",
              IsVisible = true,
              IsReadonly = false,
              IconName = "pi-microchip"
          }, //   436 ProcedureTemplate type

          new ModelType {
              Id = (int)Mte.InterfaceTemplate,
              OwnerTypeId = (int)Mte.RootTemplate,
              CategoryTypeId = (int)Mte.InterfaceTemplate,
              EditorTypeId = (int)Mte.StringEditor,
              TypeRank = 1,
              Name = "InterfaceTemplate",
              Description = "Code generation interface template",
              IsVisible = true,
              IsReadonly = false,
              IconName = "pi-microchip"
          }, //   440 InterfaceTemplate type
          new ModelType {
              Id = (int)Mte.ControllerTemplate,
              OwnerTypeId = (int)Mte.RootTemplate,
              CategoryTypeId = (int)Mte.ControllerTemplate,
              EditorTypeId = (int)Mte.StringEditor,
              TypeRank = 1,
              Name = "ControllerTemplate",
              Description = "Code generation controller template",
              IsVisible = true,
              IsReadonly = false,
              IconName = "pi-microchip"
          }, //   442 ControllerTemplate type
          new ModelType {
              Id = (int)Mte.ClassTemplate,
              OwnerTypeId = (int)Mte.RootTemplate,
              CategoryTypeId = (int)Mte.ClassTemplate,
              EditorTypeId = (int)Mte.StringEditor,
              TypeRank = 1,
              Name = "ClassTemplate",
              Description = "Code generation class template",
              IsVisible = true,
              IsReadonly = false,
              IconName = "pi-microchip"
          }  //   444 ClassTemplate type

      #endregion

      );
    }
  }

}
