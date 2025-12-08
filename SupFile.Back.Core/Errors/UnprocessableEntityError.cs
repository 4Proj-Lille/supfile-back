namespace SupFile.Back.Core.Errors;

public class UnprocessableEntityError : CustomError
{
    public UnprocessableEntityError(string message) : base(HttpStatusCode.UnprocessableEntity, message)
    {
    }
}
