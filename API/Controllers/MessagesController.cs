using API.DTOs;
using API.Entities;
using API.Extentions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class MessagesController(
    IMessageRepository messageRepository,
    IUserRepository userRepository,
    IMapper mapper) : BaseApiController
{

    [HttpGet]
    public async Task<PagedList<MessageDto>> GetMessagesForUser([FromQuery] MessageParams @params)
    {
        @params.Username = User.GetUsername();

        var messages = await messageRepository.GetMessagesForUserAsync(@params);

        Response.AddPaginationHeader(messages);

        return messages;
    }

    [HttpPost]
    public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
    {
        var username = User.GetUsername();
        if (username.Equals(createMessageDto.RecipientUsername, StringComparison.CurrentCultureIgnoreCase))
        {
            return BadRequest("You cannot message yourself");
        }

        var sender = await userRepository.GetUserByUsernameAsync(username);
        var recipient = await userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

        if (sender == null || recipient == null)
        {
            return BadRequest("Cannot send message at this time");
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

        return await messageRepository.SaveAllAsync()
            ? Ok(mapper.Map<MessageDto>(message))
            : BadRequest("Failed to save a message");
    }

    [HttpGet("thread/{username}")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
    {
        var currentUsername = User.GetUsername();
        var messages = await messageRepository.GetMessageThreadAsync(currentUsername, username);

        return Ok(messages);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMessage(int id)
    {
        var username = User.GetUsername();
        var message = await messageRepository.GetMessageAsync(id);

        if (message == null)
        {
            return BadRequest("Cannot delete this message");
        }

        if (message.SenderUsername != username || message.RecipientUsername != username)
        {
            return Forbid();
        }

        if (message.SenderUsername == username)
        {
            message.SenderDeleted = true;
        }

        if (message.RecipientUsername == username)
        {
            message.RecipientDeleted = true;
        }

        if (message is { SenderDeleted: true, RecipientDeleted: true })
        {
            messageRepository.DeleteMessage(message);
        }

        return await messageRepository.SaveAllAsync() ? Ok() : BadRequest("Problem deleting the message");
    }
}
