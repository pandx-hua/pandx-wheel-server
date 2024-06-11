using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pandx.Wheel.Authorization;
using pandx.Wheel.BackgroundJobs;
using pandx.Wheel.Controllers;
using pandx.Wheel.Exceptions;
using pandx.Wheel.Notifications;
using Sample.Domain;
using Sample.Domain.Books;
using Sample.Domain.Notifications;

namespace Sample.Host.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("[controller]/[action]")]
public class DefaultController : WheelControllerBase
{
    private readonly IBackgroundJobLauncher _backgroundJobLauncher;
    private readonly IBackgroundJobManager _backgroundJobManager;
    private readonly INotificationPublisher _notificationPublisher;
    private readonly INotificationSubscriptionManager _notificationSubscriptionManager;

    public DefaultController(INotificationPublisher notificationPublisher,
        INotificationSubscriptionManager notificationSubscriptionManager, IBackgroundJobManager backgroundJobManager,
        IBackgroundJobLauncher backgroundJobLauncher)
    {
        _notificationPublisher = notificationPublisher;
        _notificationSubscriptionManager = notificationSubscriptionManager;
        _backgroundJobManager = backgroundJobManager;
        _backgroundJobLauncher = backgroundJobLauncher;
    }

    [HttpGet(Name = "PublishNotification")]
    public async Task PublishNotification()
    {
        var notificationData = new NotificationData();
        notificationData["Message"] = "这是一条测试消息";
        await _notificationPublisher.PublishAsync(SampleNotificationNames.Test, notificationData,
            NotificationSeverity.Success);
    }

    [HttpGet(Name = "Subscribe")]
    public async Task Subscribe(string userId)
    {
        await _notificationSubscriptionManager.SubscribeToAllNotificationsAsync(new UserIdentifier(userId));
    }


    [HttpPost]
    public Task TestWheel()
    {
        throw new WheelException("这是专门抛出的一个Wheel异常，用于测试");
    }

    [HttpPost]
    public Task TestCommon()
    {
        throw new Exception("这是专门抛出的一个Common异常，用于测试");
    }

    [HttpPost]
    public Task Common()
    {
        _backgroundJobLauncher.StartAsync<TestBackgroundJob>(new BackgroundJobData
        {
            Id = Guid.NewGuid().ToString()
        });
        return Task.CompletedTask;
    }

    [HttpPost]
    public Task<JsonResult> TestJson()
    {
        return Task.FromResult(new JsonResult(new Book { Title = "abc", Author = "def" }));
    }
}