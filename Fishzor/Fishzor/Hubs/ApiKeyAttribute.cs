using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Fishzor.Hubs;

[AttributeUsage(AttributeTargets.Class)]
public class ApiKeyAttribute : Attribute, IAuthorizationFilter
{
    private const string ApiKeyHeaderName = "X-API-Key";
    private const string ApiKey = "your-secret-api-key"; // Store this securely, e.g., in configuration

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var potentialApiKey))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        if (!ApiKey.Equals(potentialApiKey))
        {
            context.Result = new UnauthorizedResult();
        }
    }
}