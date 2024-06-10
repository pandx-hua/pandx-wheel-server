using pandx.Wheel.DependencyInjection;
using pandx.Wheel.Storage;
using Sample.Application.Authorization.Users.Dto;

namespace Sample.Application.Authorization.Users.Exporting;

public interface IUsersExcelExporter : ITransientDependency
{
    Task<CachedFile> ExportToExcelAsync(List<UsersDto> dtos);
}