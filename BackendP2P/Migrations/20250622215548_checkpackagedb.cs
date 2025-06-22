using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendP2P.Migrations
{
    /// <inheritdoc />
    public partial class checkpackagedb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CheckPackage_Tasks_TaskId",
                table: "CheckPackage");

            migrationBuilder.DropForeignKey(
                name: "FK_CheckPackage_Users_UserId",
                table: "CheckPackage");

            migrationBuilder.DropForeignKey(
                name: "FK_SolutionChecks_CheckPackage_CheckPackageId",
                table: "SolutionChecks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CheckPackage",
                table: "CheckPackage");

            migrationBuilder.RenameTable(
                name: "CheckPackage",
                newName: "CheckPackages");

            migrationBuilder.RenameIndex(
                name: "IX_CheckPackage_UserId",
                table: "CheckPackages",
                newName: "IX_CheckPackages_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_CheckPackage_TaskId",
                table: "CheckPackages",
                newName: "IX_CheckPackages_TaskId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CheckPackages",
                table: "CheckPackages",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CheckPackages_Tasks_TaskId",
                table: "CheckPackages",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CheckPackages_Users_UserId",
                table: "CheckPackages",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SolutionChecks_CheckPackages_CheckPackageId",
                table: "SolutionChecks",
                column: "CheckPackageId",
                principalTable: "CheckPackages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CheckPackages_Tasks_TaskId",
                table: "CheckPackages");

            migrationBuilder.DropForeignKey(
                name: "FK_CheckPackages_Users_UserId",
                table: "CheckPackages");

            migrationBuilder.DropForeignKey(
                name: "FK_SolutionChecks_CheckPackages_CheckPackageId",
                table: "SolutionChecks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CheckPackages",
                table: "CheckPackages");

            migrationBuilder.RenameTable(
                name: "CheckPackages",
                newName: "CheckPackage");

            migrationBuilder.RenameIndex(
                name: "IX_CheckPackages_UserId",
                table: "CheckPackage",
                newName: "IX_CheckPackage_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_CheckPackages_TaskId",
                table: "CheckPackage",
                newName: "IX_CheckPackage_TaskId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CheckPackage",
                table: "CheckPackage",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CheckPackage_Tasks_TaskId",
                table: "CheckPackage",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CheckPackage_Users_UserId",
                table: "CheckPackage",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SolutionChecks_CheckPackage_CheckPackageId",
                table: "SolutionChecks",
                column: "CheckPackageId",
                principalTable: "CheckPackage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
