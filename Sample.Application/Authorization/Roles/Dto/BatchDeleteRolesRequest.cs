namespace Sample.Application.Authorization.Roles.Dto;

public class BatchDeleteRolesRequest
{
    public Guid[] RoleIds { get; set; } = default!;
}