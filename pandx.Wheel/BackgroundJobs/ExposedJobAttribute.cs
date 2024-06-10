namespace pandx.Wheel.BackgroundJobs;

[AttributeUsage(AttributeTargets.Class)]
public class ExposedJobAttribute : Attribute
{
    public ExposedJobAttribute(string description)
    {
        Description = description;
    }

    public string Description { get; set; }
}