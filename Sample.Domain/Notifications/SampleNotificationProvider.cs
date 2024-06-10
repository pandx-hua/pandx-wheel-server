using pandx.Wheel.Notifications;

namespace Sample.Domain.Notifications;

public class SampleNotificationProvider : NotificationProvider
{
    public override Task SetNotificationsAsync(INotificationDefinitionContext context)
    {
        context.Manager.Add(new NotificationDefinition(SampleNotificationNames.Test, "This is a Test"));
        context.Manager.Add(new NotificationDefinition(SampleNotificationNames.InvalidExcel, "导入用户的Excel文件未通过验证"));
        context.Manager.Add(new NotificationDefinition(SampleNotificationNames.InvalidUsers, "导入用户的Excel文件中有无效用户"));
        context.Manager.Add(new NotificationDefinition(SampleNotificationNames.CommonMessage, "通用消息"));
        return Task.CompletedTask;
    }
}