using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendP2P.Migrations
{
    /// <inheritdoc />
    public partial class some : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SolutionId",
                table: "Reports",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Reports_SolutionId",
                table: "Reports",
                column: "SolutionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Solutions_SolutionId",
                table: "Reports",
                column: "SolutionId",
                principalTable: "Solutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Solutions_SolutionId",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Reports_SolutionId",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "SolutionId",
                table: "Reports");
        }
    }
}
