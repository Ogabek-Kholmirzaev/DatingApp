namespace API.Helpers;

public class MessageParams : PaginationParams
{
    public required string Username { get; set; }
    public string Container { get; set; } = "Unread";
}
