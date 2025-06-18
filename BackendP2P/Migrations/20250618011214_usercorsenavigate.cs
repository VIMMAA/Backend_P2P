using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendP2P.Migrations
{
    /// <inheritdoc />
    public partial class usercorsenavigate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UsersCorses_CourseId",
                table: "UsersCorses",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersCorses_UserId",
                table: "UsersCorses",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UsersCorses_Courses_CourseId",
                table: "UsersCorses",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersCorses_Users_UserId",
                table: "UsersCorses",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersCorses_Courses_CourseId",
                table: "UsersCorses");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersCorses_Users_UserId",
                table: "UsersCorses");

            migrationBuilder.DropIndex(
                name: "IX_UsersCorses_CourseId",
                table: "UsersCorses");

            migrationBuilder.DropIndex(
                name: "IX_UsersCorses_UserId",
                table: "UsersCorses");
        }
    }
}
