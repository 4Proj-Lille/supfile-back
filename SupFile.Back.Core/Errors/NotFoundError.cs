namespace SupFile.Back.Core.Errors;

public class NotFoundError : CustomError
{
    public NotFoundError(string message) : base(HttpStatusCode.NotFound, message)
    {
    }
}
