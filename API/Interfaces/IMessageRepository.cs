using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface IMessageRepository
{
    Task AddMessageAsync(Message message);
    void DeleteMessage(Message message);
    Task<Message?> GetMessageAsync(int id);
    Task<PagedList<MessageDto>> GetMessagesForUserAsync(MessageParams @params);
    Task<IEnumerable<MessageDto>> GetMessageThreadAsync(string currentUsername, string recipientUsername);
    Task<bool> SaveAllAsync();
    Task AddGroupAsync(Group group);
    void RemoveConnection(Connection connection);
    Task<Connection?> GetConnectionAsync(string connectionId);
    Task<Group?> GetMessageGroupAsync(string groupName);
    Task<Group?> GetGroupForConnectionAsync(string connectionId);
}
