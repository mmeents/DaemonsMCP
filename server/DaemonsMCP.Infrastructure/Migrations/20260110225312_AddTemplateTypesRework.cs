using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DaemonsMCP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTemplateTypesRework : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 19,
                column: "Name",
                value: "LookupTypeEditor");

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 50,
                column: "OwnerTypeId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 52,
                column: "CategoryTypeId",
                value: 50);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 53,
                column: "CategoryTypeId",
                value: 50);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 54,
                column: "CategoryTypeId",
                value: 50);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 55,
                column: "CategoryTypeId",
                value: 50);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 56,
                columns: new[] { "CategoryTypeId", "EditorTypeId" },
                values: new object[] { 50, null });

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 60,
                columns: new[] { "CategoryTypeId", "EditorTypeId", "OwnerTypeId" },
                values: new object[] { 3, 3, 3 });

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "CategoryTypeId", "EditorTypeId", "OwnerTypeId" },
                values: new object[] { 222, 19, 2 });

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 103,
                column: "TypeRank",
                value: 3);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 104,
                column: "TypeRank",
                value: 4);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 106,
                column: "TypeRank",
                value: 5);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 108,
                column: "TypeRank",
                value: 6);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 110,
                column: "TypeRank",
                value: 7);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 112,
                column: "TypeRank",
                value: 8);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 114,
                column: "TypeRank",
                value: 9);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 150,
                columns: new[] { "CategoryTypeId", "EditorTypeId" },
                values: new object[] { 320, 19 });

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 160,
                column: "TypeRank",
                value: 5);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 162,
                column: "TypeRank",
                value: 6);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 164,
                columns: new[] { "EditorTypeId", "TypeRank" },
                values: new object[] { 12, 7 });

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 166,
                columns: new[] { "EditorTypeId", "TypeRank" },
                values: new object[] { 12, 8 });

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 168,
                columns: new[] { "EditorTypeId", "TypeRank" },
                values: new object[] { 12, 9 });

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 170,
                columns: new[] { "EditorTypeId", "TypeRank" },
                values: new object[] { 17, 10 });

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 172,
                columns: new[] { "EditorTypeId", "TypeRank" },
                values: new object[] { 17, 11 });

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 174,
                columns: new[] { "EditorTypeId", "TypeRank" },
                values: new object[] { 17, 12 });

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 176,
                columns: new[] { "EditorTypeId", "TypeRank" },
                values: new object[] { 10, 13 });

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 200,
                columns: new[] { "CategoryTypeId", "EditorTypeId" },
                values: new object[] { 200, 10 });

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 210,
                column: "CategoryTypeId",
                value: 210);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 220,
                column: "CategoryTypeId",
                value: 220);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 222,
                column: "CategoryTypeId",
                value: 222);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 226,
                column: "CategoryTypeId",
                value: 222);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 230,
                column: "CategoryTypeId",
                value: 230);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 232,
                column: "CategoryTypeId",
                value: 232);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 236,
                column: "CategoryTypeId",
                value: 236);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 240,
                column: "CategoryTypeId",
                value: 240);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 242,
                column: "CategoryTypeId",
                value: 242);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 246,
                columns: new[] { "CategoryTypeId", "Description", "Name" },
                values: new object[] { 246, "database function parameter model", "FunctionParameter" });

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 250,
                column: "CategoryTypeId",
                value: 250);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 252,
                column: "CategoryTypeId",
                value: 252);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 256,
                column: "CategoryTypeId",
                value: 256);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 300,
                columns: new[] { "CategoryTypeId", "OwnerTypeId" },
                values: new object[] { 300, 210 });

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 301,
                column: "CategoryTypeId",
                value: 301);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 302,
                column: "CategoryTypeId",
                value: 302);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 304,
                column: "CategoryTypeId",
                value: 304);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 306,
                column: "CategoryTypeId",
                value: 306);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 310,
                column: "CategoryTypeId",
                value: 310);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 312,
                column: "CategoryTypeId",
                value: 312);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 314,
                column: "CategoryTypeId",
                value: 314);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 316,
                column: "CategoryTypeId",
                value: 316);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 320,
                column: "CategoryTypeId",
                value: 320);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 322,
                column: "CategoryTypeId",
                value: 322);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 324,
                column: "CategoryTypeId",
                value: 324);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 326,
                column: "CategoryTypeId",
                value: 326);

            migrationBuilder.InsertData(
                table: "ModelTypes",
                columns: new[] { "Id", "CategoryTypeId", "Description", "EditorTypeId", "IconName", "IsVisible", "Name", "OwnerTypeId", "TypeRank" },
                values: new object[] { 20, 3, "lookup on model editor", 20, "pi-search", true, "LookupModelEditor", 3, 11 });

            migrationBuilder.InsertData(
                table: "ModelTypes",
                columns: new[] { "Id", "CategoryTypeId", "Description", "EditorTypeId", "IconName", "IsReadonly", "Name", "OwnerTypeId", "TypeRank" },
                values: new object[,]
                {
                    { 178, 3, "C# DateTime model", 15, "", true, "DateTime", 150, 14 },
                    { 180, 3, "C# Guid model", 13, "", true, "Guid", 150, 15 },
                    { 182, 3, "C# object model", 10, "", true, "object", 150, 16 }
                });

            migrationBuilder.InsertData(
                table: "ModelTypes",
                columns: new[] { "Id", "CategoryTypeId", "Description", "EditorTypeId", "IconName", "IsVisible", "Name", "OwnerTypeId", "TypeRank" },
                values: new object[,]
                {
                    { 400, 200, "Template folder root", 13, "pi-file-edit", true, "RootTemplate", 200, 3 },
                    { 402, 402, "Template folder", 13, "pi-folder", true, "FolderTemplate", 400, 1 },
                    { 410, 410, "Code generation database template", 13, "pi-database", true, "DatabaseTemplate", 400, 1 },
                    { 412, 412, "Code generation API template", 13, "pi-code", true, "ApiTemplate", 400, 1 },
                    { 420, 420, "Code generation table template", 13, "pi-table", true, "TablesTemplate", 400, 1 },
                    { 422, 422, "Code generation views template", 13, "pi-table", true, "ViewsTemplate", 400, 1 },
                    { 424, 424, "Code generation functions template", 13, "pi-tablet", true, "FunctionsTemplate", 400, 1 },
                    { 426, 426, "Code generation procedures template", 13, "pi-microchip", true, "ProceduresTemplate", 400, 1 },
                    { 430, 430, "Code generation table template", 13, "pi-table", true, "TableTemplate", 400, 1 },
                    { 432, 432, "Code generation view template", 13, "pi-table", true, "ViewTemplate", 400, 1 },
                    { 434, 434, "Code generation function template", 13, "pi-tablet", true, "FunctionTemplate", 400, 1 },
                    { 436, 436, "Code generation procedure template", 13, "pi-microchip", true, "ProcedureTemplate", 400, 1 },
                    { 440, 440, "Code generation interface template", 13, "pi-microchip", true, "InterfaceTemplate", 400, 1 },
                    { 442, 442, "Code generation controller template", 13, "pi-microchip", true, "ControllerTemplate", 400, 1 },
                    { 444, 444, "Code generation class template", 13, "pi-microchip", true, "ClassTemplate", 400, 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 178);

            migrationBuilder.DeleteData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 180);

            migrationBuilder.DeleteData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 182);

            migrationBuilder.DeleteData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 402);

            migrationBuilder.DeleteData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 410);

            migrationBuilder.DeleteData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 412);

            migrationBuilder.DeleteData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 420);

            migrationBuilder.DeleteData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 422);

            migrationBuilder.DeleteData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 424);

            migrationBuilder.DeleteData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 426);

            migrationBuilder.DeleteData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 430);

            migrationBuilder.DeleteData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 432);

            migrationBuilder.DeleteData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 434);

            migrationBuilder.DeleteData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 436);

            migrationBuilder.DeleteData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 440);

            migrationBuilder.DeleteData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 442);

            migrationBuilder.DeleteData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 444);

            migrationBuilder.DeleteData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 400);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 19,
                column: "Name",
                value: "Lookup");

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 50,
                column: "OwnerTypeId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 52,
                column: "CategoryTypeId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 53,
                column: "CategoryTypeId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 54,
                column: "CategoryTypeId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 55,
                column: "CategoryTypeId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 56,
                columns: new[] { "CategoryTypeId", "EditorTypeId" },
                values: new object[] { 3, 13 });

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 60,
                columns: new[] { "CategoryTypeId", "EditorTypeId", "OwnerTypeId" },
                values: new object[] { 2, 2, 2 });

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "CategoryTypeId", "EditorTypeId", "OwnerTypeId" },
                values: new object[] { 2, 2, 3 });

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 103,
                column: "TypeRank",
                value: 2);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 104,
                column: "TypeRank",
                value: 8);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 106,
                column: "TypeRank",
                value: 9);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 108,
                column: "TypeRank",
                value: 3);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 110,
                column: "TypeRank",
                value: 4);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 112,
                column: "TypeRank",
                value: 5);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 114,
                column: "TypeRank",
                value: 6);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 150,
                columns: new[] { "CategoryTypeId", "EditorTypeId" },
                values: new object[] { 2, 2 });

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 160,
                column: "TypeRank",
                value: 4);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 162,
                column: "TypeRank",
                value: 4);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 164,
                columns: new[] { "EditorTypeId", "TypeRank" },
                values: new object[] { 13, 4 });

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 166,
                columns: new[] { "EditorTypeId", "TypeRank" },
                values: new object[] { 13, 4 });

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 168,
                columns: new[] { "EditorTypeId", "TypeRank" },
                values: new object[] { 13, 4 });

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 170,
                columns: new[] { "EditorTypeId", "TypeRank" },
                values: new object[] { 13, 4 });

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 172,
                columns: new[] { "EditorTypeId", "TypeRank" },
                values: new object[] { 13, 4 });

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 174,
                columns: new[] { "EditorTypeId", "TypeRank" },
                values: new object[] { 13, 4 });

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 176,
                columns: new[] { "EditorTypeId", "TypeRank" },
                values: new object[] { 13, 4 });

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 200,
                columns: new[] { "CategoryTypeId", "EditorTypeId" },
                values: new object[] { 2, 2 });

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 210,
                column: "CategoryTypeId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 220,
                column: "CategoryTypeId",
                value: 200);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 222,
                column: "CategoryTypeId",
                value: 200);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 226,
                column: "CategoryTypeId",
                value: 200);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 230,
                column: "CategoryTypeId",
                value: 200);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 232,
                column: "CategoryTypeId",
                value: 200);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 236,
                column: "CategoryTypeId",
                value: 200);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 240,
                column: "CategoryTypeId",
                value: 200);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 242,
                column: "CategoryTypeId",
                value: 200);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 246,
                columns: new[] { "CategoryTypeId", "Description", "Name" },
                values: new object[] { 200, "database function column model", "FunctionColumn" });

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 250,
                column: "CategoryTypeId",
                value: 200);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 252,
                column: "CategoryTypeId",
                value: 200);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 256,
                column: "CategoryTypeId",
                value: 200);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 300,
                columns: new[] { "CategoryTypeId", "OwnerTypeId" },
                values: new object[] { 3, 200 });

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 301,
                column: "CategoryTypeId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 302,
                column: "CategoryTypeId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 304,
                column: "CategoryTypeId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 306,
                column: "CategoryTypeId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 310,
                column: "CategoryTypeId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 312,
                column: "CategoryTypeId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 314,
                column: "CategoryTypeId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 316,
                column: "CategoryTypeId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 320,
                column: "CategoryTypeId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 322,
                column: "CategoryTypeId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 324,
                column: "CategoryTypeId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 326,
                column: "CategoryTypeId",
                value: 3);
        }
    }
}
