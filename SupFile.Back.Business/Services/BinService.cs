// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Transactions;
using SupFile.Back.Core.Enums;
using SupFile.Back.Core.Errors.Base;
using SupFile.Back.Resources;

namespace SupFile.Back.Business.Services;

public class BinService : IBinService
{
    private readonly IMediaService _mediaService;
    private readonly IMediaRepository _mediaRepository;
    private readonly IFolderService _folderService;

    public BinService(
        IMediaService mediaService,
        IFolderService folderService, IMediaRepository mediaRepository)
    {
        _mediaService = mediaService;
        _folderService = folderService;
        _mediaRepository = mediaRepository;
    }

    public async Task<Result> RestoreAsync(int id, ApplicationUser currentUser, ObjectType type)
    {
        switch (type)
        {
            case ObjectType.Media:
                {
                    var restoredMedia = await _mediaService.RestoreAsync(currentUser, id);
                    if (restoredMedia is not { IsSuccess: true, Value.FolderId: not null })
                    {
                        return restoredMedia.ToResult();
                    }

                    var folder = await _folderService.GetByIdAsync<Folder>(restoredMedia.Value.FolderId.Value);
                    if (folder.IsFailed || folder.Value == null || folder.Value.IsActive == false)
                    {
                        restoredMedia.Value.FolderId = null;
                    }

                    return restoredMedia.ToResult();
                }
            case ObjectType.Folder:
                {
                    var restoredFolder = await _folderService.RestoreAsync(currentUser, id);
                    if (restoredFolder is not { IsSuccess: true, Value: not null })
                        return restoredFolder.ToResult();

                    var chainResult = await _folderService.RestoreChainAsync(currentUser, id);
                    if (chainResult.IsSuccess) await _mediaRepository.RestoreByFolderIdsAsync(chainResult.Value);

                    var parentFolderId = restoredFolder.Value.ParentId;
                    if (!parentFolderId.HasValue)
                        return restoredFolder.ToResult();

                    var parentFolder = await _folderService.GetByIdAsync<Folder>(parentFolderId.Value);
                    if (parentFolder.IsFailed || parentFolder.Value == null || parentFolder.Value.IsActive == false)
                        restoredFolder.Value.ParentId = null;

                    return restoredFolder.ToResult();
                }
            default:
                return Result.Fail(BinErrors.InvalidTypeProvided());
        }
    }

    public async Task<Result> DeleteOneAsync(int id, ApplicationUser currentUser, ObjectType type)
    {
        switch (type)
        {
            case ObjectType.Media:
                {
                    var mediaDeleteResult = await _mediaService.DeleteOneAsync(currentUser, id);
                    if (mediaDeleteResult.IsSuccess)
                    {
                        return Result.Ok();
                    }

                    return Result.Fail(BinErrors.NoMediaFound());
                }
            case ObjectType.Folder:
                {
                    var folderDeleteResult = await _folderService.DeleteOneAsync(currentUser, id);
                    if (folderDeleteResult.IsSuccess)
                    {
                        return Result.Ok();
                    }

                    return Result.Fail(BinErrors.NoFolderFound());
                }
            default:
                return Result.Fail(BinErrors.InvalidTypeProvided());
        }
    }

    public async Task<Result> EmptyBinAsync(ApplicationUser currentUser)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var mediaEmptyResult = await _mediaService.DeleteAllSoftDeleted(currentUser);
        var folderEmptyResult = await _folderService.DeleteAllSoftDeleted(currentUser);

        var mediaIsEmpty = mediaEmptyResult.IsFailed && 
                           mediaEmptyResult.Errors.OfType<CustomError>().Any(e => e.Title == ErrorsRes.NoMediaFound_Title);

        var folderIsEmpty = folderEmptyResult.IsFailed && 
                            folderEmptyResult.Errors.OfType<CustomError>().Any(e => e.Title == ErrorsRes.NoFolderFound_Title);
        
        if ((mediaEmptyResult.IsFailed && !mediaIsEmpty) || (folderEmptyResult.IsFailed && !folderIsEmpty))
            return Result.Fail(BinErrors.BinItem());

        if (mediaIsEmpty && folderIsEmpty)
            return Result.Fail(BinErrors.EmptyBin());

        scope.Complete();
        return Result.Ok();
    }
    
    public async Task<Result<Tuple<List<Folder>, List<Media>>>> GetBinItemsAsync(ApplicationUser currentUser,
        ObjectType? type = null)
    {
        var mediaResult = new Result<List<Media>>();
        var folderResult = new Result<List<Folder>>();
        if (type is ObjectType.Media or null)
        {
            mediaResult = await _mediaService.GetSoftDeleted<Media>(currentUser);
        }

        if (type is ObjectType.Folder or null)
        {
            folderResult = await _folderService.GetSoftDeleted<Folder>(currentUser);
        }

        if (mediaResult.IsSuccess && folderResult.IsSuccess)
        {
            return Tuple.Create(folderResult.Value, mediaResult.Value);
        }

        return Result.Fail<Tuple<List<Folder>, List<Media>>>(BinErrors.BinItem());
    }
}
