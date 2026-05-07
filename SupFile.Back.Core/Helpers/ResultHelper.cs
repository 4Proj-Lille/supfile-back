// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Identity;

namespace SupFile.Back.Core.Helpers;

public static class ResultHelper
{
    public static Result ToFluentResult(IdentityResult identityResult)
        => identityResult.Succeeded
            ? Result.Ok()
            : Result.Fail(identityResult.Errors.Select(e => CommonErrorHelper.BadRequest(e.Code, e.Description)));
}
