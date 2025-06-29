using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendP2P.Migrations
{
    /// <inheritdoc />
    public partial class filetotask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttachedFiles_Solutions_SolutionId",
                table: "AttachedFiles");

            migrationBuilder.AlterColumn<Guid>(
                name: "SolutionId",
                table: "AttachedFiles",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "MaterialReadId",
                table: "AttachedFiles",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MaterialWorkId",
                table: "AttachedFiles",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TaskModelId",
                table: "AttachedFiles",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AttachedFiles_TaskModelId",
                table: "AttachedFiles",
                column: "TaskModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_AttachedFiles_Solutions_SolutionId",
                table: "AttachedFiles",
                column: "SolutionId",
                principalTable: "Solutions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttachedFiles_Solutions_SolutionId",
                table: "AttachedFiles");

            migrationBuilder.DropIndex(
                name: "IX_AttachedFiles_TaskModelId",
                table: "AttachedFiles");

            migrationBuilder.DropColumn(
                name: "MaterialReadId",
                table: "AttachedFiles");

            migrationBuilder.DropColumn(
                name: "MaterialWorkId",
                table: "AttachedFiles");

            migrationBuilder.DropColumn(
                name: "TaskModelId",
                table: "AttachedFiles");

            migrationBuilder.AlterColumn<Guid>(
                name: "SolutionId",
                table: "AttachedFiles",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AttachedFiles_Solutions_SolutionId",
                table: "AttachedFiles",
                column: "SolutionId",
                principalTable: "Solutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
