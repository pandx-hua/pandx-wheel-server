using System.ComponentModel.DataAnnotations;
using pandx.Wheel.Domain.Entities;

namespace Sample.Domain.Books;

public class Book : AuditedEntity<long>
{
    [StringLength(256)] public string Title { get; set; } = default!;

    [StringLength(256)] public string Author { get; set; } = default!;
}