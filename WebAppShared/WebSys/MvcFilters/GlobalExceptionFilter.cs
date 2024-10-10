using System.Net;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Hosting;
using WebAppShared.Exceptions;
using WebAppShared.Metrics;
using WebAppShared.WebSys.Exceptions;

namespace WebAppShared.WebSys.MvcFilters;

[UsedImplicitly]
public class GlobalExceptionFilter(IWebHostEnvironment env, IMetricsSvc metricsSvc) : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        metricsSvc.CollectNamed(BaseAppEvent.UserException, 1);

        // User-level exception, non-critical.
        if (context.Exception is GlobalErrorException errorEx)
            HandleErrorException(context, errorEx);
        else if (context.Exception.GetBaseException() is GlobalErrorException errorBaseEx)
            HandleErrorException(context, errorBaseEx);
        else if (context.Exception is HttpJsonError err) HandleJsonError(context, err);
    }

    private void HandleErrorException(ExceptionContext context, GlobalErrorException e)
    {
        context.ExceptionHandled = true;

        // Initialise ViewData for Error View Model
        var viewData =
            new ViewDataDictionary<ErrorViewModel>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = e.GetModel()
            };

        // Show error page with View Model
        context.Result = new ViewResult
        {
            ViewName = e.ViewName,
            ViewData = viewData
        };
    }

    private void HandleJsonError(ExceptionContext context, HttpJsonError e)
    {
        context.ExceptionHandled = true;
        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        var jsonFormatted = new
        {
            error = new
            {
                message = e.Title,
                Stack = env.IsDevelopment() ? e.StackTrace : ""
            }
        };

        context.Result = new JsonResult(jsonFormatted);
    }
}