namespace SupFile.Back.Core.Errors;

public class UnauthorizedError : CustomError
{
    public UnauthorizedError(string message) : base(HttpStatusCode.Unauthorized, message)
    {
    }
}
