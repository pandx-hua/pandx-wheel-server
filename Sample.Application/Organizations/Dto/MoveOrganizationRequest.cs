namespace Sample.Application.Organizations.Dto;

public class MoveOrganizationRequest
{
    public Guid Id { get; set; }
    public Guid? NewId { get; set; }
    public string Position { get; set; } = default!;
}