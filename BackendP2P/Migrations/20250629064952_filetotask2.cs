using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendP2P.Migrations
{
    /// <inheritdoc />
    public partial class filetotask2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttachedFiles_MaterialReads_ReadModelId",
                table: "AttachedFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_AttachedFiles_MaterialWorks_WorkModelId",
                table: "AttachedFiles");

            migrationBuilder.AlterColumn<Guid>(
                name: "WorkModelId",
                table: "AttachedFiles",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "ReadModelId",
                table: "AttachedFiles",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_AttachedFiles_MaterialReads_ReadModelId",
                table: "AttachedFiles",
                column: "ReadModelId",
                principalTable: "MaterialReads",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AttachedFiles_MaterialWorks_WorkModelId",
                table: "AttachedFiles",
                column: "WorkModelId",
                principalTable: "MaterialWorks",
                principalColumn: "Id");
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

            migrationBuilder.AlterColumn<Guid>(
                name: "WorkModelId",
                table: "AttachedFiles",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ReadModelId",
                table: "AttachedFiles",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

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
    }
}
