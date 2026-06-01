using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComfortRooms.Migrations;

public partial class AddHomeTestimonials : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "HomeTestimonials",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Title = table.Column<string>(type: "TEXT", maxLength: 180, nullable: false),
                Text = table.Column<string>(type: "TEXT", maxLength: 1200, nullable: false),
                Author = table.Column<string>(type: "TEXT", maxLength: 160, nullable: true),
                ImageUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                AltText = table.Column<string>(type: "TEXT", maxLength: 240, nullable: true),
                SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                IsPublished = table.Column<bool>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_HomeTestimonials", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "HomeTestimonials");
    }
}
