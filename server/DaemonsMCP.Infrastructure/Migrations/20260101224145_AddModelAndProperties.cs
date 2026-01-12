using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DaemonsMCP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddModelAndProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Models",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    ModelTypeId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Rank = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", maxLength: -1, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Models", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Models_ModelTypes_ModelTypeId",
                        column: x => x.ModelTypeId,
                        principalTable: "ModelTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Models_Models_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Models_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ModelProperties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModelId = table.Column<int>(type: "int", nullable: false),
                    PropertyKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PropertyValue = table.Column<string>(type: "nvarchar(max)", maxLength: -1, nullable: true),
                    PropertyModelTypeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModelProperties_ModelTypes_PropertyModelTypeId",
                        column: x => x.PropertyModelTypeId,
                        principalTable: "ModelTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ModelProperties_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModelProperties_ModelId",
                table: "ModelProperties",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_ModelProperties_ModelId_PropertyKey",
                table: "ModelProperties",
                columns: new[] { "ModelId", "PropertyKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ModelProperties_PropertyKey",
                table: "ModelProperties",
                column: "PropertyKey");

            migrationBuilder.CreateIndex(
                name: "IX_ModelProperties_PropertyModelTypeId",
                table: "ModelProperties",
                column: "PropertyValueTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Models_CreatedDate",
                table: "Models",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Models_ModelTypeId",
                table: "Models",
                column: "ModelTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Models_ParentId",
                table: "Models",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Models_ProjectId",
                table: "Models",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Models_ProjectId_ParentId_Rank",
                table: "Models",
                columns: new[] { "ProjectId", "ParentId", "Rank" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModelProperties");

            migrationBuilder.DropTable(
                name: "Models");
        }
    }
}
