using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendP2P.Migrations
{
    /// <inheritdoc />
    public partial class userlistisguidlist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Tasks_TaskModelId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_TaskModelId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TaskModelId",
                table: "Users");

            migrationBuilder.AddColumn<List<Guid>>(
                name: "Students",
                table: "Tasks",
                type: "uuid[]",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Students",
                table: "Tasks");

            migrationBuilder.AddColumn<Guid>(
                name: "TaskModelId",
                table: "Users",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_TaskModelId",
                table: "Users",
                column: "TaskModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Tasks_TaskModelId",
                table: "Users",
                column: "TaskModelId",
                principalTable: "Tasks",
                principalColumn: "Id");
        }
    }
}
