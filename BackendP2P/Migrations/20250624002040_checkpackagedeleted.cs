using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendP2P.Migrations
{
    /// <inheritdoc />
    public partial class checkpackagedeleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Tasks_TaskModelId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_CriteriaAssignments_Tasks_MaterialWorkModelId",
                table: "CriteriaAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_SolutionChecks_CheckPackages_CheckPackageId",
                table: "SolutionChecks");

            migrationBuilder.DropForeignKey(
                name: "FK_Solutions_Tasks_MaterialWorkModelId",
                table: "Solutions");

            migrationBuilder.DropForeignKey(
                name: "FK_Solutions_Tasks_TaskId",
                table: "Solutions");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_CheckPackages_CheckPackageId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Courses_CourseId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Users_AuthorId",
                table: "Tasks");

            migrationBuilder.DropTable(
                name: "CheckPackages");

            migrationBuilder.DropIndex(
                name: "IX_SolutionChecks_CheckPackageId",
                table: "SolutionChecks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tasks",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_CheckPackageId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "CheckPackageId",
                table: "SolutionChecks");

            migrationBuilder.DropColumn(
                name: "CheckPackageId",
                table: "Tasks");

            migrationBuilder.RenameTable(
                name: "Tasks",
                newName: "TaskModel");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_CourseId",
                table: "TaskModel",
                newName: "IX_TaskModel_CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_AuthorId",
                table: "TaskModel",
                newName: "IX_TaskModel_AuthorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskModel",
                table: "TaskModel",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_TaskModel_TaskModelId",
                table: "Comments",
                column: "TaskModelId",
                principalTable: "TaskModel",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CriteriaAssignments_TaskModel_MaterialWorkModelId",
                table: "CriteriaAssignments",
                column: "MaterialWorkModelId",
                principalTable: "TaskModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Solutions_TaskModel_MaterialWorkModelId",
                table: "Solutions",
                column: "MaterialWorkModelId",
                principalTable: "TaskModel",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Solutions_TaskModel_TaskId",
                table: "Solutions",
                column: "TaskId",
                principalTable: "TaskModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskModel_Courses_CourseId",
                table: "TaskModel",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskModel_Users_AuthorId",
                table: "TaskModel",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_TaskModel_TaskModelId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_CriteriaAssignments_TaskModel_MaterialWorkModelId",
                table: "CriteriaAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Solutions_TaskModel_MaterialWorkModelId",
                table: "Solutions");

            migrationBuilder.DropForeignKey(
                name: "FK_Solutions_TaskModel_TaskId",
                table: "Solutions");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskModel_Courses_CourseId",
                table: "TaskModel");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskModel_Users_AuthorId",
                table: "TaskModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskModel",
                table: "TaskModel");

            migrationBuilder.RenameTable(
                name: "TaskModel",
                newName: "Tasks");

            migrationBuilder.RenameIndex(
                name: "IX_TaskModel_CourseId",
                table: "Tasks",
                newName: "IX_Tasks_CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskModel_AuthorId",
                table: "Tasks",
                newName: "IX_Tasks_AuthorId");

            migrationBuilder.AddColumn<Guid>(
                name: "CheckPackageId",
                table: "SolutionChecks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CheckPackageId",
                table: "Tasks",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tasks",
                table: "Tasks",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "CheckPackages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Deadline = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Instructions = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckPackages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CheckPackages_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CheckPackages_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SolutionChecks_CheckPackageId",
                table: "SolutionChecks",
                column: "CheckPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_CheckPackageId",
                table: "Tasks",
                column: "CheckPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckPackages_TaskId",
                table: "CheckPackages",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckPackages_UserId",
                table: "CheckPackages",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Tasks_TaskModelId",
                table: "Comments",
                column: "TaskModelId",
                principalTable: "Tasks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CriteriaAssignments_Tasks_MaterialWorkModelId",
                table: "CriteriaAssignments",
                column: "MaterialWorkModelId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SolutionChecks_CheckPackages_CheckPackageId",
                table: "SolutionChecks",
                column: "CheckPackageId",
                principalTable: "CheckPackages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Solutions_Tasks_MaterialWorkModelId",
                table: "Solutions",
                column: "MaterialWorkModelId",
                principalTable: "Tasks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Solutions_Tasks_TaskId",
                table: "Solutions",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_CheckPackages_CheckPackageId",
                table: "Tasks",
                column: "CheckPackageId",
                principalTable: "CheckPackages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Courses_CourseId",
                table: "Tasks",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Users_AuthorId",
                table: "Tasks",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
