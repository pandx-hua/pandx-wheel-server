namespace pandx.Wheel.Auditing;

public class AuditingSettings
{
    public bool IsEnabled { get; set; } = true;
    public bool IsEnabledForAnonymousUsers { get; set; } = true;
    public bool SaveReturnValues { get; set; } = false;
    public bool SaveExceptions { get; set; } = false;
    public int ExpiredAfterDays { get; set; } = 30;
}