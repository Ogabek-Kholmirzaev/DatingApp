using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class RegisterDto
{
    [Required]
    public string Username { get; set; } = default!;

    [Required]
    public string KnownAs { get; set; } = default!;

    [Required]
    public string Gender { get; set; } = default!;

    [Required]
    public DateOnly DateOfBirth { get; set; }

    [Required]
    public string City { get; set; } = default!;

    [Required]
    public string Country { get; set; } = default!;

    [Required]
    [StringLength(8, MinimumLength = 4)]
    public string Password { get; set; } = default!;
}
