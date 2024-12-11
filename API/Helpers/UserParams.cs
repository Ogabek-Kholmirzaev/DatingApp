namespace API.Helpers;

public class UserParams : PaginationParams
{
    public string? Gender { get; set; }
    public string? CurrentUsername { get; set; }
    public int? MinAge { get; set; }
    public int? MaxAge { get; set; }
}
