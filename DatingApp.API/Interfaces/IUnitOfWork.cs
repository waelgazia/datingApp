namespace DatingApp.API.Interfaces;

public interface IUnitOfWork
{
    public IMembersRepository MembersRepository { get; }
    public ILikesRepository LikesRepository { get; }
    public IMessagesRepository MessagesRepository { get; }
    public IPhotosRepository PhotosRepository { get; }
    Task<bool> Complete();
    bool HasChanges();
}
