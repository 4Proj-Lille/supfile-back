using Microsoft.AspNetCore.Mvc;
using SupFile.Back.Core.Dto;
using SupFile.Back.Core.Enums;

namespace SupFile.Back.Core.Interfaces.Services;

public interface IShareService : IBaseService<Share, int>
{
    Task<Result<Share>> AddOneAsync(ApplicationUser currentUser, Share entity);

    Task<Result> DeleteOneAsync(ApplicationUser currentUser, int id);
    
    Task<Result<List<TMapped>>> GetAccessUsersAsync<TMapped>(int objectId, ApplicationUser currentUser, ObjectType type);
    
    Task<Result<Tuple<List<TFolder>, List<TMedia>>>> GetAllAsync<TFolder, TMedia>(ApplicationUser currentUser, SearchQuery query, int? folderId = null, int? size = null);

    Task<Result<Share>> UpdatePermissionAsync(ApplicationUser currentUser, UpdateSharePermissionDto dto);
}
