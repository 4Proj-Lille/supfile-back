namespace SupFile.Back.Core.Extensions;

public static class UserLanguageExtensions
{
    public static string ToCultureCode(this UserLanguage language)
    {
        return language switch
        {
            UserLanguage.French => "fr-FR",
            UserLanguage.English => "en-US",
            _ => "en-US"
        };
    }

    public static UserLanguage FromCultureCode(string? cultureCode)
    {
        return cultureCode switch
        {
            "fr" or "fr-FR" => UserLanguage.French,
            "en" or "en-US" => UserLanguage.English,
            _ => UserLanguage.English
        };
    }
}
