using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendP2P.Migrations
{
    /// <inheritdoc />
    public partial class description : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Instructions",
                table: "MaterialWorks");

            migrationBuilder.DropColumn(
                name: "Content",
                table: "MaterialReads");

            migrationBuilder.RenameColumn(
                name: "Topic",
                table: "MaterialWorks",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "Topic",
                table: "MaterialReads",
                newName: "Description");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "MaterialWorks",
                newName: "Topic");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "MaterialReads",
                newName: "Topic");

            migrationBuilder.AddColumn<string>(
                name: "Instructions",
                table: "MaterialWorks",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "MaterialReads",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
