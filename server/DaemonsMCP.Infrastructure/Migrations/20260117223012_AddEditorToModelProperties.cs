using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DaemonsMCP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEditorToModelProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PropertyEditorTypeId",
                table: "ModelProperties",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ModelProperties_PropertyEditorTypeId",
                table: "ModelProperties",
                column: "PropertyEditorTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ModelProperties_ModelTypes_PropertyEditorTypeId",
                table: "ModelProperties",
                column: "PropertyEditorTypeId",
                principalTable: "ModelTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ModelProperties_ModelTypes_PropertyEditorTypeId",
                table: "ModelProperties");

            migrationBuilder.DropIndex(
                name: "IX_ModelProperties_PropertyEditorTypeId",
                table: "ModelProperties");

            migrationBuilder.DropColumn(
                name: "PropertyEditorTypeId",
                table: "ModelProperties");
        }
    }
}
