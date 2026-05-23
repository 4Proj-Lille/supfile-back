// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace SupFile.Back.Core.Constants;

public static class FileExtensionConstant
{
    public static readonly Dictionary<string, string> ExtensionToMime = new(StringComparer.OrdinalIgnoreCase)
    {
        { ".markdown", "text/markdown" },
        { ".md", "text/markdown" },
    };
}
