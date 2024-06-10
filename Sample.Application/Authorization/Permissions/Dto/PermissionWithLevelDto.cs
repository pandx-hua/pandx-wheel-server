namespace Sample.Application.Authorization.Permissions.Dto;

public class PermissionWithLevelDto : PermissionDto
{
    public int Level { get; set; }
    public bool Leaf { get; set; }
}