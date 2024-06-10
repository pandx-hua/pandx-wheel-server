using pandx.Wheel.DependencyInjection;
using Sample.Application.Authorization.Users.Importing.Dto;

namespace Sample.Application.Authorization.Users.Importing;

public interface IUsersExcelImporter : ITransientDependency
{
    List<ImportedUserDto> GetUsersFromExcel(byte[] fileBytes);
}