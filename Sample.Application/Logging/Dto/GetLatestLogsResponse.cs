namespace Sample.Application.Logging.Dto;

public class GetLatestLogsResponse
{
    public List<string> LatestLogLines { get; set; } = default!;
}