using DatingApp.API.Base;

namespace DatingApp.API.ResourceParameters;

public class LikesParameters : EntityPagination
{
	public string MemberId { get; set; } = string.Empty;

	/// <summary>
	/// Define which members to return (likes the member, or liked by the member)
	/// </summary>
	public string Predicate { get; set; } = LikePredicate.LIKED;
}

public static class LikePredicate
{
	public const string LIKED = "liked";
	public const string LIKED_BY = "likedBy";
	public const string MUTUAL = "mutual";
}
