namespace SupFile.Back.Core.Enums.Auth;

/// <summary>
/// The attachment type enum. It is used to store the type of the attachment.
/// </summary>
public enum PermissionActions
{
    /// <summary>
    /// Read a list of all the entities
    /// </summary>
    [LocalizedDescription(nameof(EnumDescriptionResource.PermissionActions_ReadAll), typeof(EnumDescriptionResource))]
    ReadAll = 0,
    
    /// <summary>
    /// Read a specific entity
    /// </summary>
    [LocalizedDescription(nameof(EnumDescriptionResource.PermissionActions_ReadOne), typeof(EnumDescriptionResource))]
    ReadOne = 1,
    
    /// <summary>
    /// Create a new entity
    /// </summary>
    [LocalizedDescription(nameof(EnumDescriptionResource.PermissionActions_CreateOne), typeof(EnumDescriptionResource))]
    CreateOne = 2,
    
    /// <summary>
    /// Update a specific entity
    /// </summary>
    [LocalizedDescription(nameof(EnumDescriptionResource.PermissionActions_UpdateOne), typeof(EnumDescriptionResource))]
    UpdateOne = 3,
    
    /// <summary>
    /// Delete a specific entity
    /// </summary>
    [LocalizedDescription(nameof(EnumDescriptionResource.PermissionActions_DeleteOne), typeof(EnumDescriptionResource))]
    DeleteOne = 4,
    
    /// <summary>
    /// Update a specific entity owned by the user
    /// </summary>
    [LocalizedDescription(nameof(EnumDescriptionResource.PermissionActions_UpdateOwn), typeof(EnumDescriptionResource))]
    UpdateOwn = 5,
    
    /// <summary>
    /// Delete a specific entity owned by the user
    /// </summary>
    [LocalizedDescription(nameof(EnumDescriptionResource.PermissionActions_DeleteOwn), typeof(EnumDescriptionResource))]
    DeleteOwn = 6,
}