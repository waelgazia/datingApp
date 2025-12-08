namespace DatingApp.API.Entities;

public class Photo
{
    public int Id { get; set; }
    public required string Url { get; set; }
    public string? PublicId { get; set; }
    public bool IsApproved { get; set; }

    // navigation property
    public string MemberId { get; set; } = null!;
    public Member Member { get; set; } = null!;
}
