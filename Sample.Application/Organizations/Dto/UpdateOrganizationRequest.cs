using FluentValidation;
using pandx.Wheel.Validation;

namespace Sample.Application.Organizations.Dto;

public class UpdateOrganizationRequest : IShouldValidate
{
    public Guid Id { get; set; }
    public string DisplayName { get; set; } = default!;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Head { get; set; }
}

public class UpdateOrganizationRequestValidator : CustomValidator<UpdateOrganizationRequest>
{
    public UpdateOrganizationRequestValidator()
    {
        RuleFor(f => f.DisplayName).NotNull().WithMessage(f => "部门名称不能为空")
            .Length(3, 128).WithMessage(f => "部门名称的长度应该介于 3 到 128 之间");
    }
}