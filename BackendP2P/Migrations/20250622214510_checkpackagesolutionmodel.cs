using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendP2P.Migrations
{
    /// <inheritdoc />
    public partial class checkpackagesolutionmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Solutions_CheckPackage_CheckPackageId",
                table: "Solutions");

            migrationBuilder.DropIndex(
                name: "IX_Solutions_CheckPackageId",
                table: "Solutions");

            migrationBuilder.DropColumn(
                name: "CheckPackageId",
                table: "Solutions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CheckPackageId",
                table: "Solutions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Solutions_CheckPackageId",
                table: "Solutions",
                column: "CheckPackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Solutions_CheckPackage_CheckPackageId",
                table: "Solutions",
                column: "CheckPackageId",
                principalTable: "CheckPackage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
