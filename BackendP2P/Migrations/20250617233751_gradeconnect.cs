using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendP2P.Migrations
{
    /// <inheritdoc />
    public partial class gradeconnect : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Grades_Tasks_TaskModelId",
                table: "Grades");

            migrationBuilder.DropIndex(
                name: "IX_Grades_TaskModelId",
                table: "Grades");

            migrationBuilder.DropColumn(
                name: "TaskModelId",
                table: "Grades");

            migrationBuilder.AddColumn<Guid>(
                name: "TaskId",
                table: "Grades",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Grades_TaskId",
                table: "Grades",
                column: "TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_Grades_Tasks_TaskId",
                table: "Grades",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Grades_Tasks_TaskId",
                table: "Grades");

            migrationBuilder.DropIndex(
                name: "IX_Grades_TaskId",
                table: "Grades");

            migrationBuilder.DropColumn(
                name: "TaskId",
                table: "Grades");

            migrationBuilder.AddColumn<Guid>(
                name: "TaskModelId",
                table: "Grades",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Grades_TaskModelId",
                table: "Grades",
                column: "TaskModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Grades_Tasks_TaskModelId",
                table: "Grades",
                column: "TaskModelId",
                principalTable: "Tasks",
                principalColumn: "Id");
        }
    }
}
