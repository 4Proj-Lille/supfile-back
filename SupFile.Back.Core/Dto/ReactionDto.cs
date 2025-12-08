using Microsoft.AspNetCore.Mvc;

namespace SupFile.Back.Core.Dto;

public class ReactionDto
{
    [FromForm(Name = "content")] public required string Content { get; set; }
    
    [FromForm(Name = "messageId")] public required int MessageId { get; set; }
    
    [FromForm(Name = "senderId")] public required int SenderId { get; set; }
    
}
