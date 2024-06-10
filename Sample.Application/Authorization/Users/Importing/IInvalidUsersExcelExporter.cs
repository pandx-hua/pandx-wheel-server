using pandx.Wheel.DependencyInjection;
using pandx.Wheel.Storage;
using Sample.Application.Authorization.Users.Importing.Dto;

namespace Sample.Application.Authorization.Users.Importing;

public interface IInvalidUsersExcelExporter : ITransientDependency
{
    Task<CachedFile> ExportToExcelAsync(List<ImportedUserDto> dtos);
}