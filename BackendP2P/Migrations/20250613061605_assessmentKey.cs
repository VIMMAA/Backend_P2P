using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendP2P.Migrations
{
    /// <inheritdoc />
    public partial class assessmentKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Assessments_StudentId",
                table: "Assessments",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Assessments_TeacherId",
                table: "Assessments",
                column: "TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assessments_Users_StudentId",
                table: "Assessments",
                column: "StudentId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Assessments_Users_TeacherId",
                table: "Assessments",
                column: "TeacherId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assessments_Users_StudentId",
                table: "Assessments");

            migrationBuilder.DropForeignKey(
                name: "FK_Assessments_Users_TeacherId",
                table: "Assessments");

            migrationBuilder.DropIndex(
                name: "IX_Assessments_StudentId",
                table: "Assessments");

            migrationBuilder.DropIndex(
                name: "IX_Assessments_TeacherId",
                table: "Assessments");
        }
    }
}
