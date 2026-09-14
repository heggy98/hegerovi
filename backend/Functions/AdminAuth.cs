using Microsoft.Azure.Functions.Worker.Http;

namespace Hegerovi.Backend.Functions;

public static class AdminAuth
{
    public static bool IsAuthorized(HttpRequestData req, string expectedApiKey)
    {
        if (string.IsNullOrEmpty(expectedApiKey))
        {
            return false;
        }

        return req.Headers.TryGetValues("x-admin-key", out var values)
            && values.Contains(expectedApiKey);
    }
}
