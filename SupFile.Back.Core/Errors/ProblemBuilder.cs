using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace SupFile.Back.Core.Errors;

public static class ProblemBuilder
{
    public static ProblemDetails Create(HttpStatusCode statusCode, string message)
    {
        return new ProblemDetails { Status = (int)statusCode, Title = statusCode.ToString(), Detail = message };
    }
}
