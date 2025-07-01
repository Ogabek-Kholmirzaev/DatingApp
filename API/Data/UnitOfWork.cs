using API.Interfaces;

namespace API.Data;

public class UnitOfWork(
    DataContext context,
    IUserRepository userRepository,
    IMessageRepository messageRepository,
    ILikesRepository likesRepository)
    : IUnitOfWork
{
    public IUserRepository UserRepository => userRepository;

    public IMessageRepository MessageRepository => messageRepository;

    public ILikesRepository LikesRepository => likesRepository;

    public async Task<bool> CompleteAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }

    public bool HasChanges()
    {
        return context.ChangeTracker.HasChanges();
    }
}
