namespace Sample.EntityFrameworkCore.Initialization;

public class Custom2 : ICustomCreator
{
    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }
}