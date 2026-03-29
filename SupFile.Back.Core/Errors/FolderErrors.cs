namespace SupFile.Back.Core.Errors;

public static class FolderErrors
{
    public static CustomError CannotBeOwnParent() =>
        CommonErrorHelper.BadRequest(
            ErrorsRes.CannotBeOwnParent_Title,
            ErrorsRes.CannotBeOwnParent_Detail
        );

    public static CustomError ParentFolderNotOwnedByUser() =>
        CommonErrorHelper.Forbidden(
            ErrorsRes.ParentFolderNotOwnedByUser_Title,
            ErrorsRes.ParentFolderNotOwnedByUser_Detail
        );
}
