using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendP2P.Migrations
{
    /// <inheritdoc />
    public partial class filetotask1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AttachedFiles_TaskModelId",
                table: "AttachedFiles");

            migrationBuilder.DropColumn(
                name: "TaskModelId",
                table: "AttachedFiles");

            migrationBuilder.AddColumn<Guid>(
                name: "ReadModelId",
                table: "AttachedFiles",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "WorkModelId",
                table: "AttachedFiles",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_AttachedFiles_ReadModelId",
                table: "AttachedFiles",
                column: "ReadModelId");

            migrationBuilder.CreateIndex(
                name: "IX_AttachedFiles_WorkModelId",
                table: "AttachedFiles",
                column: "WorkModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_AttachedFiles_MaterialReads_ReadModelId",
                table: "AttachedFiles",
                column: "ReadModelId",
                principalTable: "MaterialReads",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AttachedFiles_MaterialWorks_WorkModelId",
                table: "AttachedFiles",
                column: "WorkModelId",
                principalTable: "MaterialWorks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttachedFiles_MaterialReads_ReadModelId",
                table: "AttachedFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_AttachedFiles_MaterialWorks_WorkModelId",
                table: "AttachedFiles");

            migrationBuilder.DropIndex(
                name: "IX_AttachedFiles_ReadModelId",
                table: "AttachedFiles");

            migrationBuilder.DropIndex(
                name: "IX_AttachedFiles_WorkModelId",
                table: "AttachedFiles");

            migrationBuilder.DropColumn(
                name: "ReadModelId",
                table: "AttachedFiles");

            migrationBuilder.DropColumn(
                name: "WorkModelId",
                table: "AttachedFiles");

            migrationBuilder.AddColumn<Guid>(
                name: "TaskModelId",
                table: "AttachedFiles",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AttachedFiles_TaskModelId",
                table: "AttachedFiles",
                column: "TaskModelId");
        }
    }
}
