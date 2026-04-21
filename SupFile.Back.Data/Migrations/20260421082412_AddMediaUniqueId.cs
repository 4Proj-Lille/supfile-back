using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupFile.Back.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMediaUniqueId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UniqueId",
                schema: "public",
                table: "Media",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UniqueId",
                schema: "public",
                table: "Media");
        }
    }
}
