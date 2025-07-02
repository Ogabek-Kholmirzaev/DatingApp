using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities;

public class Photo
{
    public int Id { get; set; }
    public required string Url { get; set; }
    public bool IsMain { get; set; }
    public bool IsApproved { get; set; }
    public string? PublicId { get; set; }
    public int AppUserId { get; set; }

    [ForeignKey(nameof(AppUserId))]
    public AppUser AppUser { get; set; } = null!;
}