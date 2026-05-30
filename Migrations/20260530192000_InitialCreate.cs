using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComfortRooms.Migrations;

public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "AdminUsers",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Login = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                PasswordHash = table.Column<string>(type: "TEXT", maxLength: 512, nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AdminUsers", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "LeadRequests",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Name = table.Column<string>(type: "TEXT", maxLength: 160, nullable: false),
                Phone = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                Message = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                SourcePageSlug = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_LeadRequests", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "SitePages",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Slug = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                Title = table.Column<string>(type: "TEXT", maxLength: 180, nullable: false),
                SortOrder = table.Column<int>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SitePages", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "PageImages",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                SitePageId = table.Column<int>(type: "INTEGER", nullable: false),
                Title = table.Column<string>(type: "TEXT", maxLength: 180, nullable: false),
                ImageUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                AltText = table.Column<string>(type: "TEXT", maxLength: 240, nullable: true),
                SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PageImages", x => x.Id);
                table.ForeignKey(
                    name: "FK_PageImages_SitePages_SitePageId",
                    column: x => x.SitePageId,
                    principalTable: "SitePages",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_AdminUsers_Login",
            table: "AdminUsers",
            column: "Login",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_PageImages_SitePageId",
            table: "PageImages",
            column: "SitePageId");

        migrationBuilder.CreateIndex(
            name: "IX_SitePages_Slug",
            table: "SitePages",
            column: "Slug",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "AdminUsers");
        migrationBuilder.DropTable(name: "LeadRequests");
        migrationBuilder.DropTable(name: "PageImages");
        migrationBuilder.DropTable(name: "SitePages");
    }
}
