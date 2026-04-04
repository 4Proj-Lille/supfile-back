using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupFile.Back.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsActiveForFolder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "public",
                table: "Folder",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "public",
                table: "Folder");
        }
    }
}
