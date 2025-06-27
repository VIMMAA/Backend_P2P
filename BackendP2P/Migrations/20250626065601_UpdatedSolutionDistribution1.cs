using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendP2P.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedSolutionDistribution1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assessments_Users_TeacherId",
                table: "Assessments");

            migrationBuilder.RenameColumn(
                name: "TeacherId",
                table: "Assessments",
                newName: "SolutionForCheckModelId");

            migrationBuilder.RenameIndex(
                name: "IX_Assessments_TeacherId",
                table: "Assessments",
                newName: "IX_Assessments_SolutionForCheckModelId");

            migrationBuilder.AddColumn<bool>(
                name: "SolutionsDistributed",
                table: "TaskModel",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SolutionsToCheckN",
                table: "TaskModel",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxScore",
                table: "Assessments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Remark",
                table: "Assessments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "PackageChecks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    Deadline = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageChecks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SolutionForChecks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SolutionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: false),
                    AuthortId = table.Column<Guid>(type: "uuid", nullable: false),
                    DueTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsChecked = table.Column<bool>(type: "boolean", nullable: false),
                    PackageCheckModelId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolutionForChecks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolutionForChecks_PackageChecks_PackageCheckModelId",
                        column: x => x.PackageCheckModelId,
                        principalTable: "PackageChecks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SolutionForChecks_PackageCheckModelId",
                table: "SolutionForChecks",
                column: "PackageCheckModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assessments_SolutionForChecks_SolutionForCheckModelId",
                table: "Assessments",
                column: "SolutionForCheckModelId",
                principalTable: "SolutionForChecks",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assessments_SolutionForChecks_SolutionForCheckModelId",
                table: "Assessments");

            migrationBuilder.DropTable(
                name: "SolutionForChecks");

            migrationBuilder.DropTable(
                name: "PackageChecks");

            migrationBuilder.DropColumn(
                name: "SolutionsDistributed",
                table: "TaskModel");

            migrationBuilder.DropColumn(
                name: "SolutionsToCheckN",
                table: "TaskModel");

            migrationBuilder.DropColumn(
                name: "MaxScore",
                table: "Assessments");

            migrationBuilder.DropColumn(
                name: "Remark",
                table: "Assessments");

            migrationBuilder.RenameColumn(
                name: "SolutionForCheckModelId",
                table: "Assessments",
                newName: "TeacherId");

            migrationBuilder.RenameIndex(
                name: "IX_Assessments_SolutionForCheckModelId",
                table: "Assessments",
                newName: "IX_Assessments_TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assessments_Users_TeacherId",
                table: "Assessments",
                column: "TeacherId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
