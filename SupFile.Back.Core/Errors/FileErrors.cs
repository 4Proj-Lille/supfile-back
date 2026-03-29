namespace SupFile.Back.Core.Errors;

public static class FileErrors
{
    public static CustomError NotFound(string name, string extension) => FileNotFound();

    public static CustomError FileNotFound() =>
        CommonErrorHelper.NotFound(
            ErrorsRes.FileNotFound_Title,
            ErrorsRes.FileNotFound_Detail
        );

    public static CustomError ExtensionDoesNotStartWithADot() =>
        CommonErrorHelper.BadRequest(
            ErrorsRes.ExtensionDoesNotStartWithADot_Title,
            ErrorsRes.ExtensionDoesNotStartWithADot_Detail
        );

    public static CustomError EmptyContent() =>
        CommonErrorHelper.BadRequest(
            ErrorsRes.EmptyContent_Title,
            ErrorsRes.EmptyContent_Detail
        );

    public static CustomError AlreadyExists(string name, string extension) =>
        CommonErrorHelper.BadRequest(
            ErrorsRes.FileAlreadyExists_Title,
            string.Format(ErrorsRes.FileAlreadyExists_Detail, name, extension)
        );
    
    public static CustomError FileUploadFailed() =>
    CommonErrorHelper.Failure(
        "File upload failed",
        "File upload failed"
        // ErrorsRes.FileUploadFailed_Title,
        // ErrorsRes.FileUploadFailed_Detail
    );
}
