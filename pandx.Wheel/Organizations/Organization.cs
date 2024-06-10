using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.IdentityModel.Tokens;
using pandx.Wheel.Domain.Entities;
using pandx.Wheel.Extensions;

namespace pandx.Wheel.Organizations;

public class Organization : AuditedEntity<Guid>
{
    public const int MaxDepth = 16;
    public const int CodeUnitLength = 5;
    public const int MaxCodeLength = MaxDepth * (CodeUnitLength + 1) - 1;
    [ForeignKey(nameof(ParentId))] public Organization Parent { get; set; } = default!;
    public Guid? ParentId { get; set; }

    [Required]
    [StringLength(MaxCodeLength)]
    public string Code { get; set; } = default!;

    [Required] [StringLength(128)] public string DisplayName { get; set; } = default!;

    [StringLength(256)] public string? Address { get; set; }
    [StringLength(16)] public string? Phone { get; set; }
    [StringLength(16)] public string? Head { get; set; }
    public ICollection<Organization> Children { get; set; } = default!;

    public static string CreateCode(params int[] numbers)
    {
        if (numbers.IsNullOrEmpty())
        {
            return string.Empty;
        }

        return numbers.Select(n => n.ToString(new string('0', CodeUnitLength))).JoinAsString(".");
    }

    public static string AppendCode(string parentCode, string childCode)
    {
        if (childCode.IsNullOrEmpty())
        {
            throw new ArgumentNullException(nameof(childCode), "ChildCode不能为空或null");
        }

        if (parentCode.IsNullOrEmpty())
        {
            return childCode;
        }

        return $"{parentCode}.{childCode}";
    }

    public static string GetRelativeCode(string code, string parentCode)
    {
        if (code.IsNullOrEmpty())
        {
            throw new ArgumentNullException(nameof(code), "Code不能为空或null");
        }

        if (parentCode.IsNullOrEmpty())
        {
            return code;
        }

        if (code.Length == parentCode.Length)
        {
            return string.Empty;
        }

        return code.Substring(parentCode.Length + 1);
    }

    public static string CalculateNextCode(string code)
    {
        if (code.IsNullOrEmpty())
        {
            throw new ArgumentNullException(nameof(code), "Code不能为空或null");
        }

        var parentCode = GetParentCode(code);
        var lastUnitCode = GetLastUnitCode(code);

        return AppendCode(parentCode, CreateCode(Convert.ToInt32(lastUnitCode) + 1));
    }

    public static string GetParentCode(string code)
    {
        if (code.IsNullOrEmpty())
        {
            throw new ArgumentNullException(nameof(code), "Code不能为空或null");
        }

        var splitCode = code.Split('.');
        if (splitCode.Length == 1)
        {
            return string.Empty;
        }

        return splitCode.Take(splitCode.Length - 1).JoinAsString(".");
    }

    public static string GetLastUnitCode(string code)
    {
        if (code.IsNullOrEmpty())
        {
            throw new ArgumentNullException(nameof(code), "Code不能为空或null");
        }

        var splitCode = code.Split('.');
        return splitCode[^1];
    }
}