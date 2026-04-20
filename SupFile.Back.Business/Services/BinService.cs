// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace SupFile.Back.Business.Services;

public class BinService : IBinService
{
    private readonly IMediaService _mediaService;
    private readonly IFolderService _folderService;

    public BinService(
        IMediaService mediaService,
        IFolderService folderService
    )
    {
        _mediaService = mediaService;
        _folderService = folderService;
    }

    public async Task<Result> RestoreAsync(int id, ApplicationUser currentUser, string type)
    {
        if (type == "media")
        {
            var restoredMedia= await _mediaService.RestoreAsync(currentUser, id);
            if (restoredMedia.IsSuccess && restoredMedia.Value != null && restoredMedia.Value.FolderId.HasValue)
            {
                var folder = await _folderService.GetByIdAsync<Folder>(restoredMedia.Value.FolderId.Value);
                if (folder.IsFailed || folder.Value == null || folder.Value.IsActive == false){
                    restoredMedia.Value.FolderId = null;
                }
                    
            }
            return restoredMedia.ToResult();
        }
        if (type == "folder")
        {
            var restoredFolder = await _folderService.RestoreAsync(currentUser, id);
            if (restoredFolder.IsSuccess && restoredFolder.Value != null)
            {
                var parentFolderId = restoredFolder.Value.ParentId;
                if (parentFolderId.HasValue)
                {
                    var parentFolder = await _folderService.GetByIdAsync<Folder>(parentFolderId.Value);
                    if (parentFolder.IsFailed || parentFolder.Value == null || parentFolder.Value.IsActive == false)
                    {
                        restoredFolder.Value.ParentId = null;
                    }
                }
            }
            return restoredFolder.ToResult();
            
        }
        return Result.Fail(BinErrors.InvalidTypeProvided());
    }
    
    public async Task<Result> DeleteOneAsync(int id, ApplicationUser currentUser, string type)
    {
        if (type == "media")
        {
            var mediaDeleteResult = await _mediaService.DeleteOneAsync(currentUser, id);
            if (mediaDeleteResult.IsSuccess)
            {
                return Result.Ok();
            }
            return Result.Fail(BinErrors.NoMediaFound());
        }
        if (type == "folder")
        {
            var folderDeleteResult = await _folderService.DeleteOneAsync(currentUser, id);
            if (folderDeleteResult.IsSuccess)
            {
                return Result.Ok();
            }
            return Result.Fail(BinErrors.NoFolderFound());
        }
        
        return Result.Fail(BinErrors.InvalidTypeProvided());
    }
    
    public async Task<Result> EmptyBinAsync(ApplicationUser currentUser)
    {
        var mediaEmptyResult = await _mediaService.DeleteAllSoftDeleted(currentUser);
        var folderEmptyResult = await _folderService.DeleteAllSoftDeleted(currentUser);
        
        if (mediaEmptyResult.IsSuccess && folderEmptyResult.IsSuccess)
        {
            return Result.Ok();
        }
        
        return Result.Fail(BinErrors.BinItem());
    }
    
    public async Task<Result<Tuple<List<Folder>,List<Media>>>>GetBinItemsAsync(ApplicationUser currentUser, string? type = null)
    {
        var mediaResult = new Result<List<Media>>();
        var folderResult = new Result<List<Folder>>();
        if (type == "media" || string.IsNullOrEmpty(type)){
            mediaResult = await _mediaService.GetSoftDeleted<Media>(currentUser);
        }
        if (type == "folder" || string.IsNullOrEmpty(type)){
             folderResult = await _folderService.GetSoftDeleted<Folder>(currentUser);
        }
        
        if (mediaResult.IsSuccess && folderResult.IsSuccess)
        {
            return Tuple.Create(folderResult.Value, mediaResult.Value);
        }
        
        return Result.Fail<Tuple<List<Folder>,List<Media>>>(BinErrors.BinItem());
    }
}
