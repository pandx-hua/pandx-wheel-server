using System.ComponentModel.DataAnnotations;
using pandx.Wheel.Domain.Entities;

namespace pandx.Wheel.Storage;

public class BinaryObject:IEntity<Guid>
{
    public Guid Id { get; set; }
    public byte[] Bytes { get; set; }= default!;
    [StringLength(64)]
    public string? ContentType { get; set; }
    [StringLength(64)]
    public string? FileName { get; set; }
    public long Length { get; set; }
    [StringLength(512)]
    public string Description { get; set; }= default!;
    public DateTime CreationTime { get; set; } = DateTime.Now;
}