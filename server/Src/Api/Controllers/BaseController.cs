using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public class BaseController : ControllerBase
{
    protected Guid GetUserId()
    {
        if (!User.Identity?.IsAuthenticated ?? true)
            throw new UnauthorizedAccessException("User is not authenticated.");

        var idValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(idValue))
            throw new UnauthorizedAccessException("User identifier claim is missing.");

        if (Guid.TryParse(idValue, out var guid))
            return guid;

        throw new UnauthorizedAccessException("User identifier claim is invalid.");
    }
}