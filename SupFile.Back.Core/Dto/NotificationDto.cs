using Microsoft.AspNetCore.Mvc;

namespace SupFile.Back.Core.Dto;

public class NotificationDto
{
    [FromForm(Name = "content")] public required string Content { get; set; }
    
    [FromForm(Name = "sendDate")] public required DateTime SendDate { get; set; }
    
    [FromForm(Name = "type")] public required string Type { get; set; }
    
    [FromForm(Name = "isActive")] public required bool IsActive { get; set; }
    
    [FromForm(Name = "messageId")] public int? MessageId { get; set; }
    
    [FromForm(Name = "userId")] public required int UserId { get; set; }
}
