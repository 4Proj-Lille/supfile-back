// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace SupFile.Back.Core.Enums.Entities;

/// <summary>
/// The user theme enum. It is used to store the theme of the user.
/// </summary>
public enum UserTheme
{
    /// <summary>
    /// The light user theme. Used when the user has a light theme.
    /// </summary>
    [LocalizedDescription(nameof(EnumDescriptionResource.UserTheme_Light), typeof(EnumDescriptionResource))]
    Light = 0,
    
    /// <summary>
    /// The dark user theme. Used when the user has a dark theme.
    /// </summary>
    [LocalizedDescription(nameof(EnumDescriptionResource.UserTheme_Dark), typeof(EnumDescriptionResource))]
    Dark = 1,
}