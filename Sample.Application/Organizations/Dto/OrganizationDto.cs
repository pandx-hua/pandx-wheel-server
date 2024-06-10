using pandx.Wheel.Application.Dto;
using pandx.Wheel.Domain.Entities;

namespace Sample.Application.Organizations.Dto;

public class OrganizationDto : EntityDto<Guid>, IHasCreationTime
{
    public Guid? ParentId { get; set; }
    public string Code { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public string Address { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public string Head { get; set; } = default!;
    public int MemberCount { get; set; }
    public DateTime CreationTime { get; set; }
}