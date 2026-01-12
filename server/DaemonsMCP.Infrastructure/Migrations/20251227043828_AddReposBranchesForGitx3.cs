using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DaemonsMCP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReposBranchesForGitx3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RepositoryBranches_ProjectRepositories_ProjectRepositoryId",
                table: "RepositoryBranches");

            migrationBuilder.DropTable(
                name: "ProjectRepositories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RepositoryBranches",
                table: "RepositoryBranches");

            migrationBuilder.RenameTable(
                name: "RepositoryBranches",
                newName: "GitBranches");

            migrationBuilder.RenameColumn(
                name: "ProjectRepositoryId",
                table: "GitBranches",
                newName: "GitRepositoryId");

            migrationBuilder.RenameIndex(
                name: "UQ_RepositoryBranches_UniqueName",
                table: "GitBranches",
                newName: "UQ_GitBranches_UniqueName");

            migrationBuilder.RenameIndex(
                name: "IX_RepositoryBranches_ProjectRepositoryId",
                table: "GitBranches",
                newName: "IX_GitBranches_GitRepositoryId");

            migrationBuilder.RenameIndex(
                name: "IX_RepositoryBranches_IsCurrentBranch",
                table: "GitBranches",
                newName: "IX_GitBranches_IsCurrentBranch");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GitBranches",
                table: "GitBranches",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "GitRepositories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    UserCredentialId = table.Column<int>(type: "int", nullable: true),
                    RemoteUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    LocalPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    RemoteName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, defaultValue: "origin"),
                    CurrentBranchName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsDirty = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ModifiedFileCount = table.Column<int>(type: "int", nullable: true),
                    UntrackedFileCount = table.Column<int>(type: "int", nullable: true),
                    LastFetchedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastSyncedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastSyncStatus = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    LastSyncError = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GitRepositories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GitRepositories_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GitRepositories_UserCredentials_UserCredentialId",
                        column: x => x.UserCredentialId,
                        principalTable: "UserCredentials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GitRepositories_ProjectId",
                table: "GitRepositories",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_GitRepositories_UserCredentialId",
                table: "GitRepositories",
                column: "UserCredentialId");

            migrationBuilder.CreateIndex(
                name: "UQ_GitRepositories_ProjectPath",
                table: "GitRepositories",
                columns: new[] { "ProjectId", "LocalPath" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_GitBranches_GitRepositories_GitRepositoryId",
                table: "GitBranches",
                column: "GitRepositoryId",
                principalTable: "GitRepositories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GitBranches_GitRepositories_GitRepositoryId",
                table: "GitBranches");

            migrationBuilder.DropTable(
                name: "GitRepositories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GitBranches",
                table: "GitBranches");

            migrationBuilder.RenameTable(
                name: "GitBranches",
                newName: "RepositoryBranches");

            migrationBuilder.RenameColumn(
                name: "GitRepositoryId",
                table: "RepositoryBranches",
                newName: "ProjectRepositoryId");

            migrationBuilder.RenameIndex(
                name: "UQ_GitBranches_UniqueName",
                table: "RepositoryBranches",
                newName: "UQ_RepositoryBranches_UniqueName");

            migrationBuilder.RenameIndex(
                name: "IX_GitBranches_IsCurrentBranch",
                table: "RepositoryBranches",
                newName: "IX_RepositoryBranches_IsCurrentBranch");

            migrationBuilder.RenameIndex(
                name: "IX_GitBranches_GitRepositoryId",
                table: "RepositoryBranches",
                newName: "IX_RepositoryBranches_ProjectRepositoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RepositoryBranches",
                table: "RepositoryBranches",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ProjectRepositories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    UserCredentialId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CurrentBranchName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true, defaultValue: "main"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsDirty = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    LastFetchedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastSyncError = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    LastSyncStatus = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    LastSyncedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LocalPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ModifiedFileCount = table.Column<int>(type: "int", nullable: true),
                    RemoteName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, defaultValue: "origin"),
                    RemoteUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    UntrackedFileCount = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
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
                name: "IX_ProjectRepositories_UserCredentialId",
                table: "ProjectRepositories",
                column: "UserCredentialId");

            migrationBuilder.CreateIndex(
                name: "UQ_ProjectRepositories_ProjectPath",
                table: "ProjectRepositories",
                columns: new[] { "ProjectId", "LocalPath" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RepositoryBranches_ProjectRepositories_ProjectRepositoryId",
                table: "RepositoryBranches",
                column: "ProjectRepositoryId",
                principalTable: "ProjectRepositories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
