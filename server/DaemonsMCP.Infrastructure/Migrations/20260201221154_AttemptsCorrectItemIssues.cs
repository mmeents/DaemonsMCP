using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DaemonsMCP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AttemptsCorrectItemIssues : Migration
    {
    protected override void Up(MigrationBuilder migrationBuilder) {
      // Drop the index first
      migrationBuilder.DropIndex(
          name: "IX_Items_ItemTypeId1",
          table: "Items");

      // Drop the FK constraint second
      migrationBuilder.DropForeignKey(
          name: "FK_Items_ItemTypes_ItemTypeId1",
          table: "Items");

      // Drop the column last
      migrationBuilder.DropColumn(
          name: "ItemTypeId1",
          table: "Items");
    }

    protected override void Down(MigrationBuilder migrationBuilder) {
      // Add the column back first
      migrationBuilder.AddColumn<int>(
          name: "ItemTypeId1",
          table: "Items",
          type: "int",
          nullable: true);

      // Recreate the FK constraint second
      migrationBuilder.AddForeignKey(
          name: "FK_Items_ItemTypes_ItemTypeId1",
          table: "Items",
          column: "ItemTypeId1",
          principalTable: "ItemTypes",
          principalColumn: "Id");

      // Recreate the index last
      migrationBuilder.CreateIndex(
          name: "IX_Items_ItemTypeId1",
          table: "Items",
          column: "ItemTypeId1");
    }
  }
}
