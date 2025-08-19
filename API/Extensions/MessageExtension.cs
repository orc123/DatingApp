using API.DTOs;
using API.Entities;
using System.Linq.Expressions;

namespace API.Extensions;

public static class MessageExtension
{
    public static MessageDto ToMessageDto(this Message message)
    {
        return new MessageDto()
        {
            Id = message.Id,
            Content = message.Content,
            RecipientId = message.RecipientId,
            RecipientDisplayName = message.Recipient.DisplayName,
            RecipientImageUrl = message.Recipient.ImageUrl,
            MessageSent = message.MessageSent,
            DateRead = message.DateRead,
            RecipientDeleted = message.RecipientDeleted,
            SenderDeteled = message.SenderDeteled,
            SenderDisplayName = message.Sender.DisplayName,
            SenderId = message.Sender.Id,
            SenderImageUrl = message.Sender.ImageUrl,
        };
    }

    public static Expression<Func<Message, MessageDto>> ToDtoProjection()
    {
        return message => new MessageDto
        {
            Id = message.Id,
            Content = message.Content,
            RecipientId = message.RecipientId,
            RecipientDisplayName = message.Recipient.DisplayName,
            RecipientImageUrl = message.Recipient.ImageUrl,
            MessageSent = message.MessageSent,
            DateRead = message.DateRead,
            RecipientDeleted = message.RecipientDeleted,
            SenderDeteled = message.SenderDeteled,
            SenderDisplayName = message.Sender.DisplayName,
            SenderId = message.Sender.Id,
            SenderImageUrl = message.Sender.ImageUrl,
        };
    }
}
