using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendP2P.Migrations
{
    /// <inheritdoc />
    public partial class fixingtaskmaterial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CriteriaAssignments_MaterialWorks_MaterialWorkModelId",
                table: "CriteriaAssignments");

            migrationBuilder.DropTable(
                name: "MaterialReads");

            migrationBuilder.DropTable(
                name: "MaterialWorks");

            migrationBuilder.DropIndex(
                name: "IX_CheckPackages_TaskId",
                table: "CheckPackages");

            migrationBuilder.DropColumn(
                name: "MaterialReadId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Students",
                table: "Tasks");

            migrationBuilder.RenameColumn(
                name: "MaterialWorkId",
                table: "Tasks",
                newName: "CheckPackageId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Deadline",
                table: "Tasks",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<int>(
                name: "Check",
                table: "Tasks",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "Tasks",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Tasks",
                type: "character varying(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Instructions",
                table: "Tasks",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Score",
                table: "Tasks",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MaterialWorkModelId",
                table: "Solutions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_CheckPackageId",
                table: "Tasks",
                column: "CheckPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_Solutions_MaterialWorkModelId",
                table: "Solutions",
                column: "MaterialWorkModelId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckPackages_TaskId",
                table: "CheckPackages",
                column: "TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_CriteriaAssignments_Tasks_MaterialWorkModelId",
                table: "CriteriaAssignments",
                column: "MaterialWorkModelId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Solutions_Tasks_MaterialWorkModelId",
                table: "Solutions",
                column: "MaterialWorkModelId",
                principalTable: "Tasks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_CheckPackages_CheckPackageId",
                table: "Tasks",
                column: "CheckPackageId",
                principalTable: "CheckPackages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CriteriaAssignments_Tasks_MaterialWorkModelId",
                table: "CriteriaAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Solutions_Tasks_MaterialWorkModelId",
                table: "Solutions");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_CheckPackages_CheckPackageId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_CheckPackageId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Solutions_MaterialWorkModelId",
                table: "Solutions");

            migrationBuilder.DropIndex(
                name: "IX_CheckPackages_TaskId",
                table: "CheckPackages");

            migrationBuilder.DropColumn(
                name: "Content",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Instructions",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "MaterialWorkModelId",
                table: "Solutions");

            migrationBuilder.RenameColumn(
                name: "CheckPackageId",
                table: "Tasks",
                newName: "MaterialWorkId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Deadline",
                table: "Tasks",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Check",
                table: "Tasks",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MaterialReadId",
                table: "Tasks",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<List<Guid>>(
                name: "Students",
                table: "Tasks",
                type: "uuid[]",
                nullable: false);

            migrationBuilder.CreateTable(
                name: "MaterialReads",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialReads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterialReads_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaterialWorks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    Deadline = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Instructions = table.Column<string>(type: "text", nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialWorks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterialWorks_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CheckPackages_TaskId",
                table: "CheckPackages",
                column: "TaskId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaterialReads_TaskId",
                table: "MaterialReads",
                column: "TaskId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaterialWorks_TaskId",
                table: "MaterialWorks",
                column: "TaskId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CriteriaAssignments_MaterialWorks_MaterialWorkModelId",
                table: "CriteriaAssignments",
                column: "MaterialWorkModelId",
                principalTable: "MaterialWorks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
