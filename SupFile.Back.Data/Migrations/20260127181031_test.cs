using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SupFile.Back.Data.Migrations
{
    /// <inheritdoc />
    public partial class test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Directory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    OwnerId = table.Column<int>(type: "integer", nullable: false),
                    ParentId = table.Column<int>(type: "integer", nullable: true),
                    OwnerApplicationUserDirectoryId = table.Column<int>(type: "integer", nullable: false),
                    ParentDirectoryId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Directory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Directory_ApplicationUsers_OwnerApplicationUserDirectoryId",
                        column: x => x.OwnerApplicationUserDirectoryId,
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Directory_Directory_ParentDirectoryId",
                        column: x => x.ParentDirectoryId,
                        principalTable: "Directory",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "File",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Extension = table.Column<string>(type: "text", nullable: false),
                    Size = table.Column<int>(type: "integer", nullable: false),
                    Path = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DirectoryId = table.Column<int>(type: "integer", nullable: false),
                    OwnerId = table.Column<int>(type: "integer", nullable: false),
                    OwnerApplicationUserFileId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_File", x => x.Id);
                    table.ForeignKey(
                        name: "FK_File_ApplicationUsers_OwnerApplicationUserFileId",
                        column: x => x.OwnerApplicationUserFileId,
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_File_Directory_DirectoryId",
                        column: x => x.DirectoryId,
                        principalTable: "Directory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Link",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Token = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ShareFileId = table.Column<int>(type: "integer", nullable: true),
                    ShareDirectoryId = table.Column<int>(type: "integer", nullable: true),
                    ShareLinkFileId = table.Column<int>(type: "integer", nullable: true),
                    ShareLinkDirectoryId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Link", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Link_Directory_ShareLinkDirectoryId",
                        column: x => x.ShareLinkDirectoryId,
                        principalTable: "Directory",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Link_File_ShareLinkFileId",
                        column: x => x.ShareLinkFileId,
                        principalTable: "File",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Share",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Permission = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    ShareFileId = table.Column<int>(type: "integer", nullable: true),
                    ShareDirectoryId = table.Column<int>(type: "integer", nullable: true),
                    ShareUserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Share", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Share_ApplicationUsers_ShareUserId",
                        column: x => x.ShareUserId,
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Share_Directory_ShareDirectoryId",
                        column: x => x.ShareDirectoryId,
                        principalTable: "Directory",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Share_File_ShareFileId",
                        column: x => x.ShareFileId,
                        principalTable: "File",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Directory_OwnerApplicationUserDirectoryId",
                table: "Directory",
                column: "OwnerApplicationUserDirectoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Directory_ParentDirectoryId",
                table: "Directory",
                column: "ParentDirectoryId");

            migrationBuilder.CreateIndex(
                name: "IX_File_DirectoryId",
                table: "File",
                column: "DirectoryId");

            migrationBuilder.CreateIndex(
                name: "IX_File_OwnerApplicationUserFileId",
                table: "File",
                column: "OwnerApplicationUserFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Link_ShareLinkDirectoryId",
                table: "Link",
                column: "ShareLinkDirectoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Link_ShareLinkFileId",
                table: "Link",
                column: "ShareLinkFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Share_ShareDirectoryId",
                table: "Share",
                column: "ShareDirectoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Share_ShareFileId",
                table: "Share",
                column: "ShareFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Share_ShareUserId",
                table: "Share",
                column: "ShareUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Link");

            migrationBuilder.DropTable(
                name: "Share");

            migrationBuilder.DropTable(
                name: "File");

            migrationBuilder.DropTable(
                name: "Directory");
        }
    }
}
