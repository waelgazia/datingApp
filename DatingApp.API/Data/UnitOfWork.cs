using Microsoft.EntityFrameworkCore;

using DatingApp.API.Interfaces;

namespace DatingApp.API.Data;

public class UnitOfWork(AppDbContext _dbContext) : IUnitOfWork
{
    private IMembersRepository? _memberRepository;
    private ILikesRepository? _likesRepository;
    private IMessagesRepository? _messageRepository;
    private IPhotosRepository? _photosRepository;

    public IMembersRepository MembersRepository =>
        _memberRepository ??= new MembersRepository(_dbContext);

    public ILikesRepository LikesRepository =>
        _likesRepository ??= new LikesRepository(_dbContext);

    public IMessagesRepository MessagesRepository =>
        _messageRepository ??= new MessagesRepository(_dbContext);

    public IPhotosRepository PhotosRepository =>
        _photosRepository ??= new PhotosRepository(_dbContext);

    public async Task<bool> Complete()
    {
        try
        {
            return await _dbContext.SaveChangesAsync() >= 0;
        }
        catch (DbUpdateException ex)
        {
            throw new Exception("An error occurred while saving changes", ex);
        }
    }

    public bool HasChanges()
    {
        return _dbContext.ChangeTracker.HasChanges();
    }
}
