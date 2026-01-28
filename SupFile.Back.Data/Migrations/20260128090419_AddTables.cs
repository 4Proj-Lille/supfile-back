using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SupFile.Back.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUsers_AspNetUsers_IdentityUserId",
                table: "ApplicationUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationUsers",
                table: "ApplicationUsers");

            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.RenameTable(
                name: "ApplicationUsers",
                newName: "User",
                newSchema: "public");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationUsers_IdentityUserId",
                schema: "public",
                table: "User",
                newName: "IX_User_IdentityUserId");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                schema: "public",
                table: "User",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                schema: "public",
                table: "User",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Language",
                schema: "public",
                table: "User",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                schema: "public",
                table: "User",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                schema: "public",
                table: "User",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_User",
                schema: "public",
                table: "User",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Directory",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    OwnerId = table.Column<int>(type: "int", nullable: false),
                    ParentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Directory", x => x.Id);
                    table.ForeignKey(
                        name: "Directories_User_Id_fk",
                        column: x => x.OwnerId,
                        principalSchema: "public",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "DirectoryParent___fk",
                        column: x => x.ParentId,
                        principalSchema: "public",
                        principalTable: "Directory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "File",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    Extension = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    Path = table.Column<string>(type: "varchar(1024)", maxLength: 1024, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    DirectoryId = table.Column<int>(type: "int", nullable: false),
                    OwnerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_File", x => x.Id);
                    table.ForeignKey(
                        name: "Files_Directory_Id_fk",
                        column: x => x.DirectoryId,
                        principalSchema: "public",
                        principalTable: "Directory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "Files_User_Id_fk",
                        column: x => x.OwnerId,
                        principalSchema: "public",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Link",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ShareFileId = table.Column<int>(type: "int", nullable: false),
                    ShareDirectoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Link", x => x.Id);
                    table.ForeignKey(
                        name: "Link_Directory_Id_fk",
                        column: x => x.ShareDirectoryId,
                        principalSchema: "public",
                        principalTable: "Directory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "Link_File_Id_fk",
                        column: x => x.ShareFileId,
                        principalSchema: "public",
                        principalTable: "File",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Share",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Permission = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    ShareFileId = table.Column<int>(type: "int", nullable: true),
                    ShareDirectoryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Share", x => x.Id);
                    table.ForeignKey(
                        name: "Share_Directory_Id_fk",
                        column: x => x.ShareDirectoryId,
                        principalSchema: "public",
                        principalTable: "Directory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "Share_File_Id_fk",
                        column: x => x.ShareFileId,
                        principalSchema: "public",
                        principalTable: "File",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "Shares_User_Id_fk",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Directory_OwnerId",
                schema: "public",
                table: "Directory",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Directory_ParentId",
                schema: "public",
                table: "Directory",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_File_DirectoryId",
                schema: "public",
                table: "File",
                column: "DirectoryId");

            migrationBuilder.CreateIndex(
                name: "IX_File_OwnerId",
                schema: "public",
                table: "File",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Link_ShareDirectoryId",
                schema: "public",
                table: "Link",
                column: "ShareDirectoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Link_ShareFileId",
                schema: "public",
                table: "Link",
                column: "ShareFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Share_ShareDirectoryId",
                schema: "public",
                table: "Share",
                column: "ShareDirectoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Share_ShareFileId",
                schema: "public",
                table: "Share",
                column: "ShareFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Share_UserId",
                schema: "public",
                table: "Share",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_ApplicationUser",
                schema: "public",
                table: "User",
                column: "IdentityUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_ApplicationUser",
                schema: "public",
                table: "User");

            migrationBuilder.DropTable(
                name: "Link",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Share",
                schema: "public");

            migrationBuilder.DropTable(
                name: "File",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Directory",
                schema: "public");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User",
                schema: "public",
                table: "User");

            migrationBuilder.RenameTable(
                name: "User",
                schema: "public",
                newName: "ApplicationUsers");

            migrationBuilder.RenameIndex(
                name: "IX_User_IdentityUserId",
                table: "ApplicationUsers",
                newName: "IX_ApplicationUsers_IdentityUserId");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "ApplicationUsers",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "ApplicationUsers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Language",
                table: "ApplicationUsers",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "ApplicationUsers",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ApplicationUsers",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationUsers",
                table: "ApplicationUsers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUsers_AspNetUsers_IdentityUserId",
                table: "ApplicationUsers",
                column: "IdentityUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
