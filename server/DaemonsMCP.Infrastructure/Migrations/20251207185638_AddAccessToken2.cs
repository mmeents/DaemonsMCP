using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DaemonsMCP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAccessToken2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UsedBy",
                table: "AccessTokens",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsedUrl",
                table: "AccessTokens",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccessTokens_UsedBy",
                table: "AccessTokens",
                column: "UsedBy");

            migrationBuilder.CreateIndex(
                name: "IX_AccessTokens_UsedUrl",
                table: "AccessTokens",
                column: "UsedUrl");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AccessTokens_UsedBy",
                table: "AccessTokens");

            migrationBuilder.DropIndex(
                name: "IX_AccessTokens_UsedUrl",
                table: "AccessTokens");

            migrationBuilder.DropColumn(
                name: "UsedBy",
                table: "AccessTokens");

            migrationBuilder.DropColumn(
                name: "UsedUrl",
                table: "AccessTokens");
        }
    }
}
