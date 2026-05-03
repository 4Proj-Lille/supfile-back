namespace SupFile.Back.Core.Errors;

public static class MediaErrors
{
    public static CustomError InvalidSortField(string sortField) =>
        CommonErrorHelper.BadRequest(
            string.Format(ErrorsRes.InvalidSortField_Title, sortField),
            string.Format(ErrorsRes.InvalidSortField_Detail, sortField)
        );
    
    public static CustomError InvalidStorageSizeGroupBy() =>
        CommonErrorHelper.BadRequest(
            ErrorsRes.InvalidStorageSizeGroupBy_Title,
          ErrorsRes.InvalidStorageSizeGroupBy_Detail
        );
    
    public static CustomError InvalidUniqueId(string uniqueId) =>
        CommonErrorHelper.BadRequest(
            string.Format(ErrorsRes.InvalidUniqueId_Title, uniqueId),
            string.Format(ErrorsRes.InvalidUniqueId_Detail, uniqueId)
        );
    
    public static CustomError MediaIsDeleted(string uniqueId) =>
        CommonErrorHelper.NotFound(
            string.Format(ErrorsRes.MediaIsDeleted_Title, uniqueId),
            string.Format(ErrorsRes.MediaIsDeleted_Detail, uniqueId)
        );
    
    public static CustomError InvalideProfilePictureType() =>
        CommonErrorHelper.BadRequest(
            ErrorsRes.InvalideProfilePictureType_Title,
            ErrorsRes.InvalideProfilePictureType_Detail
        );
    
    public static CustomError StorageLimitExceeded() =>
        CommonErrorHelper.BadRequest(
            ErrorsRes.StorageLimitExceeded_Title,
            ErrorsRes.StorageLimitExceeded_Detail
        );
    
    public static CustomError NoMediaFound() =>
        CommonErrorHelper.NotFound(
            ErrorsRes.NoMediaFound_Title,
            ErrorsRes.NoMediaFound_Detail
        );
}
