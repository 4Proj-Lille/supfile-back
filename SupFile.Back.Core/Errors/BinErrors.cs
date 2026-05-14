namespace SupFile.Back.Core.Errors;

public static class BinErrors
{
    public static CustomError InvalidTypeProvided() =>
        CommonErrorHelper.BadRequest(
            ErrorsRes.InvalidTypeProvided_Title,
            ErrorsRes.InvalidTypeProvided_Detail
        );

    public static CustomError NoMediaFound() =>
        CommonErrorHelper.NotFound(
            ErrorsRes.NoMediaFound_Title,
            ErrorsRes.NoMediaFound_Detail
        );
    
    public static CustomError NoFolderFound() =>
        CommonErrorHelper.NotFound(
            ErrorsRes.NoFolderFound_Title,
            ErrorsRes.NoFolderFound_Detail
        );
    
    public static CustomError EmptyBin() =>
        CommonErrorHelper.NotFound(
            ErrorsRes.EmptyBin_Title,
            ErrorsRes.EmptyBin_Detail
        );
    
    public static CustomError BinItem() =>
        CommonErrorHelper.Failure(
            ErrorsRes.BinItem_Title,
            ErrorsRes.BinItem_Detail
        );
}
