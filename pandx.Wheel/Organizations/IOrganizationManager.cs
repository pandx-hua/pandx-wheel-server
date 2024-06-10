using pandx.Wheel.DependencyInjection;

namespace pandx.Wheel.Organizations;

public interface IOrganizationManager : ITransientDependency
{
    Task CreateAsync(Organization organization);
    Task UpdateAsync(Organization organization);
    Task DeleteAsync(Guid id);
    Task Move(Guid id, Guid? parentId);
    Task DownAsync(Organization organization);
    Task UpAsync(Organization organization);
    Task<string> GetNextChildCodeAsync(Guid? parentId);
    Task<Organization?> GetLastChildAsync(Guid? parentId);
    Task<string> GetCodeAsync(Guid id);
    Task<List<Organization>> FindChildrenAsync(Guid? parentId, bool recursive = false);
}