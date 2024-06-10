using Microsoft.IdentityModel.Tokens;
using pandx.Wheel.Domain.UnitOfWork;
using pandx.Wheel.Extensions;

namespace pandx.Wheel.Notifications;

public class NotificationPublisher : INotificationPublisher
{
    private readonly INotificationDistributor _notificationDistributor;
    private readonly INotificationManager _notificationManager;
    private readonly IUnitOfWork _unitOfWork;

    public NotificationPublisher(
        INotificationManager notificationManager,
        INotificationDistributor notificationDistributor,
        IUnitOfWork unitOfWork
    )
    {
        _notificationManager = notificationManager;
        _notificationDistributor = notificationDistributor;
        _unitOfWork = unitOfWork;
    }

    public async Task PublishAsync(string notificationName, NotificationData? data = null,
        NotificationSeverity severity = NotificationSeverity.Info, Guid[]? userIds = null,
        Guid[]? excludedUserIds = null)
    {
        if (notificationName.IsNullOrEmpty())
        {
            throw new ArgumentException("NotificationName 不能为空", nameof(notificationName));
        }

        var notification = new Notification
        {
            NotificationName = notificationName,
            NotificationData = data?.Properties.ToJsonString(true),
            Severity = severity,
            UserIds = userIds.IsNullOrEmpty() ? null : userIds!.Select(x => x.ToString()).JoinAsString(","),
            ExcludedUserIds = excludedUserIds.IsNullOrEmpty()
                ? null
                : excludedUserIds!.Select(x => x.ToString()).JoinAsString(",")
        };

        await _notificationManager.InsertNotificationAsync(notification);
        await _unitOfWork.CommitAsync();

        if (userIds is not null)
        {
            await _notificationDistributor.DistributeAsync(notification.Id);
        }
    }
}