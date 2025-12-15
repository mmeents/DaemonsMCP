using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DaemonsMCP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddsIssuedToAndStuff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AccessTokens_Expires_Used",
                table: "AccessTokens");

            migrationBuilder.AddColumn<string>(
                name: "IssuedTo",
                table: "AccessTokens",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "unknown");

            migrationBuilder.AddColumn<string>(
                name: "UsedUrlNextToken",
                table: "AccessTokens",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccessTokens_Expires_UsedUrl",
                table: "AccessTokens",
                columns: new[] { "Expires", "UsedUrl" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AccessTokens_Expires_UsedUrl",
                table: "AccessTokens");

            migrationBuilder.DropColumn(
                name: "IssuedTo",
                table: "AccessTokens");

            migrationBuilder.DropColumn(
                name: "UsedUrlNextToken",
                table: "AccessTokens");

            migrationBuilder.CreateIndex(
                name: "IX_AccessTokens_Expires_Used",
                table: "AccessTokens",
                columns: new[] { "Expires", "Used" });
        }
    }
}
