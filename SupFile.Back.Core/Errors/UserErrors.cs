namespace SupFile.Back.Core.Errors;

public static class UserErrors
{
    public static CustomError IncorrectPassword() =>
        CommonErrorHelper.BadRequest(
            ErrorsRes.IncorrectPassword_Title,
            ErrorsRes.IncorrectPassword_Detail
        );
    
    public static CustomError PasswordDoesntMatch() =>
        CommonErrorHelper.BadRequest(
            ErrorsRes.PasswordDoesntMatch_Title,
            ErrorsRes.PasswordDoesntMatch_Detail
        );
    
    public static CustomError AspNetUserNotFound() =>
        CommonErrorHelper.NotFound(
            ErrorsRes.AspNetUserNotFound_Title,
            ErrorsRes.AspNetUserNotFound_Detail
        );
    
    public static CustomError UserNameNotfound() =>
        CommonErrorHelper.NotFound(
            ErrorsRes.UserNameNotFound_Title,
            ErrorsRes.UserNameNotFound_Detail
        );
    
    public static CustomError InvalidUserName() =>
        CommonErrorHelper.BadRequest(
            ErrorsRes.InvalidUserName_Title,
            ErrorsRes.InvalidUserName_Detail
        );
}
