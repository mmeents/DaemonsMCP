using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DaemonsMCP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameModelPropandUpdateSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 232,
                columns: new[] { "Description", "Name" },
                values: new object[] { "database View", "View" });

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 236,
                columns: new[] { "Description", "Name" },
                values: new object[] { "database view column model", "ViewColumn" });

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 314,
                column: "OwnerTypeId",
                value: 310);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 324,
                column: "OwnerTypeId",
                value: 320);

            migrationBuilder.InsertData(
                table: "ModelTypes",
                columns: new[] { "Id", "CategoryTypeId", "Description", "EditorTypeId", "IconName", "IsVisible", "Name", "OwnerTypeId", "TypeRank" },
                values: new object[,]
                {
                    { 301, 3, "API interface model", 13, "pi-microchip", true, "Interface", 300, 1 },
                    { 302, 3, "API interface property model", 13, "pi-tag", true, "Property", 301, 1 },
                    { 304, 3, "API interface method model", 13, "pi-stop", true, "Method", 301, 1 },
                    { 306, 3, "API interface method parameter model", 13, "pi-tablet", true, "Parameter", 304, 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 302);

            migrationBuilder.DeleteData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 306);

            migrationBuilder.DeleteData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 304);

            migrationBuilder.DeleteData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 301);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 232,
                columns: new[] { "Description", "Name" },
                values: new object[] { "database table", "Table" });

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 236,
                columns: new[] { "Description", "Name" },
                values: new object[] { "database table column model", "TableColumn" });

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 314,
                column: "OwnerTypeId",
                value: 312);

            migrationBuilder.UpdateData(
                table: "ModelTypes",
                keyColumn: "Id",
                keyValue: 324,
                column: "OwnerTypeId",
                value: 322);
        }
    }
}
