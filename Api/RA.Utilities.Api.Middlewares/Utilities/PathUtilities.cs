using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace RA.Utilities.Api.Middlewares.Utilities;

internal static class PathUtilities
{
    public static bool ShouldIgnorePath(PathString path, ISet<string> pathsToIgnore)
    {
        if (!path.HasValue)
        {
            return false;
        }

        foreach (string pathToIgnore in pathsToIgnore)
        {
            if (path.Value.StartsWith(pathToIgnore, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}
