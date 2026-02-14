using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupFile.Back.Data.Migrations
{
    /// <inheritdoc />
    public partial class LinkToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                schema: "public",
                table: "Link",
                newName: "Token");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Token",
                schema: "public",
                table: "Link",
                newName: "Name");
        }
    }
}
