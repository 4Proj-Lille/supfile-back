namespace SupFile.Back.Core.Errors;

public class ForbiddenError : CustomError
{
    public ForbiddenError(string message) : base(HttpStatusCode.Forbidden, message)
    {
    }
}
