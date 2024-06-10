using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using pandx.Wheel.Authorization;
using pandx.Wheel.Controllers;
using pandx.Wheel.Domain.Repositories;
using pandx.Wheel.Domain.UnitOfWork;
using pandx.Wheel.Miscellaneous;

namespace pandx.Wheel.Auditing;

public class AuditingHelper : IAuditingHelper
{
    private readonly IRepository<AuditingInfo, Guid> _auditingInfoRepository;
    private readonly AuditingSettings _auditingSettings;
    private readonly IClientInfoProvider _clientInfoProvider;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<AuditingHelper> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public AuditingHelper(IRepository<AuditingInfo, Guid> auditingInfoRepository,
        ICurrentUser currentUser,
        IOptions<AuditingSettings> auditingSettings,
        IUnitOfWork unitOfWork,
        IClientInfoProvider clientInfoProvider,
        ILogger<AuditingHelper> logger)
    {
        _auditingInfoRepository = auditingInfoRepository;
        _currentUser = currentUser;
        _logger = logger;
        _auditingSettings = auditingSettings.Value;
        _unitOfWork = unitOfWork;
        _clientInfoProvider = clientInfoProvider;
    }

    public Stopwatch Stopwatch { get; set; } = default!;
    public AuditingInfo AuditingInfo { get; set; } = default!;

    public bool ShouldSave(ActionExecutingContext context)
    {
        if (!context.ActionDescriptor.IsControllerActionDescriptor())
        {
            return false;
        }

        if (!_auditingSettings.IsEnabled)
        {
            return false;
        }

        if (!_auditingSettings.IsEnabledForAnonymousUsers && !_currentUser.IsAuthenticated())
        {
            return false;
        }

        if (context.ActionDescriptor.AsControllerActionDescriptor().MethodInfo
            .IsDefined(typeof(NoAuditedAttribute), true))
        {
            return false;
        }

        return true;
    }

    public AuditingInfo CreateAuditing(ActionExecutingContext context)
    {
        var auditingInfo = new AuditingInfo
        {
            UserId = _currentUser.GetUserId(),
            Controller = context.Controller.GetType().Name,
            Action = context.ActionDescriptor.AsControllerActionDescriptor().ActionName,
            Parameters = ConvertArgumentsToJson(context.ActionArguments),
            ExecutionTime = DateTime.Now,
            BrowserInfo = _clientInfoProvider.BrowserInfo,
            ClientIpAddress = _clientInfoProvider.ClientIpAddress,
            ClientName = _clientInfoProvider.ComputerName
        };
        return auditingInfo;
    }

    public AuditingInfo CreateAuditing(ResultExecutingContext context)
    {
        var auditingInfo = new AuditingInfo
        {
            UserId = _currentUser.GetUserId(),
            Controller = context.Controller.GetType().Name,
            Action = context.ActionDescriptor.AsControllerActionDescriptor().ActionName,
            //TODO 如何才能获取到请求的参数呢？
            // Parameters = GetJsonFromContext(context),
            Parameters = "{}",
            ExecutionTime = DateTime.Now,
            BrowserInfo = _clientInfoProvider.BrowserInfo,
            ClientIpAddress = _clientInfoProvider.ClientIpAddress,
            ClientName = _clientInfoProvider.ComputerName
        };
        return auditingInfo;
    }


    public async Task Save(AuditingInfo auditingInfo)
    {
        try
        {
            await _auditingInfoRepository.InsertAsync(auditingInfo);
            await _unitOfWork.CommitAsync();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex.ToString(), ex);
        }
    }

    private string GetJsonFromContext(ResultExecutingContext context)
    {
        context.HttpContext.Request.EnableBuffering();

        // Leave the body open so the next middleware can read it.
        using var reader = new StreamReader(
            context.HttpContext.Request.Body,
            Encoding.UTF8,
            false,
            1024,
            true);

        var body = reader.ReadToEndAsync().Result;

        // Reset the request body stream position so the next middleware can read it
        context.HttpContext.Request.Body.Position = 0;
        return body;
    }

    private string ConvertArgumentsToJson(IDictionary<string, object?> arguments)
    {
        if (arguments == null)
        {
            throw new ArgumentNullException(nameof(arguments));
        }

        try
        {
            if (arguments.IsNullOrEmpty())
            {
                return "{}";
            }

            return JsonConvert.SerializeObject(arguments);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex.ToString(), ex);
            return "{}";
        }
    }
}