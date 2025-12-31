using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DaemonsMCP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReposBranchesForGitx2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_ProjectRepositories_ProjectId_LocalPath",
                table: "ProjectRepositories",
                newName: "UQ_ProjectRepositories_ProjectPath");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentBranchName",
                table: "ProjectRepositories",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                defaultValue: "main",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldDefaultValue: "main");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "UQ_ProjectRepositories_ProjectPath",
                table: "ProjectRepositories",
                newName: "IX_ProjectRepositories_ProjectId_LocalPath");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentBranchName",
                table: "ProjectRepositories",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "main",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldDefaultValue: "main");
        }
    }
}
