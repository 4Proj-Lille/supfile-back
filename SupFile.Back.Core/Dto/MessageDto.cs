using Microsoft.AspNetCore.Mvc;

namespace SupFile.Back.Core.Dto;

public class MessageDto
{
    [FromForm(Name = "content")] public required string Content { get; set; }

    [FromForm(Name = "sendDate")] public required DateTime SendDate { get; set; }
    
    [FromForm(Name = "senderId")] public required int SenderId { get; set; }
    
    [FromForm(Name = "channelId")] public int? ChannelId { get; set; }
    
    [FromForm(Name = "receiverId")] public int? ReceiverId { get; set; }
}
