namespace DatingApp.API.Entities;

public class Message
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string Content { get; set; }
    public DateTime? ReadAt { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public bool SenderDeleted { get; set; }
    public bool RecipientDeleted { get; set; }

    // navigation properties
    public required string SenderId { get; set; }
    public Member Sender { get; set; } = null!;

    public required string RecipientId { get; set; }
    public Member Recipient { get; set; } = null!;
}