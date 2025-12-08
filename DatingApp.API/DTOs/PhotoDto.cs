namespace DatingApp.API.DTOs;

public class PhotoDto
{
    public required int Id { get; set; }
    public required string Url { get; set; }
    public string? PublicId { get; set; }
    public required string MemberId { get; set; }
    public required bool IsApproved { get; set; }
}
