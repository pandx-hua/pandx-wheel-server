using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using pandx.Wheel.DependencyInjection;
using pandx.Wheel.Domain.Repositories;
using pandx.Wheel.Domain.UnitOfWork;
using pandx.Wheel.Modules;
using pandx.Wheel.Persistence.EntityFrameworkCore;

namespace Sample.EntityFrameworkCore;

public class SampleEntityFrameworkCoreModule : IModule
{
    public void Initialize(WebApplicationBuilder builder)
    {
        builder.Services.AddServicesByConvention(typeof(SampleEntityFrameworkCoreModule).Assembly);
        //entityFrameworkCore
        builder.Services.AddDbContext<SampleDbContext>(options =>
        {
            //此处为了兼容sqlserver2014版本，设置了兼容级别为120
            //https://learn.microsoft.com/zh-cn/ef/core/what-is-new/ef-core-8.0/breaking-changes#sqlserver-contains-compatibility
            options.UseSqlServer(builder.Configuration.GetConnectionString("Default"),
                o => o.UseCompatibilityLevel(120));
        });
        builder.Services.AddTransient(typeof(IRepository<,>), typeof(SampleRepository<,>));
        builder.Services.AddTransient(typeof(IRepository<>), typeof(SampleRepository<>));
        builder.Services.AddScoped(typeof(IUnitOfWork), typeof(EfCoreUnitOfWork<SampleDbContext>));
    }
}