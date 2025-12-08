namespace SupFile.Back.Core.Errors;

public class BadRequestError : CustomError
{
    public BadRequestError(string message) : base(HttpStatusCode.BadRequest, message)
    {
    }
}
