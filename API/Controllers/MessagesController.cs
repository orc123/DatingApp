using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class MessagesController(IMessageRepository messageRepository, IMemberRepository memberRepository) : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult<MessageDto>> CreateMessageAsync(CreateMessageDto createMessageDto)
    {
        var sender = await memberRepository.GetMemberByIdAsync(User.GetMemberId());
        var recipient = await memberRepository.GetMemberByIdAsync(createMessageDto.RecipientId);

        if (sender == null || recipient == null || sender.Id == createMessageDto.RecipientId)
        {
            return BadRequest("Cannot send this message");
        }

        var message = new MessageDto
        {
            Id = Guid.NewGuid().ToString(),
            SenderId = sender.Id,
            SenderDisplayName = sender.DisplayName,
            SenderImageUrl = sender.ImageUrl,
            Content = createMessageDto.Content,
            RecipientId = recipient.Id,
            RecipientDisplayName = recipient.DisplayName,
            RecipientImageUrl = recipient.ImageUrl,
        };

        var result = await messageRepository.AddMessagesAsync(message);
        if (result == null)
        {
            return BadRequest("Failed to send message");
        }
        return result;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResult<MessageDto>>> GetMessagesByContainer([FromQuery] MessageParam messageParam)
    {
        messageParam.MemberId = User.GetMemberId();
        return await messageRepository.GetMessagesForMemberAsync(messageParam);
    }

    [HttpGet("thread/{recipientId}")]
    public async Task<ActionResult<IReadOnlyList<MessageDto>>> GetMessageThread(string recipientId) 
        => Ok(await messageRepository.GetMessageThreadAsync(User.GetMemberId(), recipientId));

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMessage(string id)
    {
        var memberId = User.GetMemberId();
        var message = await messageRepository.GetMessageByIdAsync(id);
        if (message == null || (message.SenderId != memberId && message.RecipientId != memberId))
        {
            return NotFound("Message not found or you do not have permission to delete it.");
        }
        if (message.SenderId == memberId)
        {
            message.SenderDeteled = true;
        }
        if (message.RecipientId == memberId)
        {
            message.RecipientDeleted = true;
        }

        var deletePart = await messageRepository.DeletePartAsync(message);
        if (deletePart <= 0)
        {
            return BadRequest("Failed to delete message");
        }
        // If both sender and recipient have deleted the message, delete it from the database

        if (message is { SenderDeteled: true, RecipientDeleted: true})
        {
            var result = await messageRepository.DeleteMessageAsync(id);
            if (result <= 0)
            {
                return BadRequest("Failed to delete message");
            }
            return NoContent();
        }
        return NoContent();

    }
}
