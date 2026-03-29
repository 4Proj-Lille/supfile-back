namespace SupFile.Back.Core.Errors;

public static class LinkErrors
{
    public static CustomError InvalidType() =>
        CommonErrorHelper.BadRequest(
            ErrorsRes.InvalidType_Title,
            ErrorsRes.InvalidType_Detail
        );

    public static CustomError CannotAcceptOwnShareLink() =>
        CommonErrorHelper.BadRequest(
            ErrorsRes.CannotAcceptOwnShareLink_Title,
            ErrorsRes.CannotAcceptOwnShareLink_Detail
        );

    public static CustomError LinkExpired() =>
        CommonErrorHelper.BadRequest(
            ErrorsRes.LinkExpired_Title,
            ErrorsRes.LinkExpired_Detail
        );
}
