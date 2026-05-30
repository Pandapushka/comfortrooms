using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComfortRooms.Migrations;

public partial class AddPageContentBlocks : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "PageContentBlocks",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                SitePageId = table.Column<int>(type: "INTEGER", nullable: false),
                Key = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                Label = table.Column<string>(type: "TEXT", maxLength: 180, nullable: false),
                Value = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: false),
                SortOrder = table.Column<int>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PageContentBlocks", x => x.Id);
                table.ForeignKey(
                    name: "FK_PageContentBlocks_SitePages_SitePageId",
                    column: x => x.SitePageId,
                    principalTable: "SitePages",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_PageContentBlocks_SitePageId_Key",
            table: "PageContentBlocks",
            columns: new[] { "SitePageId", "Key" },
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "PageContentBlocks");
    }
}
