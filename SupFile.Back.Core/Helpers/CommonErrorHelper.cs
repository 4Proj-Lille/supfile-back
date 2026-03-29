using System.Globalization;

namespace SupFile.Back.Core.Helpers;

public static class CommonErrorHelper
{
    public static CustomError NullValue(string? field = null) => new(
        ErrorsRes.Null_Value_Error_Title,
        field is null ? ErrorsRes.Null_Value_Error_Detail : string.Format(ErrorsRes.Null_Field_Error_Detail, field),
        ErrorType.BadRequest
    );

    public static CustomError Failure(string code, string detail) =>
        new(code, detail, ErrorType.Failure);

    public static CustomError NotFound(string code, string detail) =>
        new(code, detail, ErrorType.NotFound);

    public static CustomError BadRequest(string code, string detail) =>
        new(code, detail, ErrorType.BadRequest);

    public static CustomError Forbidden(string code, string detail) =>
        new(code, detail, ErrorType.Forbidden);
}
