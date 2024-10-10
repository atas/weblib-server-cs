using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using WebAppShared.MVC;

namespace WebAppSharedTest.MVC;

public class ViewRenderServiceTests
{
    private readonly Mock<IRazorViewEngine> _mockRazorViewEngine;
    private readonly Mock<ITempDataProvider> _mockTempDataProvider;
    private readonly Mock<IServiceScopeFactory> _mockServiceScopeFactory;

    public ViewRenderServiceTests()
    {
        _mockRazorViewEngine = new Mock<IRazorViewEngine>();
        _mockTempDataProvider = new Mock<ITempDataProvider>();
        _mockServiceScopeFactory = new Mock<IServiceScopeFactory>();
        
        Mock<IServiceScope> mockServiceScope = new();
        Mock<IServiceProvider> mockServiceProvider = new();

        _mockServiceScopeFactory.Setup(m => m.CreateScope()).Returns(mockServiceScope.Object);
        mockServiceScope.Setup(m => m.ServiceProvider).Returns(mockServiceProvider.Object);
    }
    
    [Fact]
    public async Task RenderToStringAsync_ReturnsRenderedViewAsString()
    {
        var viewName = "TestView";
        var model = new { }; // Use an appropriate model object

        var viewMock = new Mock<IView>();
        viewMock.Setup(v => v.RenderAsync(It.IsAny<ViewContext>()))
            .Callback<ViewContext>(vc => vc.Writer.Write("Rendered View Content"))
            .Returns(Task.CompletedTask);

        _mockRazorViewEngine.Setup(r => r.FindView(It.IsAny<ActionContext>(), viewName, It.IsAny<bool>()))
            .Returns(ViewEngineResult.Found(viewName, viewMock.Object));

        var service = new ViewRenderService(_mockRazorViewEngine.Object, _mockTempDataProvider.Object, _mockServiceScopeFactory.Object);
        var result = await service.RenderToStringAsync(viewName, model);

        Assert.Equal("Rendered View Content", result);
    }
    
    [Fact]
    public async Task RenderToStringAsync_ThrowsException_WhenViewNotFound()
    {
        var viewName = "NonExistentView";
        var model = new { }; // Use an appropriate model object

        _mockRazorViewEngine.Setup(r => r.FindView(It.IsAny<ActionContext>(), viewName, It.IsAny<bool>()))
            .Returns(ViewEngineResult.NotFound(viewName, new[] { "View not found" }));

        var service = new ViewRenderService(_mockRazorViewEngine.Object, _mockTempDataProvider.Object, _mockServiceScopeFactory.Object);

        await Assert.ThrowsAsync<ArgumentNullException>(() => service.RenderToStringAsync(viewName, model));
    }


}
