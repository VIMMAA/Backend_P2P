using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendP2P.Migrations
{
    /// <inheritdoc />
    public partial class TaskCourseKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Courses_CourseModelId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_CourseModelId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "CourseModelId",
                table: "Tasks");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_CourseId",
                table: "Tasks",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Courses_CourseId",
                table: "Tasks",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Courses_CourseId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_CourseId",
                table: "Tasks");

            migrationBuilder.AddColumn<Guid>(
                name: "CourseModelId",
                table: "Tasks",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_CourseModelId",
                table: "Tasks",
                column: "CourseModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Courses_CourseModelId",
                table: "Tasks",
                column: "CourseModelId",
                principalTable: "Courses",
                principalColumn: "Id");
        }
    }
}
