namespace SupFile.Back.Core.Errors;

public class ConflictError : CustomError
{
    public ConflictError(string message) : base(HttpStatusCode.Conflict, message)
    {
    }
}
