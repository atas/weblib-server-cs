using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using WebLibServer.SharedLogic.KVStore;
using WebLibServer.WebSys.DI;

namespace WebLibServer.MVC;

/// <summary>
/// Renders a given Razor view to a string.
/// </summary>
/// <param name="razorViewEngine"></param>
/// <param name="tempDataProvider"></param>
/// <param name="serviceScopeFactory"></param>
[Service(InterfaceToBind = typeof(IViewRenderService)), UsedImplicitly]
public class ViewRenderService(
    IRazorViewEngine razorViewEngine,
    ITempDataProvider tempDataProvider,
    IServiceScopeFactory serviceScopeFactory)
    : IViewRenderService
{
    public async Task<string> RenderToStringAsync(string viewName, object model)
    {
        var httpContext = new DefaultHttpContext
        { RequestServices = serviceScopeFactory.CreateScope().ServiceProvider };
        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

        await using var sw = new StringWriter();
        var viewResult = razorViewEngine.FindView(actionContext, viewName, false);

        if (viewResult.View == null) throw new ArgumentNullException($"{viewName} does not match any available view");

        var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
        {
            Model = model
        };

        var viewContext = new ViewContext(
            actionContext,
            viewResult.View,
            viewDictionary,
            new TempDataDictionary(actionContext.HttpContext, tempDataProvider),
            sw,
            new HtmlHelperOptions()
        );

        await viewResult.View.RenderAsync(viewContext);
        return sw.ToString();
    }
}