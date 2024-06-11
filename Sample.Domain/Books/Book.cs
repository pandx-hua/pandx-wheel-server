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