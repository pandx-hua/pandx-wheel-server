using FluentValidation;
using pandx.Wheel.Validation;

namespace Sample.Application.Personal.Dto;

public class ChangePersonalPasswordRequest
{
    public string CurrentPassword { get; set; } = default!;
    public string NewPassword { get; set; } = default!;
}

public class ChangePasswordRequestValidator : CustomValidator<ChangePersonalPasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(f => f.CurrentPassword).NotNull().WithMessage(f => "当前密码是必须的");
        RuleFor(f => f.NewPassword).NotNull().WithMessage(f => "新密码是必须的").MinimumLength(8)
            .WithMessage(f => "新密码至少8位");
    }
}