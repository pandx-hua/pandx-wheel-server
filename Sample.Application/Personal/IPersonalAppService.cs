using pandx.Wheel.Application.Services;
using pandx.Wheel.DependencyInjection;
using pandx.Wheel.Models;
using Sample.Application.Personal.Dto;

namespace Sample.Application.Personal;

public interface IPersonalAppService : IApplicationService
{
    Task<PersonalResponse> GetPersonalAsync();
    Task<PersonalDto> UpdateUserAsync(UpdatePersonalRequest request);
    Task<bool> ChangePasswordAsync(ChangePersonalPasswordRequest request);
    Task<ListResponse<LoginAttemptDto>> GetLoginAttemptsAsync();
    Task UploadAvatarAsync();
    Task<string> GetAvatarAsync();
}