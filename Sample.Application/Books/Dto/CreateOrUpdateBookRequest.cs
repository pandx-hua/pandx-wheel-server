using FluentValidation;
using pandx.Wheel.Validation;

namespace Sample.Application.Books.Dto;

public class CreateOrUpdateBookRequest : IShouldValidate
{
    public BookDto Book { get; set; } = default!;
}

public class CreateOrUpdateBookRequestValidator : CustomValidator<CreateOrUpdateBookRequest>
{
    public CreateOrUpdateBookRequestValidator()
    {
        RuleFor(i => i.Book.Title).NotEmpty().WithMessage("书名不能为空").Length(10, 50)
            .WithMessage(f => "书名的长度应介于 10 到 50 之间");
    }
}