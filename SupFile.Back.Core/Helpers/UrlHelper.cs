// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text;
using Microsoft.AspNetCore.WebUtilities;

namespace SupFile.Back.Core.Helpers;

public static class UrlHelper
{
    public static string UrlEncode(string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        return WebEncoders.Base64UrlEncode(bytes);
    }

    public static string UrlDecode(string value)
    {
        var bytes = WebEncoders.Base64UrlDecode(value);
        return Encoding.UTF8.GetString(bytes);
    }
}
