namespace DatingApp.API.Base;

public class PaginationMetadata
{
    public int PageNumber { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public bool HasNext { get { return PageNumber < TotalPages; } }
    public bool HasPrevious { get { return PageNumber > 1; } }
}
