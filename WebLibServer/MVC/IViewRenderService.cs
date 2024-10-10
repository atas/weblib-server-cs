namespace WebLibServer.MVC;

public interface IViewRenderService
{
    Task<string> RenderToStringAsync(string viewName, object model);
}
