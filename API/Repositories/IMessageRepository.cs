using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Repositories;

public interface IMessageRepository
{
    Task<MessageDto?> AddMessagesAsync(MessageDto messageDto);
    Task<int> DeleteMessageAsync(string messageId);

    Task<MessageDto?> GetMessageByIdAsync(string id);

    Task<PaginatedResult<MessageDto>> GetMessagesForMemberAsync(MessageParam messageParam);

    Task<IReadOnlyList<MessageDto>> GetMessageThreadAsync(string currentMemberId, string recipientId);

    Task<int> DeletePartAsync(MessageDto messageDto);

    Task<bool> AddGroupAsync(GroupDto groupDto);
    Task RemoveContectionAsync(string connectionId);
    Task<ConnectionDto?> GetConnectionAsync(string connectionId);
    Task<GroupDto?> GetMessageGroupAsync(string groupName);
    Task<GroupDto?> GetGroupForConnectionAsync(string connectionId);
}
