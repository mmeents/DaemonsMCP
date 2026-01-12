using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DaemonsMCP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddModelTypeAndSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ModelTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerTypeId = table.Column<int>(type: "int", nullable: true),
                    CategoryTypeId = table.Column<int>(type: "int", nullable: true),
                    EditorTypeId = table.Column<int>(type: "int", nullable: true),
                    TypeRank = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, defaultValue: ""),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsReadonly = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IconName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModelTypes_ModelTypes_CategoryTypeId",
                        column: x => x.CategoryTypeId,
                        principalTable: "ModelTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ModelTypes_ModelTypes_EditorTypeId",
                        column: x => x.EditorTypeId,
                        principalTable: "ModelTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ModelTypes_ModelTypes_OwnerTypeId",
                        column: x => x.OwnerTypeId,
                        principalTable: "ModelTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "ModelTypes",
                columns: new[] { "Id", "CategoryTypeId", "Description", "EditorTypeId", "IconName", "IsReadonly", "Name", "OwnerTypeId" },
                values: new object[] { 1, null, "Base root type model", null, "pi-cog", true, "Root Internal Owner Type Model", null });

            migrationBuilder.InsertData(
                table: "ModelTypes",
                columns: new[] { "Id", "CategoryTypeId", "Description", "EditorTypeId", "IconName", "IsReadonly", "Name", "OwnerTypeId", "TypeRank" },
                values: new object[,]
                {
                    { 2, 2, "Adds node for categories dimension", 2, "pi-folder", true, "Categories", 1, 1 },
                    { 3, 2, "Adds node for editor types dimension", 2, "pi-pencil", true, "Editor Types", 1, 2 },
                    { 60, 2, "Accessibility types list", 2, "", true, "C# Accessibility Types", 2, 1 },
                    { 150, 2, "C# Type Lookups", 2, "", true, "C# Types", 2, 1 },
                    { 200, 2, "", 2, "pi-microchip", true, "Project Templates", 2, 1 },
                    { 10, 3, "editor is hidden", 10, "pi-eye-slash", true, "Hidden", 3, 1 }
                });

            migrationBuilder.InsertData(
                table: "ModelTypes",
                columns: new[] { "Id", "CategoryTypeId", "Description", "EditorTypeId", "IconName", "IsVisible", "Name", "OwnerTypeId", "TypeRank" },
                values: new object[,]
                {
                    { 11, 3, "boolean editor", 11, "pi-check", true, "Boolean", 3, 2 },
                    { 12, 3, "integer editor", 12, "pi-pencil", true, "Integer", 3, 3 },
                    { 13, 3, "string editor", 13, "pi-pencil", true, "String", 3, 4 },
                    { 14, 3, "filename editor", 14, "pi-file", true, "Filename", 3, 5 },
                    { 15, 3, "date editor", 15, "pi-calendar", true, "Date", 3, 6 },
                    { 16, 3, "time editor", 16, "pi-clock", true, "Time", 3, 7 },
                    { 17, 3, "decimal editor", 17, "pi-dollar", true, "Decimal", 3, 8 },
                    { 18, 3, "password editor", 18, "pi-lock", true, "Password", 3, 9 },
                    { 19, 3, "lookup editor", 19, "pi-search", true, "Lookup", 3, 10 }
                });

            migrationBuilder.InsertData(
                table: "ModelTypes",
                columns: new[] { "Id", "CategoryTypeId", "Description", "EditorTypeId", "IconName", "IsReadonly", "IsVisible", "Name", "OwnerTypeId", "TypeRank" },
                values: new object[,]
                {
                    { 50, 3, "HTTP method types editor", null, "pi-pencil", true, true, "HTTP Method Types", 2, 11 },
                    { 62, 3, "public accessibility", null, "", true, true, "public", 60, 1 },
                    { 64, 3, "private accessibility", null, "", true, true, "private", 60, 2 },
                    { 66, 3, "protected accessibility", null, "", true, true, "protected", 60, 3 },
                    { 68, 3, "internal accessibility", null, "", true, true, "internal", 60, 4 }
                });

            migrationBuilder.InsertData(
                table: "ModelTypes",
                columns: new[] { "Id", "CategoryTypeId", "Description", "EditorTypeId", "IconName", "IsReadonly", "Name", "OwnerTypeId", "TypeRank" },
                values: new object[] { 100, 2, "Sql data types list", 2, "", true, "Sql Types", 3, 1 });

            migrationBuilder.InsertData(
                table: "ModelTypes",
                columns: new[] { "Id", "CategoryTypeId", "Description", "EditorTypeId", "IconName", "IsReadonly", "IsVisible", "Name", "OwnerTypeId", "TypeRank" },
                values: new object[,]
                {
                    { 52, 3, "HTTP GET method", null, "pi-eye", true, true, "GET", 50, 1 },
                    { 53, 3, "HTTP POST method", null, "pi-pencil", true, true, "POST", 50, 2 },
                    { 54, 3, "HTTP PUT method", null, "pi-pencil", true, true, "PUT", 50, 3 },
                    { 55, 3, "HTTP DELETE method", null, "pi-delete-left", true, true, "DELETE", 50, 4 },
                    { 56, 3, "HTTP PATCH method", 13, "pi-pencil", true, true, "PATCH", 50, 5 }
                });

            migrationBuilder.InsertData(
                table: "ModelTypes",
                columns: new[] { "Id", "CategoryTypeId", "Description", "EditorTypeId", "IconName", "IsVisible", "Name", "OwnerTypeId", "TypeRank" },
                values: new object[,]
                {
                    { 101, 3, "SQL bit type", 11, "", true, "bit", 100, 1 },
                    { 102, 3, "SQL Small Integer type", 12, "", true, "smallint", 100, 2 },
                    { 103, 3, "SQL Integer type", 12, "", true, "int", 100, 2 },
                    { 104, 3, "SQL bigint type", 12, "", true, "bigint", 100, 8 },
                    { 106, 3, "SQL uniqueidentifier type", 13, "", true, "uniqueidentifier", 100, 9 },
                    { 108, 3, "SQL Variable Character type", 13, "", true, "varchar", 100, 3 },
                    { 110, 3, "SQL National Variable Character type", 13, "", true, "nvarchar", 100, 4 },
                    { 112, 3, "SQL Decimal type", 17, "", true, "decimal", 100, 5 },
                    { 114, 3, "SQL DateTime type", 15, "", true, "datetime", 100, 6 },
                    { 118, 3, "SQL date type", 15, "", true, "date", 100, 10 },
                    { 120, 3, "SQL time type", 16, "", true, "time", 100, 11 }
                });

            migrationBuilder.InsertData(
                table: "ModelTypes",
                columns: new[] { "Id", "CategoryTypeId", "Description", "EditorTypeId", "IconName", "IsReadonly", "Name", "OwnerTypeId", "TypeRank" },
                values: new object[,]
                {
                    { 152, 3, "C# class model", 13, "", true, "class", 150, 1 },
                    { 154, 3, "C# record model", 13, "", true, "record", 150, 2 },
                    { 156, 3, "C# struct model", 13, "", true, "struct", 150, 3 },
                    { 158, 3, "C# string model", 13, "", true, "string", 150, 4 },
                    { 160, 3, "C# bool model", 13, "", true, "bool", 150, 4 },
                    { 162, 3, "C# char model", 13, "", true, "char", 150, 4 },
                    { 164, 3, "C# int model", 13, "", true, "int", 150, 4 },
                    { 166, 3, "C# long model", 13, "", true, "long", 150, 4 },
                    { 168, 3, "C# short model", 13, "", true, "short", 150, 4 },
                    { 170, 3, "C# decimal model", 13, "", true, "decimal", 150, 4 },
                    { 172, 3, "C# double model", 13, "", true, "double", 150, 4 },
                    { 174, 3, "C# float model", 13, "", true, "float", 150, 4 },
                    { 176, 3, "C# byte model", 13, "", true, "byte", 150, 4 }
                });

            migrationBuilder.InsertData(
                table: "ModelTypes",
                columns: new[] { "Id", "CategoryTypeId", "Description", "EditorTypeId", "IconName", "IsVisible", "Name", "OwnerTypeId", "TypeRank" },
                values: new object[,]
                {
                    { 210, 3, "database model", 13, "pi-database", true, "Database", 200, 1 },
                    { 300, 3, "API model", 13, "pi-warehouse", true, "Api", 200, 2 }
                });

            migrationBuilder.InsertData(
                table: "ModelTypes",
                columns: new[] { "Id", "CategoryTypeId", "Description", "EditorTypeId", "IconName", "IsReadonly", "IsVisible", "Name", "OwnerTypeId", "TypeRank" },
                values: new object[,]
                {
                    { 220, 200, "database tables folder", 13, "pi-folder", true, true, "Tables", 210, 1 },
                    { 230, 200, "database views folder", 13, "pi-folder", true, true, "Views", 210, 2 },
                    { 240, 200, "database functions folder", 13, "pi-folder", true, true, "Functions", 210, 3 },
                    { 250, 200, "database procedures folder", 13, "pi-folder", true, true, "Procedures", 210, 4 }
                });

            migrationBuilder.InsertData(
                table: "ModelTypes",
                columns: new[] { "Id", "CategoryTypeId", "Description", "EditorTypeId", "IconName", "IsVisible", "Name", "OwnerTypeId", "TypeRank" },
                values: new object[,]
                {
                    { 310, 3, "API controller model", 13, "pi-microchip", true, "Controller", 300, 1 },
                    { 320, 3, "API class model", 13, "pi-microchip", true, "Class", 300, 1 },
                    { 222, 200, "database table", 13, "pi-table", true, "Table", 220, 1 },
                    { 232, 200, "database table", 13, "pi-table", true, "Table", 230, 1 },
                    { 242, 200, "database function", 13, "pi-tablet", true, "Function", 240, 1 },
                    { 252, 200, "database procedure", 13, "pi-microchip", true, "Procedure", 250, 1 },
                    { 312, 3, "API controller property model", 13, "pi-tag", true, "Property", 310, 1 },
                    { 322, 3, "API class property model", 13, "pi-tag", true, "Property", 320, 1 },
                    { 226, 200, "database table column model", 13, "pi-stop", true, "TableColumn", 222, 1 },
                    { 236, 200, "database table column model", 13, "pi-stop", true, "TableColumn", 232, 1 },
                    { 246, 200, "database function column model", 13, "pi-stop", true, "FunctionColumn", 242, 1 },
                    { 256, 200, "database procedure parameter model", 13, "pi-stop", true, "Parameter", 252, 1 },
                    { 314, 3, "API controller method model", 13, "pi-stop", true, "Method", 312, 1 },
                    { 324, 3, "API class method model", 13, "pi-stop", true, "Method", 322, 1 },
                    { 316, 3, "API controller method parameter model", 13, "pi-tablet", true, "Parameter", 314, 1 },
                    { 326, 3, "API class method parameter model", 13, "pi-tablet", true, "Parameter", 324, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModelTypes_CategoryTypeId",
                table: "ModelTypes",
                column: "CategoryTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ModelTypes_EditorTypeId",
                table: "ModelTypes",
                column: "EditorTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ModelTypes_Name",
                table: "ModelTypes",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ModelTypes_OwnerTypeId",
                table: "ModelTypes",
                column: "OwnerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ModelTypes_OwnerTypeId_TypeRank",
                table: "ModelTypes",
                columns: new[] { "OwnerTypeId", "TypeRank" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModelTypes");
        }
    }
}
