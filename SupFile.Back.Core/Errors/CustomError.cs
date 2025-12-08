using System.Net;

namespace SupFile.Back.Core.Errors;

public class CustomError : Error
{
    public CustomError(HttpStatusCode statusCode, string message)
        : base(message)
    {
        StatusCode = statusCode;
    }

    public HttpStatusCode StatusCode { get; }
}
