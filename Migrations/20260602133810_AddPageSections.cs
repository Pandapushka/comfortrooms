using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComfortRooms.Migrations
{
    /// <inheritdoc />
    public partial class AddPageSections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PageSections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SitePageId = table.Column<int>(type: "INTEGER", nullable: false),
                    TemplateKey = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    LayoutKey = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    Eyebrow = table.Column<string>(type: "TEXT", maxLength: 180, nullable: true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 240, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    ImageUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ImageAltText = table.Column<string>(type: "TEXT", maxLength: 240, nullable: true),
                    BackgroundClass = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    EyebrowColorClass = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    TitleColorClass = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    DescriptionColorClass = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    IsPublished = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageSections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PageSections_SitePages_SitePageId",
                        column: x => x.SitePageId,
                        principalTable: "SitePages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PageSectionButtons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PageSectionId = table.Column<int>(type: "INTEGER", nullable: false),
                    Text = table.Column<string>(type: "TEXT", maxLength: 160, nullable: false),
                    Url = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    StyleClass = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageSectionButtons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PageSectionButtons_PageSections_PageSectionId",
                        column: x => x.PageSectionId,
                        principalTable: "PageSections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PageSectionCards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PageSectionId = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 180, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 800, nullable: true),
                    IsLink = table.Column<bool>(type: "INTEGER", nullable: false),
                    Url = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageSectionCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PageSectionCards_PageSections_PageSectionId",
                        column: x => x.PageSectionId,
                        principalTable: "PageSections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PageSectionTestimonials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PageSectionId = table.Column<int>(type: "INTEGER", nullable: false),
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
                    table.PrimaryKey("PK_PageSectionTestimonials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PageSectionTestimonials_PageSections_PageSectionId",
                        column: x => x.PageSectionId,
                        principalTable: "PageSections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PageSectionPortfolioImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PageSectionId = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 180, nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    AltText = table.Column<string>(type: "TEXT", maxLength: 240, nullable: true),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageSectionPortfolioImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PageSectionPortfolioImages_PageSections_PageSectionId",
                        column: x => x.PageSectionId,
                        principalTable: "PageSections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PageSectionButtons_PageSectionId",
                table: "PageSectionButtons",
                column: "PageSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_PageSectionCards_PageSectionId",
                table: "PageSectionCards",
                column: "PageSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_PageSectionTestimonials_PageSectionId",
                table: "PageSectionTestimonials",
                column: "PageSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_PageSectionPortfolioImages_PageSectionId",
                table: "PageSectionPortfolioImages",
                column: "PageSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_PageSections_SitePageId",
                table: "PageSections",
                column: "SitePageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PageSectionButtons");

            migrationBuilder.DropTable(
                name: "PageSectionCards");

            migrationBuilder.DropTable(
                name: "PageSectionTestimonials");

            migrationBuilder.DropTable(
                name: "PageSectionPortfolioImages");

            migrationBuilder.DropTable(
                name: "PageSections");
        }
    }
}
