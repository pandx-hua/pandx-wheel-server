using pandx.Wheel.DependencyInjection;

namespace pandx.Wheel.BackgroundJobs;

public interface IBackgroundJobManager : ITransientDependency
{
    Task CreateAsync(BackgroundJobInfo backgroundJobInfo);
    Task UpdateAsync(BackgroundJobInfo backgroundJobInfo);
    Task DeleteAsync(Guid id);
    Task<BackgroundJobInfo> GetAsync(Guid id);
    Task<IQueryable<BackgroundJobInfo>> GetAllAsync();
    Task<List<JobExecutionInfo>> GetJobExecutionsAsync(Guid backgroundJobId);
    Task<Dictionary<string, string>> GetExposedBackgroundJobsAsync();
}