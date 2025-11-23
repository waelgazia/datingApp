using DatingApp.API.Base;

namespace DatingApp.API.ResourceParameters;

public class MembersParameters : EntityPagination
{
    public string? Gender { get; set; }
    public string? CurrentMemberId { get; set; }
    public int MinAge { get; set; } = 18;
    public int MaxAge { get; set; } = 100;
    public string OrderBy { get; set; } = "lastActive";
}
