namespace DatingApp.API.Entities;

public class Connection(string connectionId, string userId)
{
    public string ConnectionId { get; set; } = connectionId;
    public string UserId { get; set; } = userId;

    // navigation properties
    public string GroupId { get; set; } = null!;
    public Group Group { get; set; } = null!;
}
