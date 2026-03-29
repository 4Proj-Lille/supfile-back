namespace SupFile.Back.Core.Errors;

public static class AuthErrors
{
    public static CustomError UnauthorizedForEntity<T, TId>(TId entityId) where T: IEntity<T, TId> =>
        CommonErrorHelper.Forbidden(
            ErrorsRes.Unauthorize_Title,
            ErrorsRes.Unauthorize_Detail
        );

    public static CustomError UserNotFound() =>
        CommonErrorHelper.NotFound(
            ErrorsRes.UserNotFound_Title,
            ErrorsRes.UserNotFound_Detail
        );

    public static CustomError RefreshTokenExpired() =>
        CommonErrorHelper.BadRequest(
            ErrorsRes.RefreshTokenExpired_Title,
            ErrorsRes.RefreshTokenExpired_Detail
        );
    
    public static CustomError EmailAlreadyExist() =>
        CommonErrorHelper.BadRequest(
            ErrorsRes.EmailAlreadyExist_Title,
            ErrorsRes.EmailAlreadyExist_Detail
        );
    
    public static CustomError UsernameAlreadyExist() =>
        CommonErrorHelper.BadRequest(
            ErrorsRes.UsernameAlreadyExist_Title,
            ErrorsRes.UsernameAlreadyExist_Detail
        );
    
        
    public static CustomError InvalidEmailOrPassword() =>
        CommonErrorHelper.BadRequest(
            ErrorsRes.InvalidEmailOrPassword_Title,
            ErrorsRes.InvalidEmailOrPassword_Detail
        );  
    
    public static CustomError EmailNotConfirmed() =>
        CommonErrorHelper.BadRequest(
            ErrorsRes.EmailNotConfirmed_Title,
            ErrorsRes.EmailNotConfirmed_Detail
        );
    
}
