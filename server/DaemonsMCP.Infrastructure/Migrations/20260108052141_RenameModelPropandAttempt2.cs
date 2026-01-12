using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DaemonsMCP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameModelPropandAttempt2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          migrationBuilder.RenameColumn(
            name: "PropertyModelTypeId",
            table: "ModelProperties",
            newName: "PropertyValueTypeId");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
          migrationBuilder.RenameColumn(
            name: "PropertyValueTypeId",
            table: "ModelProperties",
            newName: "PropertyModelTypeId");
        }
    }
}
