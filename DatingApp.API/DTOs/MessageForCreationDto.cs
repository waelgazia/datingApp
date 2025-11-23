namespace DatingApp.API.DTOs;

public class MessageForCreationDto
{
    public required string RecipientId { get; set; }
    public required string Content { get; set; }
}
