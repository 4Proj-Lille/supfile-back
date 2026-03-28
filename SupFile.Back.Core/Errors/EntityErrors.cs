namespace SupFile.Back.Core.Errors;

public static class EntityErrors
{
    public static CustomError NotFound<T>() =>
        CommonErrorHelper.BadRequest(
            ErrorsRes.Entity_Not_Found_Title,
            ErrorsRes.Entity_Not_Found_Detail
        );

    public static CustomError NotFoundWithId<T, TId>(TId id) =>
        CommonErrorHelper.BadRequest(
            string.Format(ErrorsRes.Entity_Not_Found_With_Id_Title, typeof(T).Name),
            string.Format(ErrorsRes.Entity_Not_Found_With_Id_Detail, typeof(T).Name)
        );
}
