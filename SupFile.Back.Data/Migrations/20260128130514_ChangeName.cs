using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SupFile.Back.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "Link_Directory_Id_fk",
                schema: "public",
                table: "Link");

            migrationBuilder.DropForeignKey(
                name: "Link_File_Id_fk",
                schema: "public",
                table: "Link");

            migrationBuilder.DropForeignKey(
                name: "Share_Directory_Id_fk",
                schema: "public",
                table: "Share");

            migrationBuilder.DropForeignKey(
                name: "Share_File_Id_fk",
                schema: "public",
                table: "Share");

            migrationBuilder.DropTable(
                name: "File",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Directory",
                schema: "public");

            migrationBuilder.RenameColumn(
                name: "ShareFileId",
                schema: "public",
                table: "Share",
                newName: "ShareMediaId");

            migrationBuilder.RenameColumn(
                name: "ShareDirectoryId",
                schema: "public",
                table: "Share",
                newName: "ShareFolderId");

            migrationBuilder.RenameIndex(
                name: "IX_Share_ShareFileId",
                schema: "public",
                table: "Share",
                newName: "IX_Share_ShareMediaId");

            migrationBuilder.RenameIndex(
                name: "IX_Share_ShareDirectoryId",
                schema: "public",
                table: "Share",
                newName: "IX_Share_ShareFolderId");

            migrationBuilder.RenameColumn(
                name: "ShareFileId",
                schema: "public",
                table: "Link",
                newName: "ShareMediaId");

            migrationBuilder.RenameColumn(
                name: "ShareDirectoryId",
                schema: "public",
                table: "Link",
                newName: "ShareFolderId");

            migrationBuilder.RenameIndex(
                name: "IX_Link_ShareFileId",
                schema: "public",
                table: "Link",
                newName: "IX_Link_ShareMediaId");

            migrationBuilder.RenameIndex(
                name: "IX_Link_ShareDirectoryId",
                schema: "public",
                table: "Link",
                newName: "IX_Link_ShareFolderId");

            migrationBuilder.CreateTable(
                name: "Folder",
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
                    table.PrimaryKey("PK_Folder", x => x.Id);
                    table.ForeignKey(
                        name: "FolderParent___fk",
                        column: x => x.ParentId,
                        principalSchema: "public",
                        principalTable: "Folder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "Folders_User_Id_fk",
                        column: x => x.OwnerId,
                        principalSchema: "public",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Media",
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
                    FolderId = table.Column<int>(type: "int", nullable: false),
                    OwnerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Media", x => x.Id);
                    table.ForeignKey(
                        name: "Files_Folder_Id_fk",
                        column: x => x.FolderId,
                        principalSchema: "public",
                        principalTable: "Folder",
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

            migrationBuilder.CreateIndex(
                name: "IX_Folder_OwnerId",
                schema: "public",
                table: "Folder",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Folder_ParentId",
                schema: "public",
                table: "Folder",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Media_FolderId",
                schema: "public",
                table: "Media",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_Media_OwnerId",
                schema: "public",
                table: "Media",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "Link_File_Id_fk",
                schema: "public",
                table: "Link",
                column: "ShareMediaId",
                principalSchema: "public",
                principalTable: "Media",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "Link_Folder_Id_fk",
                schema: "public",
                table: "Link",
                column: "ShareFolderId",
                principalSchema: "public",
                principalTable: "Folder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "Share_Folder_Id_fk",
                schema: "public",
                table: "Share",
                column: "ShareFolderId",
                principalSchema: "public",
                principalTable: "Folder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "Share_Media_Id_fk",
                schema: "public",
                table: "Share",
                column: "ShareMediaId",
                principalSchema: "public",
                principalTable: "Media",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "Link_File_Id_fk",
                schema: "public",
                table: "Link");

            migrationBuilder.DropForeignKey(
                name: "Link_Folder_Id_fk",
                schema: "public",
                table: "Link");

            migrationBuilder.DropForeignKey(
                name: "Share_Folder_Id_fk",
                schema: "public",
                table: "Share");

            migrationBuilder.DropForeignKey(
                name: "Share_Media_Id_fk",
                schema: "public",
                table: "Share");

            migrationBuilder.DropTable(
                name: "Media",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Folder",
                schema: "public");

            migrationBuilder.RenameColumn(
                name: "ShareMediaId",
                schema: "public",
                table: "Share",
                newName: "ShareFileId");

            migrationBuilder.RenameColumn(
                name: "ShareFolderId",
                schema: "public",
                table: "Share",
                newName: "ShareDirectoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Share_ShareMediaId",
                schema: "public",
                table: "Share",
                newName: "IX_Share_ShareFileId");

            migrationBuilder.RenameIndex(
                name: "IX_Share_ShareFolderId",
                schema: "public",
                table: "Share",
                newName: "IX_Share_ShareDirectoryId");

            migrationBuilder.RenameColumn(
                name: "ShareMediaId",
                schema: "public",
                table: "Link",
                newName: "ShareFileId");

            migrationBuilder.RenameColumn(
                name: "ShareFolderId",
                schema: "public",
                table: "Link",
                newName: "ShareDirectoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Link_ShareMediaId",
                schema: "public",
                table: "Link",
                newName: "IX_Link_ShareFileId");

            migrationBuilder.RenameIndex(
                name: "IX_Link_ShareFolderId",
                schema: "public",
                table: "Link",
                newName: "IX_Link_ShareDirectoryId");

            migrationBuilder.CreateTable(
                name: "Directory",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OwnerId = table.Column<int>(type: "int", nullable: false),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
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
                    DirectoryId = table.Column<int>(type: "int", nullable: false),
                    OwnerId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    Extension = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    Path = table.Column<string>(type: "varchar(1024)", maxLength: 1024, nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false)
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

            migrationBuilder.AddForeignKey(
                name: "Link_Directory_Id_fk",
                schema: "public",
                table: "Link",
                column: "ShareDirectoryId",
                principalSchema: "public",
                principalTable: "Directory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "Link_File_Id_fk",
                schema: "public",
                table: "Link",
                column: "ShareFileId",
                principalSchema: "public",
                principalTable: "File",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "Share_Directory_Id_fk",
                schema: "public",
                table: "Share",
                column: "ShareDirectoryId",
                principalSchema: "public",
                principalTable: "Directory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "Share_File_Id_fk",
                schema: "public",
                table: "Share",
                column: "ShareFileId",
                principalSchema: "public",
                principalTable: "File",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
