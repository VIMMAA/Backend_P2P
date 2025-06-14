using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendP2P.Migrations
{
    /// <inheritdoc />
    public partial class Materials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MaterialReads",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialReads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterialReads_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaterialWorks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Score = table.Column<string>(type: "text", nullable: false),
                    Deadline = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Instructions = table.Column<string>(type: "text", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialWorks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterialWorks_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CriteriaAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Conditions = table.Column<string>(type: "text", nullable: false),
                    CountScore = table.Column<string>(type: "text", nullable: false),
                    Level = table.Column<string>(type: "text", nullable: false),
                    MaterialWorkModelId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CriteriaAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CriteriaAssignments_MaterialWorks_MaterialWorkModelId",
                        column: x => x.MaterialWorkModelId,
                        principalTable: "MaterialWorks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CriteriaAssignments_MaterialWorkModelId",
                table: "CriteriaAssignments",
                column: "MaterialWorkModelId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialReads_TaskId",
                table: "MaterialReads",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialWorks_TaskId",
                table: "MaterialWorks",
                column: "TaskId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CriteriaAssignments");

            migrationBuilder.DropTable(
                name: "MaterialReads");

            migrationBuilder.DropTable(
                name: "MaterialWorks");
        }
    }
}
