using API.Data;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class MessageRepository(DataContext context, IMapper mapper) : IMessageRepository
{
    public async Task AddMessageAsync(Message message)
    {
        await context.Messages.AddAsync(message);
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
            "Inbox" => query.Where(x => x.Recipient.UserName == @params.Username && !x.RecipientDeleted),
            "Outbox" => query.Where(x => x.Sender.UserName == @params.Username && !x.SenderDeleted),
            _ => query.Where(x => x.Recipient.UserName == @params.Username && x.DateRead == null && !x.RecipientDeleted)
        };

        var messages = query.ProjectTo<MessageDto>(mapper.ConfigurationProvider);
        return await PagedList<MessageDto>.CreateAsync(messages, @params.PageNumber, @params.PageSize);
    }

    public async Task<IEnumerable<MessageDto>> GetMessageThreadAsync(
        string currentUsername,
        string recipientUsername)
    {
        var messages = await context.Messages
            .Include(x => x.Sender).ThenInclude(u => u.Photos)
            .Include(x => x.Recipient).ThenInclude(u => u.Photos)
            .Where(x =>
                x.Recipient.UserName == currentUsername && !x.RecipientDeleted && x.Sender.UserName == recipientUsername ||
                x.Sender.UserName == currentUsername && !x.SenderDeleted && x.Recipient.UserName == recipientUsername)
            .OrderBy(x => x.MessageSent)
            .ToListAsync();

        var unreadMessages = messages
            .Where(x => x.Recipient.UserName == currentUsername && x.DateRead == null)
            .ToList();
        
        if (unreadMessages.Count > 0)
        {
            unreadMessages.ForEach(x => x.DateRead = DateTime.UtcNow);
            await SaveAllAsync();
        }

        return mapper.Map<IEnumerable<MessageDto>>(messages);
    }

    public async Task<bool> SaveAllAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }
}
