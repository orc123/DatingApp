using API.DTOs;
using API.Extensions;
using API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR;

[Authorize]
public class MessageHub(
    IMessageRepository messageRepository, 
    IMemberRepository memberRepository,
    IHubContext<PresenceHub> presenceHub
) : Hub
{
    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();

        var otherUser = httpContext?.Request?.Query["userId"].ToString()
            ?? throw new HubException("Other user not found");

        var groupName = GetGroupName(GetUserId(), otherUser);

        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        await AddToGroup(groupName);

        var messages = await messageRepository.GetMessageThreadAsync(GetUserId(), otherUser);
        await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
    }

    public async Task SendMessage(CreateMessageDto createMessageDto)
    {
        var sender = await memberRepository.GetMemberByIdAsync(GetUserId());
        var recipient = await memberRepository.GetMemberByIdAsync(createMessageDto.RecipientId);

        if (sender == null || recipient == null || sender.Id == createMessageDto.RecipientId)
        {
            throw new HubException("Cannot send message");
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

        var groupName = GetGroupName(sender.Id, recipient.Id);

        var group = await messageRepository.GetMessageGroupAsync(groupName);
        var userInGroup = group != null && group.Connections.Any(x => x.UserId == message.RecipientId);
        if (userInGroup)
        {
            message.DateRead = DateTime.UtcNow;
        }

        var result = await messageRepository.AddMessagesAsync(message);
        if (result == null)
        {
            throw new HubException("Failed to send message");
        }
        await Clients.Group(groupName).SendAsync("NewMessage", message);
        var connections = await PresenceTracker.GetConnectionsForUser(message.RecipientId);
        if (connections != null && connections.Count > 0 && !userInGroup)
        {
            await presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived", message);
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await messageRepository.RemoveContectionAsync(Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    private async Task<bool> AddToGroup(string groupName)
    {
        var group = await messageRepository.GetMessageGroupAsync(groupName);
        var connection = new ConnectionDto
        {
            ConnectionId = Context.ConnectionId,
            UserId = GetUserId()
        };

        if (group == null)
        {
            group = new GroupDto
            {
                Name = groupName
            };
            return await messageRepository.AddGroupAsync(group);
        }
        group.Connections.Add(connection);
        return true;
    }

    private static string GetGroupName(string? caller, string? other)
    {
        var stringCompare = string.CompareOrdinal(caller, other) < 0;

        return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
    }

    private string GetUserId() => Context.User?.GetMemberId() ?? throw new HubException("Could not get user id");
}
