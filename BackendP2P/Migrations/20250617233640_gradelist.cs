using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendP2P.Migrations
{
    /// <inheritdoc />
    public partial class gradelist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Grades_GradeId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_GradeId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "GradeId",
                table: "Tasks");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "GradeId",
                table: "Tasks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_GradeId",
                table: "Tasks",
                column: "GradeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Grades_GradeId",
                table: "Tasks",
                column: "GradeId",
                principalTable: "Grades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
