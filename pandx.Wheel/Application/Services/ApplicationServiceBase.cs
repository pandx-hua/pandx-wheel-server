using AutoMapper;
using pandx.Wheel.Authorization;
using pandx.Wheel.Caching;
using pandx.Wheel.DependencyInjection;

namespace pandx.Wheel.Application.Services;

public abstract class ApplicationServiceBase : ServiceBase, IApplicationService
{
    [Injection] public IMapper Mapper { get; set; } = default!;
    [Injection] public ICurrentUser CurrentUser { get; set; } = default!;

    [Injection] public ICacheService CacheService { get; set; } = default!;

    [Injection] public ICacheKeyService CacheKeyService { get; set; } = default!;
}