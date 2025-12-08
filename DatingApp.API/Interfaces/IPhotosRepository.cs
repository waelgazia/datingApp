using DatingApp.API.Entities;

namespace DatingApp.API.Interfaces;

public interface IPhotosRepository
{
    Task<IReadOnlyList<Photo>> GetUnapprovedPhotosAsync();
    Task<Photo?> GetPhotoByIdAsync(int photoId);
    void DeletePhoto(Photo photo);
}
