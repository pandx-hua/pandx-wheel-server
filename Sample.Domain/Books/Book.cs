using System.ComponentModel.DataAnnotations;
using FluentValidation;
using pandx.Wheel.Domain.Entities;
using pandx.Wheel.Validation;

namespace Sample.Domain.Books;

public class Book : AuditedEntity<long>
{
    [StringLength(256)] public string Title { get; set; } = default!;

    [StringLength(256)] public string Author { get; set; } = default!;
}
//
// public class BookValidator : CustomValidator<Book>
// {
//     public BookValidator()
//     {
//         RuleFor(f => f.Title).NotNull().WithMessage(f => "书名不能为空")
//             .Length(10, 50).WithMessage(f => "书名的长度应介于 3 到 10 之间");
//         RuleFor(f => f.Author).NotNull().WithMessage(f => "作者名不能为空")
//             .Length(10, 50).WithMessage(f => "作者名的长度应介于 3 到 10 之间");
//     }
// }