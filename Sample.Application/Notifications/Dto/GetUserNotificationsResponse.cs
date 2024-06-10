using pandx.Wheel.Models;

namespace Sample.Application.Notifications.Dto;

public class GetUserNotificationsResponse : PagedResponse<UserNotificationsDto>
{
    public GetUserNotificationsResponse(int totalCount, int unReadCount, IReadOnlyList<UserNotificationsDto> items) :
        base(totalCount, items)
    {
        UnReadCount = unReadCount;
    }

    public int UnReadCount { get; set; }
}