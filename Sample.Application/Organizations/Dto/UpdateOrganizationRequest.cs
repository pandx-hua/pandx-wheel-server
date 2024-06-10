namespace Sample.Application.Organizations.Dto;

public class UpdateOrganizationRequest
{
    public Guid Id { get; set; }
    public string DisplayName { get; set; } = default!;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Head { get; set; }
}