using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Base;

public class PagedList<T> : List<T>
{
    public PaginationMetadata PaginationMetadata { get; }

    private PagedList(List<T> items, int count, int pageNumber, int pageSize)
    {
        PaginationMetadata = new PaginationMetadata
        {
            PageNumber = pageNumber,
            TotalPages = (int)Math.Ceiling(count / (double)pageSize),
            PageSize = pageSize,
            TotalCount = count
        };
        AddRange(items);
    }

    public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
    {
        int count = await source.CountAsync();
        List<T> items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedList<T>(items, count, pageNumber, pageSize);
    }
}
