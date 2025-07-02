namespace API.DTOs;

public class PhotoApprovalDto
{
    public int Id { get; set; }
    public required string Url { get; set; }
    public string? Username { get; set; }
    public bool IsApproved { get; set; }
}