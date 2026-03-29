namespace SupFile.Back.Core.Errors;

public static class RepositoryErrors
{
    public static CustomError NoChangesOnDelete() =>
        CommonErrorHelper.NotFound(
            ErrorsRes.NoChangesOnDelete_Title,
            ErrorsRes.NoChangesOnDelete_Detail
        );
}
