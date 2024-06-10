namespace Sample.Application.Organizations.Dto;

public class UsersToOrganizationRequest
{
    public Guid[] UserIds { get; set; } = default!;
    public Guid OrganizationId { get; set; }
}