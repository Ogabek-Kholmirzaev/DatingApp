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
    IMapper mapper,
    IHubContext<PresenceHub> presenceHub)
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

        var group = await AddToGroupAsync(groupName);
        await Clients.Group(groupName).SendAsync("UpdatedGroup", group);  

        var messages = await messageRepository.GetMessageThreadAsync(Context.User.GetUsername(), recipientUsername!);
        await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var group = await RemoveFromMessageGroupAsync();
        await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
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

        var groupName = GetGroupName(sender.UserName!, recipient.UserName!);
        var group = await messageRepository.GetMessageGroupAsync(groupName);
        if (group != null && group.Connections.Any(x => x.Username == recipient.UserName))
        {
            message.DateRead = DateTime.UtcNow;
        }
        else
        {
            var connections = await PresenceTracker.GetConnectionsForUserAsync(recipient.UserName!);
            if (connections?.Count > 0)
            {
                await presenceHub.Clients.Clients(connections)
                    .SendAsync("NewMessageReceived", new { username, knownAs = sender.KnownAs });
            }
        }

        await messageRepository.AddMessageAsync(message);

        if (await messageRepository.SaveAllAsync())
        {
            await Clients.Group(groupName).SendAsync("NewMessage", mapper.Map<MessageDto>(message));
        }
        else
        {
            throw new HubException("Failed to send message");
        }
    }

    private async Task<Group> AddToGroupAsync(string groupName)
    {
        var username = Context.User?.GetUsername() ?? throw new Exception("Could not get user");
        var group = await messageRepository.GetMessageGroupAsync(groupName);
        var connection = new Connection { ConnectionId = Context.ConnectionId, Username = username };

        if (group == null)
        {
            group = new Group { Name = groupName };
            await messageRepository.AddGroupAsync(group);
        }

        group.Connections.Add(connection);

        if (await messageRepository.SaveAllAsync())
        {
            return group;
        }

        throw new HubException("Failed to join group");
    }

    private async Task<Group> RemoveFromMessageGroupAsync()
    {
        var group = await messageRepository.GetGroupForConnectionAsync(Context.ConnectionId);
        var connection = group?.Connections.FirstOrDefault(c => c.ConnectionId == Context.ConnectionId);
        if (connection != null && group != null)
        {
            messageRepository.RemoveConnection(connection);
            if (await messageRepository.SaveAllAsync())
            {
                return group;
            }
        }

        throw new Exception("Failed to remove from group");
    }

    private string GetGroupName(string sender, string recipient)
    {
        var stringCompare = string.CompareOrdinal(sender, recipient) < 0;
        return stringCompare ? $"{sender}-{recipient}" : $"{recipient}-{sender}";
    }
}
