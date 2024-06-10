using Microsoft.EntityFrameworkCore;
using pandx.Wheel.Domain.Repositories;
using pandx.Wheel.Domain.UnitOfWork;

namespace pandx.Wheel.BackgroundJobs;

public class BackgroundJobManager : IBackgroundJobManager
{
    private readonly IBackgroundJobLauncher _backgroundJobLauncher;
    private readonly IRepository<BackgroundJobInfo, Guid> _backgroundJobRepository;
    private readonly IRepository<JobExecutionInfo, Guid> _jobExecutionInfoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public BackgroundJobManager(
        IRepository<BackgroundJobInfo, Guid> backgroundJobRepository,
        IUnitOfWork unitOfWork,
        IRepository<JobExecutionInfo, Guid> jobExecutionInfoRepository,
        IBackgroundJobLauncher backgroundJobLauncher)
    {
        _backgroundJobRepository = backgroundJobRepository;
        _unitOfWork = unitOfWork;
        _backgroundJobLauncher = backgroundJobLauncher;
        _jobExecutionInfoRepository = jobExecutionInfoRepository;
    }

    public async Task<List<JobExecutionInfo>> GetJobExecutionsAsync(Guid backgroundJobId)
    {
        return await (await _jobExecutionInfoRepository.GetAllAsync(a => a.BackgroundJobId == backgroundJobId))
            .OrderByDescending(a => a.StartTime).ToListAsync();
    }

    public async Task<BackgroundJobInfo> GetAsync(Guid id)
    {
        return await _backgroundJobRepository.GetAsync(id);
    }

    public async Task CreateAsync(BackgroundJobInfo backgroundJobInfo)
    {
        await _backgroundJobRepository.InsertAsync(backgroundJobInfo);
        await _unitOfWork.CommitAsync();
    }

    public async Task UpdateAsync(BackgroundJobInfo backgroundJobInfo)
    {
        await _backgroundJobLauncher.StopAsync(backgroundJobInfo.Id.ToString());
        await _backgroundJobRepository.UpdateAsync(backgroundJobInfo);
        await _unitOfWork.CommitAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        await _backgroundJobRepository.DeleteAsync(id);
        await _unitOfWork.CommitAsync();
        await _backgroundJobLauncher.StopAsync(id.ToString());
    }

    public async Task<IQueryable<BackgroundJobInfo>> GetAllAsync()
    {
        return await _backgroundJobRepository.GetAllAsync();
    }
}