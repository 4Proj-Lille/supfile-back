namespace SupFile.Back.Core.Enums.Entities;

/// <summary>
/// The user language enum. It is used to store the language of the user.
/// </summary>
public enum UserLanguage
{
    
    /// <summary>
    /// The default user language. Used when the user has no language.
    /// </summary>
    [LocalizedDescription(nameof(EnumDescriptionResource.UserLanguage_English), typeof(EnumDescriptionResource))]
    English = 0,
    
    /// <summary>
    /// The french user language. Used when the user has a french language.
    /// </summary>
    [LocalizedDescription(nameof(EnumDescriptionResource.UserLanguage_French), typeof(EnumDescriptionResource))]
    French = 1,
}