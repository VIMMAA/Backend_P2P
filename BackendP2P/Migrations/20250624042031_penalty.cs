using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendP2P.Migrations
{
    /// <inheritdoc />
    public partial class penalty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Penalty",
                table: "TaskModel",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remark",
                table: "Grades",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Penalty",
                table: "TaskModel");

            migrationBuilder.DropColumn(
                name: "Remark",
                table: "Grades");
        }
    }
}
