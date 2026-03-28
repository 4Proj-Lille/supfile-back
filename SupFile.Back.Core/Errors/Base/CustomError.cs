namespace SupFile.Back.Core.Errors.Base;

public class CustomError : Error
{
    public string Title { get; }

    public ErrorType ErrorType { get; }

    internal CustomError(string title, string detail, ErrorType errorType)
        : base(detail)
    {
        Title = title;
        ErrorType = errorType;
    }
}
