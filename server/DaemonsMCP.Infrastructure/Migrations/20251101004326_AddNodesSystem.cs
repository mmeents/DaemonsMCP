using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DaemonsMCP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNodesSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ItemTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    Rank = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemTypes_ItemTypes_ParentId",
                        column: x => x.ParentId,
                        principalTable: "ItemTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    ItemTypeId = table.Column<int>(type: "int", nullable: false),
                    StatusTypeId = table.Column<int>(type: "int", nullable: false),
                    Rank = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Completed = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReferenceFileSystemId = table.Column<int>(type: "int", nullable: true),
                    ReferenceObjectHierarchyId = table.Column<int>(type: "int", nullable: true),
                    ItemTypeId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Items_FileSystemNodes_ReferenceFileSystemId",
                        column: x => x.ReferenceFileSystemId,
                        principalTable: "FileSystemNodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Items_ItemTypes_ItemTypeId",
                        column: x => x.ItemTypeId,
                        principalTable: "ItemTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Items_ItemTypes_ItemTypeId1",
                        column: x => x.ItemTypeId1,
                        principalTable: "ItemTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Items_ItemTypes_StatusTypeId",
                        column: x => x.StatusTypeId,
                        principalTable: "ItemTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Items_Items_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Items_ObjectHierarchies_ReferenceObjectHierarchyId",
                        column: x => x.ReferenceObjectHierarchyId,
                        principalTable: "ObjectHierarchies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "ItemTypes",
                columns: new[] { "Id", "Description", "Name", "ParentId", "Rank" },
                values: new object[,]
                {
                    { 1, "Reserved null type - default unset value", "None", null, 0 },
                    { 2, "Internal root - high-level grouping for type categories", "Categories", null, 0 },
                    { 3, "Parent container for all item type definitions", "ItemTypes", 2, 1 },
                    { 4, "Parent container for all status type definitions", "StatusTypes", 2, 2 },
                    { 5, "A task or action item to be completed", "Todo", 3, 1 },
                    { 6, "Documentation or informational content", "Readme", 3, 2 },
                    { 7, "General note or observation", "Note", 3, 3 },
                    { 10, "Item has not been started yet", "Not Started", 4, 1 },
                    { 11, "Item is currently being worked on", "In Progress", 4, 2 },
                    { 12, "Item is finished", "Complete", 4, 3 },
                    { 13, "Item is paused or waiting", "On Hold", 4, 4 },
                    { 14, "Item was cancelled and will not be completed", "Cancelled", 4, 5 }
                });

            migrationBuilder.InsertData(
                table: "Items",
                columns: new[] { "Id", "Completed", "Created", "Details", "ItemTypeId", "ItemTypeId1", "Modified", "Name", "ParentId", "Rank", "ReferenceFileSystemId", "ReferenceObjectHierarchyId", "StatusTypeId" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Living documentation for the Daemons3MCP tool. This documentation is stored as hierarchical nodes and can be extended through the nodes interface. The system provides tools for file management, code indexing, and hierarchical note/task organization.", 6, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Daemons3MCP Documentation", null, 1, null, null, 12 },
                    { 2, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Root container for all todo lists. Todo lists are created as children of this node. Use make-todo-list to create new lists, and get-next-todo to retrieve the next actionable item.", 5, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Todo Root", null, 2, null, null, 11 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Items_Completed",
                table: "Items",
                column: "Completed");

            migrationBuilder.CreateIndex(
                name: "IX_Items_Created",
                table: "Items",
                column: "Created");

            migrationBuilder.CreateIndex(
                name: "IX_Items_ItemTypeId",
                table: "Items",
                column: "ItemTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_ItemTypeId_StatusTypeId",
                table: "Items",
                columns: new[] { "ItemTypeId", "StatusTypeId" });

            migrationBuilder.CreateIndex(
                name: "IX_Items_ItemTypeId1",
                table: "Items",
                column: "ItemTypeId1");

            migrationBuilder.CreateIndex(
                name: "IX_Items_ParentId",
                table: "Items",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_ParentId_Rank",
                table: "Items",
                columns: new[] { "ParentId", "Rank" });

            migrationBuilder.CreateIndex(
                name: "IX_Items_ReferenceFileSystemId",
                table: "Items",
                column: "ReferenceFileSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_ReferenceObjectHierarchyId",
                table: "Items",
                column: "ReferenceObjectHierarchyId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_StatusTypeId",
                table: "Items",
                column: "StatusTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemTypes_Name",
                table: "ItemTypes",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ItemTypes_ParentId",
                table: "ItemTypes",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemTypes_ParentId_Rank",
                table: "ItemTypes",
                columns: new[] { "ParentId", "Rank" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "ItemTypes");
        }
    }
}
