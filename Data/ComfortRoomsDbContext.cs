using ComfortRooms.Models;
using Microsoft.EntityFrameworkCore;

namespace ComfortRooms.Data;

public sealed class ComfortRoomsDbContext(DbContextOptions<ComfortRoomsDbContext> options) : DbContext(options)
{
    public DbSet<AdminUser> AdminUsers => Set<AdminUser>();

    public DbSet<SitePage> SitePages => Set<SitePage>();

    public DbSet<PageImage> PageImages => Set<PageImage>();

    public DbSet<LeadRequest> LeadRequests => Set<LeadRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AdminUser>(entity =>
        {
            entity.HasIndex(user => user.Login).IsUnique();
            entity.Property(user => user.Login).HasMaxLength(80);
            entity.Property(user => user.PasswordHash).HasMaxLength(512);
        });

        modelBuilder.Entity<SitePage>(entity =>
        {
            entity.HasIndex(page => page.Slug).IsUnique();
            entity.Property(page => page.Slug).HasMaxLength(120);
            entity.Property(page => page.Title).HasMaxLength(180);
            entity.HasMany(page => page.Images)
                .WithOne(image => image.SitePage)
                .HasForeignKey(image => image.SitePageId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PageImage>(entity =>
        {
            entity.Property(image => image.Title).HasMaxLength(180);
            entity.Property(image => image.ImageUrl).HasMaxLength(500);
            entity.Property(image => image.AltText).HasMaxLength(240);
        });

        modelBuilder.Entity<LeadRequest>(entity =>
        {
            entity.Property(request => request.Name).HasMaxLength(160);
            entity.Property(request => request.Phone).HasMaxLength(80);
            entity.Property(request => request.Message).HasMaxLength(2000);
            entity.Property(request => request.SourcePageSlug).HasMaxLength(120);
        });
    }
}
