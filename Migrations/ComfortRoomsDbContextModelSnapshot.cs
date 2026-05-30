using System;
using ComfortRooms.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ComfortRooms.Migrations;

[DbContext(typeof(ComfortRoomsDbContext))]
partial class ComfortRoomsDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder.HasAnnotation("ProductVersion", "8.0.17");

        modelBuilder.Entity("ComfortRooms.Models.AdminUser", b =>
        {
            b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("INTEGER");
            b.Property<DateTime>("CreatedAtUtc").HasColumnType("TEXT");
            b.Property<string>("Login").IsRequired().HasMaxLength(80).HasColumnType("TEXT");
            b.Property<string>("PasswordHash").IsRequired().HasMaxLength(512).HasColumnType("TEXT");
            b.HasKey("Id");
            b.HasIndex("Login").IsUnique();
            b.ToTable("AdminUsers");
        });

        modelBuilder.Entity("ComfortRooms.Models.LeadRequest", b =>
        {
            b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("INTEGER");
            b.Property<DateTime>("CreatedAtUtc").HasColumnType("TEXT");
            b.Property<string>("Message").HasMaxLength(2000).HasColumnType("TEXT");
            b.Property<string>("Name").IsRequired().HasMaxLength(160).HasColumnType("TEXT");
            b.Property<string>("Phone").IsRequired().HasMaxLength(80).HasColumnType("TEXT");
            b.Property<string>("SourcePageSlug").IsRequired().HasMaxLength(120).HasColumnType("TEXT");
            b.HasKey("Id");
            b.ToTable("LeadRequests");
        });

        modelBuilder.Entity("ComfortRooms.Models.PageImage", b =>
        {
            b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("INTEGER");
            b.Property<string>("AltText").HasMaxLength(240).HasColumnType("TEXT");
            b.Property<DateTime>("CreatedAtUtc").HasColumnType("TEXT");
            b.Property<string>("ImageUrl").IsRequired().HasMaxLength(500).HasColumnType("TEXT");
            b.Property<int>("SitePageId").HasColumnType("INTEGER");
            b.Property<int>("SortOrder").HasColumnType("INTEGER");
            b.Property<string>("Title").IsRequired().HasMaxLength(180).HasColumnType("TEXT");
            b.HasKey("Id");
            b.HasIndex("SitePageId");
            b.ToTable("PageImages");
        });

        modelBuilder.Entity("ComfortRooms.Models.PageContentBlock", b =>
        {
            b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("INTEGER");
            b.Property<string>("Key").IsRequired().HasMaxLength(120).HasColumnType("TEXT");
            b.Property<string>("Label").IsRequired().HasMaxLength(180).HasColumnType("TEXT");
            b.Property<int>("SitePageId").HasColumnType("INTEGER");
            b.Property<int>("SortOrder").HasColumnType("INTEGER");
            b.Property<string>("Value").IsRequired().HasMaxLength(4000).HasColumnType("TEXT");
            b.HasKey("Id");
            b.HasIndex("SitePageId", "Key").IsUnique();
            b.ToTable("PageContentBlocks");
        });

        modelBuilder.Entity("ComfortRooms.Models.SitePage", b =>
        {
            b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("INTEGER");
            b.Property<int>("SortOrder").HasColumnType("INTEGER");
            b.Property<string>("Slug").IsRequired().HasMaxLength(120).HasColumnType("TEXT");
            b.Property<string>("Title").IsRequired().HasMaxLength(180).HasColumnType("TEXT");
            b.HasKey("Id");
            b.HasIndex("Slug").IsUnique();
            b.ToTable("SitePages");
        });

        modelBuilder.Entity("ComfortRooms.Models.PageImage", b =>
        {
            b.HasOne("ComfortRooms.Models.SitePage", "SitePage")
                .WithMany("Images")
                .HasForeignKey("SitePageId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            b.Navigation("SitePage");
        });

        modelBuilder.Entity("ComfortRooms.Models.PageContentBlock", b =>
        {
            b.HasOne("ComfortRooms.Models.SitePage", "SitePage")
                .WithMany("ContentBlocks")
                .HasForeignKey("SitePageId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            b.Navigation("SitePage");
        });

        modelBuilder.Entity("ComfortRooms.Models.SitePage", b =>
        {
            b.Navigation("ContentBlocks");
            b.Navigation("Images");
        });
#pragma warning restore 612, 618
    }
}
