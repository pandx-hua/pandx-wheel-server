namespace Sample.Application.Authorization.Roles.Dto;

public class RoleDto
{
    public string? Id { get; set; }
    public string DisplayName { get; set; } = default!;
    public string? Description { get; set; } = default!;
    public bool IsDefault { get; set; }
}