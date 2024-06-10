namespace Sample.Application.Authorization.Users.Dto;

public class BatchDeleteUsersRequest
{
    public Guid[] UserIds { get; set; } = default!;
}