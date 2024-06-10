using pandx.Wheel.Menus;
using Sample.Domain.Authorization.Permissions;

namespace Sample.Domain.Menus;

public class SampleMenuProvider : MenuProvider
{
    public override Task SetMenusAsync(IMenuContext context)
    {
        var home = context.Manager.GetMenu("name") ?? context.Manager.CreateMenu("/home/index", "home",
            $"Permission.{SamplePermissions.Resources.Pages}.{SamplePermissions.Actions.Empty}",
            new Meta("Grid", "仪表板", "", false, false, true, true),
            "Home");
        //只是一个分组作用，没有具体的组件对应
        var administration = context.Manager.GetMenu("administration") ?? context.Manager.CreateMenu("/administration",
            "administration",
            $"Permission.{SamplePermissions.Resources.Administration}.{SamplePermissions.Actions.Empty}",
            new Meta("Setting", "系统管理", "", false, false, false, true)
        );
        var users = administration.CreateChildMenu("/administration/users/index", "users",
            $"Permission.{SamplePermissions.Resources.Users}.{SamplePermissions.Actions.Search}",
            new Meta("Menu", "用户", "", false, false, false, true),
            "Administration/Users");
        var roles = administration.CreateChildMenu("/administration/roles/index", "roles",
            $"Permission.{SamplePermissions.Resources.Roles}.{SamplePermissions.Actions.Search}",
            new Meta("Menu", "角色", "", false, false, false, true),
            "Administration/Roles");
        var organizations = administration.CreateChildMenu("/administration/organizations/index", "organizations",
            $"Permission.{SamplePermissions.Resources.Organizations}.{SamplePermissions.Actions.Search}",
            new Meta("Menu", "部门", "", false, false, false, true),
            "Administration/Organizations");
        var dictionaries = administration.CreateChildMenu("/administration/dictionaries/index", "dictionaries",
            $"Permission.{SamplePermissions.Resources.Dictionaries}.{SamplePermissions.Actions.Search}",
            new Meta("Menu", "字典", "", false, false, false, true),
            "Administration/Dictionaries");
        var forms = administration.CreateChildMenu("/administration/forms/index", "forms",
            $"Permission.{SamplePermissions.Resources.Forms}.{SamplePermissions.Actions.Search}",
            new Meta("Menu", "表单", "", false, false, false, true),
            "Administration/Forms");
        var flows = administration.CreateChildMenu("/administration/flows/index", "flows",
            $"Permission.{SamplePermissions.Resources.Flows}.{SamplePermissions.Actions.Search}",
            new Meta("Menu", "流程", "", false, false, false, true),
            "Administration/Flows");
        var maintenance = administration.CreateChildMenu("/administration/maintenance/index", "maintenance",
            $"Permission.{SamplePermissions.Resources.Logs}.{SamplePermissions.Actions.Search}",
            new Meta("Menu", "维护", "", false, false, false, true),
            "Administration/Maintenance");
        var settings = administration.CreateChildMenu("/administration/jobs/index", "jobs",
            $"Permission.{SamplePermissions.Resources.Jobs}.{SamplePermissions.Actions.Search}",
            new Meta("Menu", "定时任务", "", false, false, false, true),
            "Administration/Jobs");
        var logs = administration.CreateChildMenu("/administration/auditing/index", "auditing",
            $"Permission.{SamplePermissions.Resources.Auditing}.{SamplePermissions.Actions.Search}",
            new Meta("Menu", "审计日志", "", false, false, false, true),
            "Administration/Auditing");
        return Task.CompletedTask;
    }
}