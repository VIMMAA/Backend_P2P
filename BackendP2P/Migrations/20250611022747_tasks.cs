using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendP2P.Migrations
{
    /// <inheritdoc />
    public partial class tasks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentModel_TaskModel_TaskModelId",
                table: "CommentModel");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskModel_Courses_CourseModelId",
                table: "TaskModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskModel",
                table: "TaskModel");

            migrationBuilder.RenameTable(
                name: "TaskModel",
                newName: "Tasks");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Tasks",
                newName: "Topic");

            migrationBuilder.RenameIndex(
                name: "IX_TaskModel_CourseModelId",
                table: "Tasks",
                newName: "IX_Tasks_CourseModelId");

            migrationBuilder.AddColumn<List<Guid>>(
                name: "Solution",
                table: "Tasks",
                type: "uuid[]",
                nullable: true);

            migrationBuilder.AddColumn<List<Guid>>(
                name: "StudentGroup",
                table: "Tasks",
                type: "uuid[]",
                nullable: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tasks",
                table: "Tasks",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentModel_Tasks_TaskModelId",
                table: "CommentModel",
                column: "TaskModelId",
                principalTable: "Tasks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Courses_CourseModelId",
                table: "Tasks",
                column: "CourseModelId",
                principalTable: "Courses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentModel_Tasks_TaskModelId",
                table: "CommentModel");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Courses_CourseModelId",
                table: "Tasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tasks",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Solution",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "StudentGroup",
                table: "Tasks");

            migrationBuilder.RenameTable(
                name: "Tasks",
                newName: "TaskModel");

            migrationBuilder.RenameColumn(
                name: "Topic",
                table: "TaskModel",
                newName: "Description");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_CourseModelId",
                table: "TaskModel",
                newName: "IX_TaskModel_CourseModelId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskModel",
                table: "TaskModel",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentModel_TaskModel_TaskModelId",
                table: "CommentModel",
                column: "TaskModelId",
                principalTable: "TaskModel",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskModel_Courses_CourseModelId",
                table: "TaskModel",
                column: "CourseModelId",
                principalTable: "Courses",
                principalColumn: "Id");
        }
    }
}
