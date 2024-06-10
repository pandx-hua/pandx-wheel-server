namespace Sample.Application.Organizations.Dto;

public class UserToOrganizationRequest
{
    public Guid UserId { get; set; }
    public Guid OrganizationId { get; set; }
}