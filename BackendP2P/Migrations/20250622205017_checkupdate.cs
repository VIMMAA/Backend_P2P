using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendP2P.Migrations
{
    /// <inheritdoc />
    public partial class checkupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Grades_SolutionChecks_SolutionCheckId",
                table: "Grades");

            migrationBuilder.DropForeignKey(
                name: "FK_Grades_Tasks_TaskId",
                table: "Grades");

            migrationBuilder.DropForeignKey(
                name: "FK_Remarks_SolutionChecks_SolutionCheckId",
                table: "Remarks");

            migrationBuilder.DropIndex(
                name: "IX_Remarks_SolutionCheckId",
                table: "Remarks");

            migrationBuilder.DropIndex(
                name: "IX_Grades_SolutionCheckId",
                table: "Grades");

            migrationBuilder.DropIndex(
                name: "IX_Grades_TaskId",
                table: "Grades");

            migrationBuilder.DropColumn(
                name: "SolutionCheckId",
                table: "Remarks");

            migrationBuilder.DropColumn(
                name: "SolutionCheckId",
                table: "Grades");

            migrationBuilder.DropColumn(
                name: "TaskId",
                table: "Grades");

            migrationBuilder.AddColumn<Guid>(
                name: "CheckPackageId",
                table: "Solutions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CheckPackageId",
                table: "SolutionChecks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "GradeId",
                table: "SolutionChecks",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RemarkId",
                table: "SolutionChecks",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "GradeModelId",
                table: "CriteriaAssignments",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CheckPackage",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Deadline = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Instructions = table.Column<string>(type: "text", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckPackage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CheckPackage_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CheckPackage_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Solutions_CheckPackageId",
                table: "Solutions",
                column: "CheckPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_SolutionChecks_CheckPackageId",
                table: "SolutionChecks",
                column: "CheckPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_SolutionChecks_GradeId",
                table: "SolutionChecks",
                column: "GradeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SolutionChecks_RemarkId",
                table: "SolutionChecks",
                column: "RemarkId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CriteriaAssignments_GradeModelId",
                table: "CriteriaAssignments",
                column: "GradeModelId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckPackage_TaskId",
                table: "CheckPackage",
                column: "TaskId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CheckPackage_UserId",
                table: "CheckPackage",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CriteriaAssignments_Grades_GradeModelId",
                table: "CriteriaAssignments",
                column: "GradeModelId",
                principalTable: "Grades",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SolutionChecks_CheckPackage_CheckPackageId",
                table: "SolutionChecks",
                column: "CheckPackageId",
                principalTable: "CheckPackage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SolutionChecks_Grades_GradeId",
                table: "SolutionChecks",
                column: "GradeId",
                principalTable: "Grades",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SolutionChecks_Remarks_RemarkId",
                table: "SolutionChecks",
                column: "RemarkId",
                principalTable: "Remarks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Solutions_CheckPackage_CheckPackageId",
                table: "Solutions",
                column: "CheckPackageId",
                principalTable: "CheckPackage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CriteriaAssignments_Grades_GradeModelId",
                table: "CriteriaAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_SolutionChecks_CheckPackage_CheckPackageId",
                table: "SolutionChecks");

            migrationBuilder.DropForeignKey(
                name: "FK_SolutionChecks_Grades_GradeId",
                table: "SolutionChecks");

            migrationBuilder.DropForeignKey(
                name: "FK_SolutionChecks_Remarks_RemarkId",
                table: "SolutionChecks");

            migrationBuilder.DropForeignKey(
                name: "FK_Solutions_CheckPackage_CheckPackageId",
                table: "Solutions");

            migrationBuilder.DropTable(
                name: "CheckPackage");

            migrationBuilder.DropIndex(
                name: "IX_Solutions_CheckPackageId",
                table: "Solutions");

            migrationBuilder.DropIndex(
                name: "IX_SolutionChecks_CheckPackageId",
                table: "SolutionChecks");

            migrationBuilder.DropIndex(
                name: "IX_SolutionChecks_GradeId",
                table: "SolutionChecks");

            migrationBuilder.DropIndex(
                name: "IX_SolutionChecks_RemarkId",
                table: "SolutionChecks");

            migrationBuilder.DropIndex(
                name: "IX_CriteriaAssignments_GradeModelId",
                table: "CriteriaAssignments");

            migrationBuilder.DropColumn(
                name: "CheckPackageId",
                table: "Solutions");

            migrationBuilder.DropColumn(
                name: "CheckPackageId",
                table: "SolutionChecks");

            migrationBuilder.DropColumn(
                name: "GradeId",
                table: "SolutionChecks");

            migrationBuilder.DropColumn(
                name: "RemarkId",
                table: "SolutionChecks");

            migrationBuilder.DropColumn(
                name: "GradeModelId",
                table: "CriteriaAssignments");

            migrationBuilder.AddColumn<Guid>(
                name: "SolutionCheckId",
                table: "Remarks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "SolutionCheckId",
                table: "Grades",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TaskId",
                table: "Grades",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Remarks_SolutionCheckId",
                table: "Remarks",
                column: "SolutionCheckId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Grades_SolutionCheckId",
                table: "Grades",
                column: "SolutionCheckId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Grades_TaskId",
                table: "Grades",
                column: "TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_Grades_SolutionChecks_SolutionCheckId",
                table: "Grades",
                column: "SolutionCheckId",
                principalTable: "SolutionChecks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Grades_Tasks_TaskId",
                table: "Grades",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Remarks_SolutionChecks_SolutionCheckId",
                table: "Remarks",
                column: "SolutionCheckId",
                principalTable: "SolutionChecks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
