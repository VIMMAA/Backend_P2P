using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendP2P.Migrations
{
    /// <inheritdoc />
    public partial class materialid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MaterialReads_TaskId",
                table: "MaterialReads");

            migrationBuilder.AddColumn<Guid>(
                name: "MaterialReadId",
                table: "Tasks",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MaterialWorkId",
                table: "Tasks",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaterialReads_TaskId",
                table: "MaterialReads",
                column: "TaskId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MaterialReads_TaskId",
                table: "MaterialReads");

            migrationBuilder.DropColumn(
                name: "MaterialReadId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "MaterialWorkId",
                table: "Tasks");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialReads_TaskId",
                table: "MaterialReads",
                column: "TaskId");
        }
    }
}
