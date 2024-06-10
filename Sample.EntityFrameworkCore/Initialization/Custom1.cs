namespace Sample.EntityFrameworkCore.Initialization;

public class Custom1 : ICustomCreator
{
    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }
}