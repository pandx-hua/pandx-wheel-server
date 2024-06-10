using pandx.Wheel.Extensions;

namespace pandx.Wheel.BackgroundJobs;

[Serializable]
public class BackgroundJobData
{
    public BackgroundJobData()
    {
        Properties = new Dictionary<string, object>();
    }

    public object this[string key]
    {
        get => Properties.GetOrDefault(key);
        set => Properties[key] = value;
    }

    public Dictionary<string, object> Properties { get; set; }
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string CronExpression { get; set; } = default!;

    public override string ToString()
    {
        return this.ToJsonString();
    }
}