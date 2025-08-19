﻿namespace API.DTOs;

public class MessageDto
{
    public required string Id { get; set; }
    public required string Content { get; set; }
    public DateTime? DateRead { get; set; }
    public DateTime MessageSent { get; set; } = DateTime.UtcNow;
    public bool SenderDeteled { get; set; }
    public bool RecipientDeleted { get; set; }

    // nav propeerties
    public required string SenderId { get; set; }
    public required string SenderDisplayName { get; set; }
    public string? SenderImageUrl { get; set; }
    public required string RecipientId { get; set; }
    public required string RecipientDisplayName { get; set; }
    public string? RecipientImageUrl { get; set; }
}
