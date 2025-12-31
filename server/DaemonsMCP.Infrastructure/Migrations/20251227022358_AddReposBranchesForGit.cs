using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DaemonsMCP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReposBranchesForGit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultBranch",
                table: "ProjectRepositories");

            migrationBuilder.RenameColumn(
                name: "LastSyncDate",
                table: "ProjectRepositories",
                newName: "LastSyncedAt");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ProjectRepositories",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<string>(
                name: "CurrentBranchName",
                table: "ProjectRepositories",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "main");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ProjectRepositories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDirty",
                table: "ProjectRepositories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastFetchedAt",
                table: "ProjectRepositories",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedFileCount",
                table: "ProjectRepositories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RemoteName",
                table: "ProjectRepositories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "origin");

            migrationBuilder.AddColumn<int>(
                name: "UntrackedFileCount",
                table: "ProjectRepositories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "ProjectRepositories",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.CreateTable(
                name: "RepositoryBranches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectRepositoryId = table.Column<int>(type: "int", nullable: false),
                    BranchName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsRemote = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsCurrentBranch = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsTracking = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    TrackingBranchName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LastCommitSha = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    LastCommitMessage = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    LastCommitAuthor = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LastCommitDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepositoryBranches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RepositoryBranches_ProjectRepositories_ProjectRepositoryId",
                        column: x => x.ProjectRepositoryId,
                        principalTable: "ProjectRepositories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RepositoryBranches_IsCurrentBranch",
                table: "RepositoryBranches",
                columns: new[] { "ProjectRepositoryId", "IsCurrentBranch" },
                filter: "[IsCurrentBranch] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_RepositoryBranches_ProjectRepositoryId",
                table: "RepositoryBranches",
                column: "ProjectRepositoryId");

            migrationBuilder.CreateIndex(
                name: "UQ_RepositoryBranches_UniqueName",
                table: "RepositoryBranches",
                columns: new[] { "ProjectRepositoryId", "FullName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RepositoryBranches");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ProjectRepositories");

            migrationBuilder.DropColumn(
                name: "CurrentBranchName",
                table: "ProjectRepositories");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ProjectRepositories");

            migrationBuilder.DropColumn(
                name: "IsDirty",
                table: "ProjectRepositories");

            migrationBuilder.DropColumn(
                name: "LastFetchedAt",
                table: "ProjectRepositories");

            migrationBuilder.DropColumn(
                name: "ModifiedFileCount",
                table: "ProjectRepositories");

            migrationBuilder.DropColumn(
                name: "RemoteName",
                table: "ProjectRepositories");

            migrationBuilder.DropColumn(
                name: "UntrackedFileCount",
                table: "ProjectRepositories");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ProjectRepositories");

            migrationBuilder.RenameColumn(
                name: "LastSyncedAt",
                table: "ProjectRepositories",
                newName: "LastSyncDate");

            migrationBuilder.AddColumn<string>(
                name: "DefaultBranch",
                table: "ProjectRepositories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "main");
        }
    }
}
