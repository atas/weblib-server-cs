using System.Net;
using Microsoft.AspNetCore.Http;
using WebAppShared.Exceptions;
using WebAppShared.Metrics;

namespace WebAppShared.WebSys.MvcFilters;

public class CustomExceptionMiddleware(RequestDelegate next, IMetricsSvc metricsSvc)
{
    public async Task Invoke(HttpContext context /* other dependencies */)
    {
        try
        {
            await next(context);
        }
        catch (HttpStatusCodeException ex)
        {
            metricsSvc.Collect("HttpStatusCodeException", 1,
                new Dictionary<string, string> { { "StatusCode", ((int)ex.StatusCode).ToString() } });

            await HandleExceptionAsync(context, ex);
        }
        catch (Exception)
        {
            // Log the unhandled exception
            metricsSvc.CollectNamed(BaseAppEvent.CriticalException, 1);
            throw;
            // await HandleExceptionAsync(context, exceptionObj);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, HttpStatusCodeException e)
    {
        var result = new CustomExceptionErrorDetails
        {
            Message = string.IsNullOrEmpty(e.Message) ? e.StatusCode.ToString() : e.Message,
            StatusCode = (int)e.StatusCode
        };

        context.Response.StatusCode = (int)e.StatusCode;
        context.Response.ContentType = "application/json";

        return context.Response.WriteAsync(result.ToString());
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var result = new CustomExceptionErrorDetails
            { Message = exception.Message, StatusCode = (int)HttpStatusCode.InternalServerError }.ToString();
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        return context.Response.WriteAsync(result);
    }
}