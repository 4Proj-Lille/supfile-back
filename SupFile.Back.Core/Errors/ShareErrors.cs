namespace SupFile.Back.Core.Errors;

public static class ShareErrors
{
    public static CustomError ShareNotFound() =>
        CommonErrorHelper.NotFound(
            ErrorsRes.ShareNotFound_Title,
            ErrorsRes.ShareNotFound_Detail
        );

    public static CustomError AlreadyShared() =>
        CommonErrorHelper.BadRequest(
            ErrorsRes.AlreadyShared_Title,
            ErrorsRes.AlreadyShared_Detail
        );
}
