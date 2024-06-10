using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Sample.EntityFrameworkCore;

public class SampleDbContextFactory : IDesignTimeDbContextFactory<SampleDbContext>
{
    public SampleDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<SampleDbContext>();
        //读取pandx.Wheel.Host.WebAPI下的appsettings.json文件
        var currentDirectory = Environment.CurrentDirectory;
        var directory = new DirectoryInfo(currentDirectory);

        var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(directory.Parent!.FullName, "Sample.Host.WebAPI"))
            .AddJsonFile("appsettings.json", true, true);
        var configuration = configurationBuilder.Build();

        builder.UseSqlServer(configuration.GetConnectionString("Default"));

        return new SampleDbContext(builder.Options, null!, null!, null!);
    }
}