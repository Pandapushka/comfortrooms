using System.ComponentModel.DataAnnotations;

namespace ComfortRooms.Models;

public sealed class LeadRequest
{
    public int Id { get; set; }

    [Required]
    [MaxLength(160)]
    public required string Name { get; set; }

    [Required]
    [MaxLength(80)]
    public required string Phone { get; set; }

    [MaxLength(2000)]
    public string? Message { get; set; }

    [MaxLength(120)]
    public required string SourcePageSlug { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
