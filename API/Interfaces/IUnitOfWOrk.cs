namespace API.Interfaces
{
    public interface IUnitOfWOrk
    {
        IUserRepository UserRepository { get; }
        IMessageRepository MessageRepository { get; }
        ILikeRepository LikeRepository { get; }
        Task<bool> Complete();
        bool HasChanges();
    }
}