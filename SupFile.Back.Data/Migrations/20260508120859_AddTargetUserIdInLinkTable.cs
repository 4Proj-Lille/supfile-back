using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupFile.Back.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTargetUserIdInLinkTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TargetUserId",
                schema: "public",
                table: "Link",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Link_TargetUserId",
                schema: "public",
                table: "Link",
                column: "TargetUserId");

            migrationBuilder.AddForeignKey(
                name: "Link_TargetUser_Id_fk",
                schema: "public",
                table: "Link",
                column: "TargetUserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "Link_TargetUser_Id_fk",
                schema: "public",
                table: "Link");

            migrationBuilder.DropIndex(
                name: "IX_Link_TargetUserId",
                schema: "public",
                table: "Link");

            migrationBuilder.DropColumn(
                name: "TargetUserId",
                schema: "public",
                table: "Link");
        }
    }
}
