using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DaemonsMCP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReposCredsForGit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserCredentials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ProviderType = table.Column<int>(type: "int", nullable: false),
                    CredentialType = table.Column<int>(type: "int", nullable: false),
                    EncryptedUsername = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EncryptedSecret = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    LastUsedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCredentials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCredentials_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectRepositories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    UserCredentialId = table.Column<int>(type: "int", nullable: true),
                    RemoteUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    LocalPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DefaultBranch = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, defaultValue: "main"),
                    LastSyncDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastSyncStatus = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    LastSyncError = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectRepositories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectRepositories_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectRepositories_UserCredentials_UserCredentialId",
                        column: x => x.UserCredentialId,
                        principalTable: "UserCredentials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectRepositories_ProjectId",
                table: "ProjectRepositories",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectRepositories_ProjectId_LocalPath",
                table: "ProjectRepositories",
                columns: new[] { "ProjectId", "LocalPath" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectRepositories_UserCredentialId",
                table: "ProjectRepositories",
                column: "UserCredentialId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCredentials_UserId",
                table: "UserCredentials",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCredentials_UserId_IsActive",
                table: "UserCredentials",
                columns: new[] { "UserId", "IsActive" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectRepositories");

            migrationBuilder.DropTable(
                name: "UserCredentials");
        }
    }
}
