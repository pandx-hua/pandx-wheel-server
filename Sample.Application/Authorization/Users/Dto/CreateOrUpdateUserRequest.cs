using FluentValidation;
using pandx.Wheel.Validation;

namespace Sample.Application.Authorization.Users.Dto;

public class CreateOrUpdateUserRequest : IShouldValidate
{
    public UserDto User { get; set; } = default!;
    public List<string> AssignedRoleNames { get; set; } = default!;
    public List<Guid> AssignedOrganizationIds { get; set; } = default!;
}

public class CreateOrUpdateUserRequestValidator : CustomValidator<CreateOrUpdateUserRequest>
{
    public CreateOrUpdateUserRequestValidator()
    {
        RuleFor(f => f.User.Name).NotNull().WithMessage("姓名不能为空")
            .Length(3, 10).WithMessage("姓名长度应介于 3 到 10 之间");
    }
}