using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using RA.Utilities.Core.Constants;

namespace RA.Utilities.Api.Utilities;

internal static class CommonUtilities
{
    public static string GetRequestId(HttpContext context)
    {
        context.Request.Headers.TryGetValue(
            HeaderParameters.XRequestId,
            out StringValues correlationId);

        return correlationId.FirstOrDefault() ?? context.TraceIdentifier;
    }

    public static bool ShouldIgnorePath(PathString path, ISet<string> pathsToIgnore)
    {
        if (!path.HasValue)
            return false;

        foreach (string pathToIgnore in pathsToIgnore)
        {
            if (path.Value.StartsWith(pathToIgnore, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }
}
