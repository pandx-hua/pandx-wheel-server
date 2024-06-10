using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using pandx.Wheel.DependencyInjection;
using pandx.Wheel.Modules;
using pandx.Wheel.Persistence.EntityFrameworkCore;

namespace pandx.Wheel.Persistence;

public class PersistenceModule : IModule
{
    public void Initialize(WebApplicationBuilder builder)
    {
        builder.Services.AddServicesByConvention(typeof(PersistenceModule).Assembly);
        builder.Services.Configure<MvcOptions>(options => { options.Filters.Add<EfCoreUnitOfWorkFilter>(); });
    }
}