namespace API.Helpers;

public class MessageParam : PagingParams
{
    public string? MemberId { get; set; }
    public string Container { get; set; } = "Inbox";
}