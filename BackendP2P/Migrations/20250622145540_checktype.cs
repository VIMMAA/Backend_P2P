using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendP2P.Migrations
{
    /// <inheritdoc />
    public partial class checktype : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Check",
                table: "Tasks",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Check",
                table: "Tasks");
        }
    }
}
