namespace SupFile.Back.Core.Errors;

public static class MediaErrors
{
    public static CustomError InvalidSortField(string sortField) =>
        CommonErrorHelper.BadRequest(
            string.Format(ErrorsRes.InvalidSortField_Title, sortField),
            string.Format(ErrorsRes.InvalidSortField_Detail, sortField)
        );
}
