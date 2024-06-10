using pandx.Wheel.Models;
using pandx.Wheel.Validation;
using Sample.Domain;

namespace Sample.Application.Auditing.Dto;

public class GetAuditingRequest : PagedRequest, ISortedRequest, IShouldNormalize
{
    public GetAuditingRequest()
    {
        PageSize = SampleConsts.DefaultPageSize;
        StartTime = new DateTime(1900, 1, 1, 0, 0, 0);
        EndTime = new DateTime(2099, 12, 31, 23, 59, 59);
    }

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public List<bool> HasException { get; set; } = [true, false];
    public string? UserName { get; set; }

    public string? Controller { get; set; }

    public string? Action { get; set; }
    public string? ClientIpAddress { get; set; }

    public void Normalize()
    {
        if (string.IsNullOrEmpty(Sorting))
        {
            Sorting = "Auditing.ExecutionTime DESC";
        }
        else if (Sorting.Contains("UserName", StringComparison.OrdinalIgnoreCase))
        {
            Sorting = Sorting.Replace("UserName", "User.UserName", StringComparison.OrdinalIgnoreCase);
        }
        else if (Sorting.Contains("ExecutionTime", StringComparison.OrdinalIgnoreCase))
        {
            Sorting = Sorting.Replace("ExecutionTime", "Auditing.ExecutionTime", StringComparison.OrdinalIgnoreCase);
        }
        else if (Sorting.Contains("Controller", StringComparison.OrdinalIgnoreCase))
        {
            Sorting = Sorting.Replace("Controller", "Auditing.Controller", StringComparison.OrdinalIgnoreCase);
        }
        else if (Sorting.Contains("Action", StringComparison.OrdinalIgnoreCase))
        {
            Sorting = Sorting.Replace("Action", "Auditing.Action", StringComparison.OrdinalIgnoreCase);
        }
        else if (Sorting.Contains("ExecutionDuration", StringComparison.OrdinalIgnoreCase))
        {
            Sorting = Sorting.Replace("ExecutionDuration", "Auditing.ExecutionDuration",
                StringComparison.OrdinalIgnoreCase);
        }
        else if (Sorting.Contains("ClientName", StringComparison.OrdinalIgnoreCase))
        {
            Sorting = Sorting.Replace("ClientName", "Auditing.ClientName", StringComparison.OrdinalIgnoreCase);
        }
        else if (Sorting.Contains("ClientIpAddress", StringComparison.OrdinalIgnoreCase))
        {
            Sorting = Sorting.Replace("ClientIpAddress", "Auditing.ClientIpAddress",
                StringComparison.OrdinalIgnoreCase);
        }
    }

    public string? Sorting { get; set; }
}