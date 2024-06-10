using pandx.Wheel.Application.Dto;
using pandx.Wheel.Application.Services;
using pandx.Wheel.Models;
using pandx.Wheel.Storage;
using Sample.Application.Authorization.Users.Dto;

namespace Sample.Application.Authorization.Users;

public interface IUserAppService : IApplicationService
{
    Task<PagedResponse<UsersDto>> GetPagedUsersAsync(GetUsersRequest request);
    Task CreateOrUpdateUserAsync(CreateOrUpdateUserRequest request);
    Task DeleteUserAsync(EntityDto<Guid> request);
    Task<int> BatchDeleteUsersAsync(BatchDeleteUsersRequest request);
    Task UnlockUserAsync(EntityDto<Guid> request);
    Task<bool> ChangePasswordAsync(ChangePasswordRequest request);
    Task<ValidationResponse> ValidateUserNameAsync(ValidationRequest<string, string> request);
    Task<ValidationResponse> ValidateEmailAsync(ValidationRequest<string, string> request);
    Task ImportUsersFromExcelAsync();
    Task<CachedFile> ExportUsersToExcelAsync(GetUsersRequest request);
    Task<string> GetAvatarByIdAsync(EntityDto<Guid> request);
}