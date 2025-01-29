namespace API.Helpers;

public class MessageParams : PaginationParams
{
    public string Username { get; set; } = default!;
    public string Container { get; set; } = "Unread";
}
