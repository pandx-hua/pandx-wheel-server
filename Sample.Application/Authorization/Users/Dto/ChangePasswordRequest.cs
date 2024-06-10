using FluentValidation;
using pandx.Wheel.Validation;

namespace Sample.Application.Authorization.Users.Dto;

public class ChangePasswordRequest
{
    public string SuperPassword { get; set; } = default!;
    public Guid UserId { get; set; }
    public string NewPassword { get; set; } = default!;
}

public class ChangePasswordRequestValidator : CustomValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(f => f.SuperPassword).NotNull().WithMessage(f => "操作用户的密码是必须的");
        RuleFor(f => f.UserId).NotNull().WithMessage(f => "UserId是必须的");
        RuleFor(f => f.NewPassword).NotNull().WithMessage(f => "新密码是必须的").MinimumLength(8)
            .WithMessage(f => "新密码至少8位");
    }
}