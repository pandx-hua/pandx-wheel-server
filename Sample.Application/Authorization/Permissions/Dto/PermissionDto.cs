namespace Sample.Application.Authorization.Permissions.Dto;

public class PermissionDto
{
    public string ParentResource { get; set; } = default!;
    public string ParentAction { get; set; } = default!;
    public string Resource { get; set; } = default!;
    public string Action { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Value { get; set; } = default!;
}