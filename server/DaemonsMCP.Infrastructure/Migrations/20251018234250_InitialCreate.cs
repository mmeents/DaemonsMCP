using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DaemonsMCP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Identifiers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Identifiers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IdentifierTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentifierTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    RootPath = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FileSystemNodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    RelativePath = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    IsDirectory = table.Column<bool>(type: "bit", nullable: false),
                    SizeInBytes = table.Column<long>(type: "bigint", nullable: true),
                    Extension = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IndexedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileSystemNodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileSystemNodes_FileSystemNodes_ParentId",
                        column: x => x.ParentId,
                        principalTable: "FileSystemNodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FileSystemNodes_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IndexQueues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    FileSystemNodeId = table.Column<int>(type: "int", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndexQueues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IndexQueues_FileSystemNodes_FileSystemNodeId",
                        column: x => x.FileSystemNodeId,
                        principalTable: "FileSystemNodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IndexQueues_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ObjectHierarchies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentId = table.Column<int>(type: "int", nullable: false),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    FileSystemNodeId = table.Column<int>(type: "int", nullable: false),
                    IdentifierId = table.Column<int>(type: "int", nullable: false),
                    IdentifierTypeId = table.Column<int>(type: "int", nullable: false),
                    LineStart = table.Column<int>(type: "int", nullable: false),
                    LineEnd = table.Column<int>(type: "int", nullable: false),
                    IndexedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectHierarchies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjectHierarchies_FileSystemNodes_FileSystemNodeId",
                        column: x => x.FileSystemNodeId,
                        principalTable: "FileSystemNodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ObjectHierarchies_IdentifierTypes_IdentifierTypeId",
                        column: x => x.IdentifierTypeId,
                        principalTable: "IdentifierTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ObjectHierarchies_Identifiers_IdentifierId",
                        column: x => x.IdentifierId,
                        principalTable: "Identifiers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ObjectHierarchies_ObjectHierarchies_ParentId",
                        column: x => x.ParentId,
                        principalTable: "ObjectHierarchies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ObjectHierarchies_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "IdentifierTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "namespace" },
                    { 2, "interface" },
                    { 3, "class" },
                    { 4, "method" },
                    { 5, "property" },
                    { 6, "field" },
                    { 7, "event" },
                    { 8, "methodParameter" }
                });

            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "Id", "CreatedAt", "Description", "Key", "ModifiedAt", "Value" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Comma-separated list of folder names to exclude from sync", "FileSystem.BlockedFolders", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "bin,obj,node_modules,.git,.vs,.vscode,packages,Debug,Release,TestResults,.idea" },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Comma-separated list of file extensions to exclude from sync", "FileSystem.BlockedExtensions", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), ".dll,.exe,.pdb,.cache,.suo,.user,.tmp,.temp,.log,.bak" },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "If set, only these extensions are synced (leave empty to allow all non-blocked)", "FileSystem.AllowedExtensions", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), ".cs,.csproj,.sln,.json,.xml,.md,.txt,.config,.yml,.yaml" },
                    { 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Comma-separated list of specific filenames to exclude", "FileSystem.BlockedFiles", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), ".DS_Store,Thumbs.db,desktop.ini" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileSystemNodes_ParentId",
                table: "FileSystemNodes",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_FileSystemNodes_ProjectId",
                table: "FileSystemNodes",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_FileSystemNodes_ProjectId_ParentId",
                table: "FileSystemNodes",
                columns: new[] { "ProjectId", "ParentId" });

            migrationBuilder.CreateIndex(
                name: "IX_FileSystemNodes_ProjectId_RelativePath",
                table: "FileSystemNodes",
                columns: new[] { "ProjectId", "RelativePath" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Identifiers_Name",
                table: "Identifiers",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IdentifierTypes_Name",
                table: "IdentifierTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IndexQueues_FileSystemNodeId",
                table: "IndexQueues",
                column: "FileSystemNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_IndexQueues_ProjectId",
                table: "IndexQueues",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_IndexQueues_Status",
                table: "IndexQueues",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_IndexQueues_Status_CreatedAt",
                table: "IndexQueues",
                columns: new[] { "Status", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ObjectHierarchies_FileSystemNodeId",
                table: "ObjectHierarchies",
                column: "FileSystemNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectHierarchies_IdentifierId",
                table: "ObjectHierarchies",
                column: "IdentifierId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectHierarchies_IdentifierTypeId",
                table: "ObjectHierarchies",
                column: "IdentifierTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectHierarchies_ParentId",
                table: "ObjectHierarchies",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectHierarchies_ProjectId",
                table: "ObjectHierarchies",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectHierarchies_ProjectId_ParentId",
                table: "ObjectHierarchies",
                columns: new[] { "ProjectId", "ParentId" });

            migrationBuilder.CreateIndex(
                name: "IX_Settings_Key",
                table: "Settings",
                column: "Key",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IndexQueues");

            migrationBuilder.DropTable(
                name: "ObjectHierarchies");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "FileSystemNodes");

            migrationBuilder.DropTable(
                name: "IdentifierTypes");

            migrationBuilder.DropTable(
                name: "Identifiers");

            migrationBuilder.DropTable(
                name: "Projects");
        }
    }
}
