namespace DatingApp.API.DTOs;

public class MessageDto
{
    public required string Id { get; set; }
    public required string SenderId { get; set; }
    public required string SenderDisplayName { get; set; }
    public string? SenderImageUrl { get; set; }
    public required string RecipientId { get; set; }
    public required string RecipientDisplayName { get; set; }
    public string? RecipientImageUrl { get; set; }
    public required string Content { get; set; }
    public DateTime? ReadAt { get; set; }
    public required DateTime SentAt { get; set; }
}
