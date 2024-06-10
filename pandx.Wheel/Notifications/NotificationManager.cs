using System.Linq.Expressions;
using LinqKit;
using pandx.Wheel.Authorization;
using pandx.Wheel.Domain.Repositories;
using pandx.Wheel.Domain.UnitOfWork;
using pandx.Wheel.Extensions;

namespace pandx.Wheel.Notifications;

public class NotificationManager : INotificationManager
{
    private readonly IRepository<Notification, Guid> _notificationRepository;
    private readonly IRepository<NotificationSubscription, Guid> _notificationSubscriptionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<UserNotification, Guid> _userNotificationRepository;

    public NotificationManager(
        IRepository<Notification, Guid> notificationRepository,
        IRepository<UserNotification, Guid> userNotificationRepository,
        IRepository<NotificationSubscription, Guid> notificationSubscriptionRepository,
        IUnitOfWork unitOfWork)
    {
        _notificationRepository = notificationRepository;
        _userNotificationRepository = userNotificationRepository;
        _notificationSubscriptionRepository = notificationSubscriptionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task InsertNotificationSubscriptionAsync(NotificationSubscription notificationSubscription)
    {
        await _notificationSubscriptionRepository.InsertAsync(notificationSubscription);
        await _unitOfWork.CommitAsync();
    }

    public async Task DeleteNotificationSubscriptionAsync(UserIdentifier userIdentifier, string notificationName)
    {
        await _notificationSubscriptionRepository.DeleteAsync(x =>
            x.NotificationName == notificationName && x.UserId == userIdentifier.UserId);
        await _unitOfWork.CommitAsync();
    }

    public async Task<List<NotificationSubscription>> GetNotificationSubscriptionsAsync(string notificationName)
    {
        return await _notificationSubscriptionRepository.GetAllListAsync(x => x.NotificationName == notificationName);
    }

    public async Task<List<NotificationSubscription>> GetNotificationSubscriptionsAsync(UserIdentifier userIdentifier)
    {
        return await _notificationSubscriptionRepository.GetAllListAsync(x => x.UserId == userIdentifier.UserId);
    }

    public async Task<bool> IsSubscribedAsync(UserIdentifier userIdentifier, string notificationName)
    {
        return await _notificationSubscriptionRepository.CountAsync(x =>
            x.UserId == userIdentifier.UserId && x.NotificationName == notificationName) > 0;
    }

    public async Task InsertNotificationAsync(Notification notification)
    {
        await _notificationRepository.InsertAsync(notification);
        await _unitOfWork.CommitAsync();
    }

    public async Task<Notification?> GetNotificationAsync(Guid notificationId)
    {
        return await _notificationRepository.FirstOrDefaultAsync(notificationId);
    }

    public async Task DeleteNotificationAsync(Guid notificationId)
    {
        await _notificationRepository.DeleteAsync(notificationId);
        await _unitOfWork.CommitAsync();
    }

    public async Task InsertUserNotificationAsync(UserNotification userNotification)
    {
        await _userNotificationRepository.InsertAsync(userNotification);
        await _unitOfWork.CommitAsync();
    }

    public async Task UpdateUserNotificationStateAsync(UserIdentifier userIdentifier, UserNotificationState state,
        Guid userNotificationId)
    {
        var userNotification = await _userNotificationRepository.FirstOrDefaultAsync(x =>
            x.UserId == userIdentifier.UserId && x.Id == userNotificationId);
        if (userNotification is null)
        {
            return;
        }

        userNotification.State = state;
        await _unitOfWork.CommitAsync();
    }

    public async Task UpdateUserNotificationsStateAsync(UserIdentifier userIdentifier, UserNotificationState state)
    {
        var userNotifications =
            await _userNotificationRepository.GetAllListAsync(x => x.UserId == userIdentifier.UserId);
        foreach (var userNotification in userNotifications)
        {
            userNotification.State = state;
        }

        await _unitOfWork.CommitAsync();
    }

    public async Task DeleteUserNotificationAsync(UserIdentifier userIdentifier, Guid userNotificationId)
    {
        var userNotification = await _userNotificationRepository.FirstOrDefaultAsync(x =>
            x.UserId == userIdentifier.UserId && x.Id == userNotificationId);
        if (userNotification is null)
        {
            return;
        }

        await _userNotificationRepository.DeleteAsync(userNotification);
        await _unitOfWork.CommitAsync();
    }

    public async Task DeleteUserNotificationsAsync(UserIdentifier userIdentifier, UserNotificationState? state = null,
        DateTime? startTime = null,
        DateTime? endTime = null)
    {
        var predicate = CreateUserNotificationFilterPredicate(userIdentifier, state, startTime, endTime);
        await _userNotificationRepository.DeleteAsync(predicate);
        await _unitOfWork.CommitAsync();
    }

    public async Task<UserNotification?> GetUserNotificationAsync(UserIdentifier userIdentifier,
        Guid userNotificationId)
    {
        return await _userNotificationRepository.FirstOrDefaultAsync(x =>
            x.UserId == userIdentifier.UserId && x.Id == userNotificationId);
    }

    public async Task<List<UserNotification>> GetUserNotificationsAsync(UserIdentifier userIdentifier,
        UserNotificationState? state = null,
        int currentPage = 1, int pageSize = int.MaxValue, DateTime? startTime = null, DateTime? endTime = null)
    {
        var query = (await _userNotificationRepository.GetAllAsync()).Where(x => x.UserId == userIdentifier.UserId)
            .OrderByDescending(x => x.CreationTime)
            .WhereIf(state.HasValue, x => x.State == state!.Value)
            .WhereIf(startTime.HasValue, x => x.CreationTime >= startTime!.Value)
            .WhereIf(endTime.HasValue, x => x.CreationTime <= endTime!.Value);
        query = query.PageBy(currentPage, pageSize);
        return query.ToList();
    }

    public async Task<int> GetUserNotificationCountAsync(UserIdentifier userIdentifier,
        UserNotificationState? state = null,
        DateTime? startTime = null, DateTime? endTime = null)
    {
        var predicate = CreateUserNotificationFilterPredicate(userIdentifier, state, startTime, endTime);
        return await _userNotificationRepository.CountAsync(predicate);
    }

    private Expression<Func<UserNotification, bool>> CreateUserNotificationFilterPredicate(
        UserIdentifier userIdentifier,
        UserNotificationState? state, DateTime? startTime, DateTime? endTime)
    {
        var predicate = PredicateBuilder.New<UserNotification>();
        predicate = predicate.And(x => x.UserId == userIdentifier.UserId);
        if (state.HasValue)
        {
            predicate = predicate.And(x => x.State == state.Value);
        }

        if (startTime.HasValue)
        {
            predicate = predicate.And(x => x.CreationTime >= startTime.Value);
        }

        if (endTime.HasValue)
        {
            predicate = predicate.And(x => x.CreationTime <= endTime.Value);
        }

        return predicate;
    }
}