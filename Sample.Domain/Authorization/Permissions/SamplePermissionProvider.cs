using pandx.Wheel.Authorization.Permissions;

namespace Sample.Domain.Authorization.Permissions;

public class SamplePermissionProvider : PermissionProvider
{
    public override Task SetPermissionsAsync(IPermissionContext context)
    {
        var pages = context.Manager.GetPermission(SamplePermissions.Resources.Pages, SamplePermissions.Actions.Empty) ??
                    context.Manager.CreatePermission(SamplePermissions.Resources.Pages, SamplePermissions.Actions.Empty,
                        "页面", "这里是一个相关权限的描述信息");
        //books
        var books = pages.CreateChildPermission(SamplePermissions.Resources.Books, SamplePermissions.Actions.Empty,
            "图书");
        books.CreateChildPermission(SamplePermissions.Resources.Books, SamplePermissions.Actions.Search, "查询图书");
        books.CreateChildPermission(SamplePermissions.Resources.Books, SamplePermissions.Actions.Browse, "浏览图书");
        books.CreateChildPermission(SamplePermissions.Resources.Books, SamplePermissions.Actions.Create, "新建图书");
        books.CreateChildPermission(SamplePermissions.Resources.Books, SamplePermissions.Actions.Update, "编辑图书");
        books.CreateChildPermission(SamplePermissions.Resources.Books, SamplePermissions.Actions.Delete, "删除图书");
        //administration
        var administration = pages.CreateChildPermission(SamplePermissions.Resources.Administration,
            SamplePermissions.Actions.Empty, "系统管理");
        var users = administration.CreateChildPermission(SamplePermissions.Resources.Users,
            SamplePermissions.Actions.Empty, "用户");
        users.CreateChildPermission(SamplePermissions.Resources.Users, SamplePermissions.Actions.Search, "查询用户");
        users.CreateChildPermission(SamplePermissions.Resources.Users, SamplePermissions.Actions.Browse, "浏览用户");
        users.CreateChildPermission(SamplePermissions.Resources.Users, SamplePermissions.Actions.Create, "新建用户");
        users.CreateChildPermission(SamplePermissions.Resources.Users, SamplePermissions.Actions.Update, "编辑用户");
        users.CreateChildPermission(SamplePermissions.Resources.Users, SamplePermissions.Actions.Unlock, "解锁用户");
        users.CreateChildPermission(SamplePermissions.Resources.Users, SamplePermissions.Actions.Delete, "删除用户");
        users.CreateChildPermission(SamplePermissions.Resources.Users, SamplePermissions.Actions.Change, "修改用户密码");
        users.CreateChildPermission(SamplePermissions.Resources.Users, SamplePermissions.Actions.Export, "导出用户");
        users.CreateChildPermission(SamplePermissions.Resources.Users, SamplePermissions.Actions.Import, "导入用户");
        var roles = administration.CreateChildPermission(SamplePermissions.Resources.Roles,
            SamplePermissions.Actions.Empty, "角色");
        roles.CreateChildPermission(SamplePermissions.Resources.Roles, SamplePermissions.Actions.Search, "查询角色");
        roles.CreateChildPermission(SamplePermissions.Resources.Roles, SamplePermissions.Actions.Browse, "浏览角色");
        roles.CreateChildPermission(SamplePermissions.Resources.Roles, SamplePermissions.Actions.Create, "新建角色");
        roles.CreateChildPermission(SamplePermissions.Resources.Roles, SamplePermissions.Actions.Update, "编辑角色");
        roles.CreateChildPermission(SamplePermissions.Resources.Roles, SamplePermissions.Actions.Delete, "删除角色");
        var organizations = administration.CreateChildPermission(SamplePermissions.Resources.Organizations,
            SamplePermissions.Actions.Empty, "部门");
        organizations.CreateChildPermission(SamplePermissions.Resources.Organizations, SamplePermissions.Actions.Search,
            "查询部门");
        organizations.CreateChildPermission(SamplePermissions.Resources.Organizations, SamplePermissions.Actions.Browse,
            "浏览部门");
        organizations.CreateChildPermission(SamplePermissions.Resources.Organizations, SamplePermissions.Actions.Create,
            "新建部门");
        organizations.CreateChildPermission(SamplePermissions.Resources.Organizations, SamplePermissions.Actions.Update,
            "编辑部门");
        organizations.CreateChildPermission(SamplePermissions.Resources.Organizations, SamplePermissions.Actions.Delete,
            "删除部门");
        organizations.CreateChildPermission(SamplePermissions.Resources.Organizations, SamplePermissions.Actions.Move,
            "移动部门");
        organizations.CreateChildPermission(SamplePermissions.Resources.Organizations, SamplePermissions.Actions.Add,
            "添加成员");
        organizations.CreateChildPermission(SamplePermissions.Resources.Organizations, SamplePermissions.Actions.Remove,
            "移除成员");
        var dictionaries = administration.CreateChildPermission(SamplePermissions.Resources.Dictionaries,
            SamplePermissions.Actions.Empty, "字典");
        dictionaries.CreateChildPermission(SamplePermissions.Resources.Dictionaries, SamplePermissions.Actions.Search,
            "查询字典");
        var forms = administration.CreateChildPermission(SamplePermissions.Resources.Forms,
            SamplePermissions.Actions.Empty, "表单");
        forms.CreateChildPermission(SamplePermissions.Resources.Forms, SamplePermissions.Actions.Search, "查询表单");
        var flows = administration.CreateChildPermission(SamplePermissions.Resources.Flows,
            SamplePermissions.Actions.Empty, "流程");
        flows.CreateChildPermission(SamplePermissions.Resources.Flows, SamplePermissions.Actions.Search, "查询流程");
        var maintenance = administration.CreateChildPermission(SamplePermissions.Resources.Maintenance,
            SamplePermissions.Actions.Empty, "维护");
        maintenance.CreateChildPermission(SamplePermissions.Resources.Logs, SamplePermissions.Actions.Search,
            "查询最新运行日志");
        maintenance.CreateChildPermission(SamplePermissions.Resources.Logs, SamplePermissions.Actions.Export,
            "导出全部运行日志");
        maintenance.CreateChildPermission(SamplePermissions.Resources.Caches, SamplePermissions.Actions.Clean,
            "清空全部缓冲");
        var jobs = administration.CreateChildPermission(SamplePermissions.Resources.Jobs,
            SamplePermissions.Actions.Empty, "定时任务");
        jobs.CreateChildPermission(SamplePermissions.Resources.Jobs, SamplePermissions.Actions.Search, "查询定时任务");
        jobs.CreateChildPermission(SamplePermissions.Resources.Jobs, SamplePermissions.Actions.Browse, "浏览定时任务");
        jobs.CreateChildPermission(SamplePermissions.Resources.Jobs, SamplePermissions.Actions.Create, "新建定时任务");
        jobs.CreateChildPermission(SamplePermissions.Resources.Jobs, SamplePermissions.Actions.Update, "编辑定时任务");
        jobs.CreateChildPermission(SamplePermissions.Resources.Jobs, SamplePermissions.Actions.Delete, "删除定时任务");
        jobs.CreateChildPermission(SamplePermissions.Resources.Jobs, SamplePermissions.Actions.Detail, "查看运行日志详情");
        jobs.CreateChildPermission(SamplePermissions.Resources.Jobs, SamplePermissions.Actions.Change, "更改定时任务状态");
        var auditing = administration.CreateChildPermission(SamplePermissions.Resources.Auditing,
            SamplePermissions.Actions.Empty, "审计日志");
        auditing.CreateChildPermission(SamplePermissions.Resources.Auditing, SamplePermissions.Actions.Search,
            "查询审计日志");
        auditing.CreateChildPermission(SamplePermissions.Resources.Auditing, SamplePermissions.Actions.Export,
            "导出审计日志");
        auditing.CreateChildPermission(SamplePermissions.Resources.Auditing, SamplePermissions.Actions.Detail,
            "查看审计日志详情");
        return Task.CompletedTask;
    }
}