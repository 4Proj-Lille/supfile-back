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
}
