using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;

public class MessageRepository(AppDbContext context) : IMessageRepository
{
    public async Task<MessageDto?> AddMessagesAsync(MessageDto messageDto)
    {
        var messageEntity = new Message()
        {
            Content = messageDto.Content,
            RecipientId = messageDto.RecipientId,
            SenderId = messageDto.SenderId,
            DateRead = messageDto.DateRead,
            MessageSent = messageDto.MessageSent,
            RecipientDeleted = messageDto.RecipientDeleted,
            SenderDeteled = messageDto.SenderDeteled,
        };
        context.Messages.Add(messageEntity);
        int result = await context.SaveChangesAsync();
        messageDto.Id = messageEntity.Id;
        return result > 0 ?  messageDto : null;
    }

    public async Task<int> DeleteMessageAsync(string messageId)
    {
        var message = await context.Messages.FindAsync(messageId);
        if (message == null)
        {
            return -1;
        }
        context.Messages.Remove(message);
        var result = await context.SaveChangesAsync();
        return result;
    }

    public async Task<MessageDto?> GetMessageByIdAsync(string id)
    {
        return await context.Messages
            .Select(MessageExtension.ToDtoProjection())
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<PaginatedResult<MessageDto>> GetMessagesForMemberAsync(MessageParam messageParam)
    {
        var query = context.Messages
            .OrderByDescending(x => x.MessageSent)
            .AsQueryable();

        query = messageParam.Container switch
        {
            "Outbox" => query.Where(x => x.SenderId == messageParam.MemberId && !x.SenderDeteled),
            _ => query.Where(x => x.RecipientId == messageParam.MemberId && !x.RecipientDeleted),
        };

        var messageQuery = query.Select(MessageExtension.ToDtoProjection());

        return await PaginationHelper.CreateAsync(messageQuery, messageParam.PageNumber, messageParam.PageSize);
    }

    public async Task<IReadOnlyList<MessageDto>> GetMessageThreadAsync(string currentMemberId, string recipientId)
    {
        await context.Messages
            .Where(x => x.RecipientId == currentMemberId && x.SenderId == recipientId && x.DateRead == null)
            .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.DateRead, DateTime.UtcNow));

        return await context.Messages
            .Where(x => (x.RecipientId == currentMemberId && !x.RecipientDeleted && x.SenderId == recipientId)
                        || (x.SenderId == currentMemberId && !x.SenderDeteled && x.RecipientId == recipientId))
            .OrderBy(x => x.MessageSent)
            .Select(MessageExtension.ToDtoProjection())
            .ToListAsync();
    }

    public async Task<int> DeletePartAsync(MessageDto messageDto)
    {
        var message = await context.Messages.FirstOrDefaultAsync(x => x.Id == messageDto.Id && x.SenderId == messageDto.SenderId && x.RecipientId == messageDto.RecipientId);
        if (message == null)
        {
            return -1;
        }
        if (messageDto.SenderDeteled)
        {
            message.SenderDeteled = true;
        }
        else
        {
            message.RecipientDeleted = true;
        }
        context.Messages.Update(message);
        var result = await context.SaveChangesAsync();
        return result;
    }
}
