using Microsoft.Extensions.Logging;
using pandx.Wheel.Auditing;
using pandx.Wheel.Domain.Repositories;
using pandx.Wheel.Domain.UnitOfWork;
using pandx.Wheel.Miscellaneous;

namespace pandx.Wheel.Authorization.Logins;

public class LoginService : ILoginService
{
    private readonly IClientInfoProvider _clientInfoProvider;
    private readonly ILogger<AuditingHelper> _logger;
    private readonly IRepository<LoginAttempt, Guid> _loginAttemptRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LoginService(IRepository<LoginAttempt, Guid> loginAttemptRepository, IClientInfoProvider clientInfoProvider,
        ILogger<AuditingHelper> logger, IUnitOfWork unitOfWork)
    {
        _loginAttemptRepository = loginAttemptRepository;
        _clientInfoProvider = clientInfoProvider;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task CreateLoginAttemptAsync(string userNameOrEmail, LoginResultType loginResultType,
        Guid? userId = null)
    {
        var loginAttempt = new LoginAttempt
        {
            UserId = userId,
            UserNameOrEmail = userNameOrEmail,
            ClientIpAddress = _clientInfoProvider.ClientIpAddress,
            BrowserInfo = _clientInfoProvider.BrowserInfo,
            Result = loginResultType
        };
        await _loginAttemptRepository.InsertAsync(loginAttempt);
        await _unitOfWork.CommitAsync();
    }
}