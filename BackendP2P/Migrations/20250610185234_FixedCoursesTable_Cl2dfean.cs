using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendP2P.Migrations
{
    /// <inheritdoc />
    public partial class FixedCoursesTable_Cl2dfean : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Courses_CourseModelId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_CourseModelId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CourseModelId",
                table: "Users");

            migrationBuilder.AlterColumn<int>(
                name: "Role",
                table: "UsersCorses",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Role",
                table: "UsersCorses",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CourseModelId",
                table: "Users",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_CourseModelId",
                table: "Users",
                column: "CourseModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Courses_CourseModelId",
                table: "Users",
                column: "CourseModelId",
                principalTable: "Courses",
                principalColumn: "Id");
        }
    }
}
