using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Filters;

using DatingApp.API.Data;
using DatingApp.API.Extensions;

namespace DatingApp.API.helper;

/// <summary>
/// Filter to record user last active time (this filter is executed after a controller's action)
/// </summary>
public class LogUserActivity : IAsyncActionFilter
{
    /*
       - ActionExecutingContext context: Before the endpoint (action) executed: Request data, route values, controller, action arguments
       - ActionExecutionDelegate next: After endpoint (action) executed: Response, result (e.g., OkResult), exceptions, status code
     */
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // anything happens after this line is going to happen after the request has been executed in the API
        // controller if you want anything to happen before that, but it before the following line.
        ActionExecutedContext? contextResult = await next();

        bool? isAuthenticated = context.HttpContext.User.Identity?.IsAuthenticated;
        if (!isAuthenticated.GetValueOrDefault()) return;
        string memberId = context.HttpContext.User.GetMemberId();

        AppDbContext dbContext = contextResult.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
        await dbContext.Members
            .Where(m => m.Id == memberId)
            .ExecuteUpdateAsync(setter => setter.SetProperty(m => m.LastActive, DateTime.UtcNow));
    }
}
