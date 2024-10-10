using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace WebAppShared.Security.RateLimiter;

public class RateLimitFilter<T> : ActionFilterAttribute where T : IRateLimiter
{
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        var svc = filterContext.HttpContext.RequestServices;
        IRateLimiter rateLimiter = svc.GetService<T>();
        
        rateLimiter.Run();

        base.OnActionExecuting(filterContext);
    }
}
