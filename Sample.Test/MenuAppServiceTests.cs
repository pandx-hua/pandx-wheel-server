using Moq;
using pandx.Wheel.Menus;
using Sample.Application.Menus;
using Xunit;
using Assert = Xunit.Assert;

namespace Sample.Test;

public class MenuAppServiceTests
{
    [Fact]
    public async Task GetMenusAsync_RemovesUnpermittedMenus()
    {
        // Arrange
        var mockMenuManager = new Mock<IMenuManager>();
        var menus = new List<Menu>
        {
            new() { Permission = "Allowed", Component = "Component1" },
            new() { Permission = "NotAllowed", Component = "Component2" }
        };
        mockMenuManager.Setup(mm => mm.GetShrunkMenus()).Returns(menus);

        var service = new MenuAppService(mockMenuManager.Object);

        // Act
        var result = await service.GetMenusAsync();

        // Assert
        Assert.Single(result.Items);
        Assert.Equal("Component1", result.Items[0].Component);
    }

    [Fact]
    public async Task GetMenusAsync_RemovesEmptyMenus()
    {
        // Arrange
        var mockMenuManager = new Mock<IMenuManager>();
        var menus = new List<Menu>
        {
            new() { Permission = "Allowed", Component = "Component1" },
            new() { Permission = "Allowed", Component = null }
        };
        mockMenuManager.Setup(mm => mm.GetShrunkMenus()).Returns(menus);

        var service = new MenuAppService(mockMenuManager.Object);

        // Act
        var result = await service.GetMenusAsync();

        // Assert
        Assert.Single(result.Items);
        Assert.Equal("Component1", result.Items[0].Component);
    }

    [Fact]
    public async Task GetMenusAsync_ReturnsAllPermittedAndNonEmptyMenus()
    {
        // Arrange
        var mockMenuManager = new Mock<IMenuManager>();
        var menus = new List<Menu>
        {
            new() { Permission = "Allowed", Component = "Component1" },
            new() { Permission = "Allowed", Component = "Component2" }
        };
        mockMenuManager.Setup(mm => mm.GetShrunkMenus()).Returns(menus);

        var service = new MenuAppService(mockMenuManager.Object);

        // Act
        var result = await service.GetMenusAsync();

        // Assert
        Assert.Equal(2, result.Items.Count);
    }
}