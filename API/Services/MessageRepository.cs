using API.Data;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace API.Services;

public class MessageRepository(DataContext context, IMapper mapper) : IMessageRepository
{
    public void AddMessage(Message message)
    {
        context.Messages.Add(message);
    }

    public void DeleteMessage(Message message)
    {
        context.Messages.Remove(message);
    }

    public async Task<Message?> GetMessageAsync(int id)
    {
        return await context.Messages.FindAsync(id);
    }

    public async Task<PagedList<MessageDto>> GetMessagesForUserAsync(MessageParams @params)
    {
        var query = context.Messages.OrderByDescending(x => x.MessageSent).AsQueryable();

        query = @params.Container switch
        {
            "Inbox" => query.Where(x => x.Recipient.UserName == @params.Username),
            "Outbox" => query.Where(x => x.Sender.UserName == @params.Username),
            _ => query.Where(x => x.Recipient.UserName == @params.Username && x.DateRead == null)
        };

        var messages = query.ProjectTo<MessageDto>(mapper.ConfigurationProvider);

        return await PagedList<MessageDto>.CreateAsync(messages, @params.PageNumber, @params.PageSize);
    }

    public Task<IEnumerable<MessageDto>> GetMessageThreadAsync(string currentUsername, string recipientUsername)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> SaveAllAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }
}
