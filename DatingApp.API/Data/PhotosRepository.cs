using Microsoft.EntityFrameworkCore;

using DatingApp.API.Entities;
using DatingApp.API.Interfaces;

namespace DatingApp.API.Data;

public class PhotosRepository(AppDbContext _dbContext) : IPhotosRepository
{
    public async Task<IReadOnlyList<Photo>> GetUnapprovedPhotosAsync()
    {
        return await _dbContext.Photos
            .IgnoreQueryFilters()
            .Where(p => !p.IsApproved)
            .ToListAsync();
    }

    public async Task<Photo?> GetPhotoByIdAsync(int photoId)
    {
        return await _dbContext.Photos
            .IgnoreQueryFilters()
            .SingleOrDefaultAsync(p => p.Id == photoId);
    }

    public void DeletePhoto(Photo photo)
    {
        _dbContext.Photos.Remove(photo);
    }
}
