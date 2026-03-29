using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupFile.Back.Data.Migrations
{
    /// <inheritdoc />
    public partial class DeletePathInMedia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Path",
                schema: "public",
                table: "Media");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Path",
                schema: "public",
                table: "Media",
                type: "varchar(1024)",
                maxLength: 1024,
                nullable: false,
                defaultValue: "");
        }
    }
}
