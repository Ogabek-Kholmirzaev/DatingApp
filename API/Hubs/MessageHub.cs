using API.DTOs;
using API.Entities;
using API.Extentions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace API.Hubs;

public class MessageHub(
    IMessageRepository messageRepository,
    IUserRepository userRepository,
    IMapper mapper)
    : Hub
{
    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var recipientUsername = httpContext?.Request.Query["user"];

        if (string.IsNullOrEmpty(recipientUsername) || Context.User == null)
        {
            throw new Exception("Cannot join group");
        }

        var groupName = GetGroupName(Context.User.GetUsername(), recipientUsername!);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        var messages = await messageRepository.GetMessageThreadAsync(Context.User.GetUsername(), recipientUsername!);
        await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(CreateMessageDto createMessageDto)
    {
        var username = Context.User?.GetUsername() ?? throw new Exception("Could not get user");
        if (username.Equals(createMessageDto.RecipientUsername, StringComparison.CurrentCultureIgnoreCase))
        {
            throw new HubException("You cannot message yourself");
        }

        var sender = await userRepository.GetUserByUsernameAsync(username);
        var recipient = await userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

        if (sender == null || recipient == null)
        {
            throw new HubException("Cannot send message at this time");
        }

        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = username,
            RecipientUsername = createMessageDto.RecipientUsername,
            Content = createMessageDto.Content
        };

        await messageRepository.AddMessageAsync(message);

        if (await messageRepository.SaveAllAsync())
        {
            var groupName = GetGroupName(sender.UserName!, recipient.UserName!);
            await Clients.Group(groupName).SendAsync("NewMessage", mapper.Map<MessageDto>(message));
        }
        else
        {
            throw new HubException("Failed to send message");
        }
    }

    private string GetGroupName(string sender, string recipient)
    {
        var stringCompare = string.CompareOrdinal(sender, recipient) < 0;
        return stringCompare ? $"{sender}-{recipient}" : $"{recipient}-{sender}";
    }
}
