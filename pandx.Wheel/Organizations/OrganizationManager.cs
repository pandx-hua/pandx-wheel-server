using pandx.Wheel.Domain.Repositories;
using pandx.Wheel.Domain.Services;
using pandx.Wheel.Domain.UnitOfWork;
using pandx.Wheel.Exceptions;

namespace pandx.Wheel.Organizations;

public class OrganizationManager : DomainServiceBase, IOrganizationManager
{
    private readonly IRepository<Organization, Guid> _organizationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<UserOrganization> _userOrganizationRepository;

    public OrganizationManager(IRepository<Organization, Guid> organizationRepository,
        IRepository<UserOrganization> userOrganizationRepository,
        IUnitOfWork unitOfWork)
    {
        _organizationRepository = organizationRepository;
        _userOrganizationRepository = userOrganizationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task CreateAsync(Organization organization)
    {
        organization.Code = await GetNextChildCodeAsync(organization.ParentId);
        await ValidateOrganizationAsync(organization);
        await _organizationRepository.InsertAsync(organization);
        await _unitOfWork.CommitAsync();
    }

    public async Task UpdateAsync(Organization organization)
    {
        await ValidateOrganizationAsync(organization);
        await _organizationRepository.UpdateAsync(organization);
        await _unitOfWork.CommitAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var children = await FindChildrenAsync(id, true);
        foreach (var child in children)
        {
            await _organizationRepository.DeleteAsync(child);
        }

        await _organizationRepository.DeleteAsync(id);
        await _unitOfWork.CommitAsync();
    }

    public async Task Move(Guid id, Guid? parentId)
    {
        var organization = await _organizationRepository.GetAsync(id);
        if (organization.ParentId == parentId)
        {
            return;
        }

        var children = await FindChildrenAsync(id, true);
        var oldCode = organization.Code;
        organization.Code = await GetNextChildCodeAsync(parentId);
        organization.ParentId = parentId;

        await ValidateOrganizationAsync(organization);

        foreach (var child in children)
        {
            child.Code = Organization.AppendCode(organization.Code, Organization.GetRelativeCode(child.Code, oldCode));
        }

        await _unitOfWork.CommitAsync();
    }

    public async Task UpAsync(Organization organization)
    {
        var children = (await FindChildrenAsync(organization.ParentId)).OrderBy(o => o.Code).ToList();
        var index = children.FindIndex(o => o.Id == organization.Id);
        if (index > 0)
        {
            var prev = children[index - 1];
            var temp = prev.Code;
            prev.Code = organization.Code;
            await UpdateAsync(prev);
            organization.Code = temp;
            await UpdateAsync(organization);
            await _unitOfWork.CommitAsync();
        }
        else
        {
            throw new WheelException("已经是最上了");
        }
    }

    public async Task DownAsync(Organization organization)
    {
        var children = (await FindChildrenAsync(organization.ParentId)).OrderBy(o => o.Code).ToList();
        var index = children.FindIndex(o => o.Id == organization.Id);
        if (index < children.Count - 1)
        {
            var next = children[index + 1];
            var temp = next.Code;
            next.Code = organization.Code;
            await UpdateAsync(next);
            organization.Code = temp;
            await UpdateAsync(organization);
            await _unitOfWork.CommitAsync();
        }
        else
        {
            throw new WheelException("已经是最下了");
        }
    }

    public async Task<string> GetNextChildCodeAsync(Guid? parentId)
    {
        var lastChild = await GetLastChildAsync(parentId);
        if (lastChild is not null)
        {
            return Organization.CalculateNextCode(lastChild.Code);
        }

        var parentCode = parentId is not null ? await GetCodeAsync(parentId.Value) : null;
        return Organization.AppendCode(parentCode, Organization.CreateCode(1));
    }

    public async Task<Organization?> GetLastChildAsync(Guid? parentId)
    {
        return (await _organizationRepository.GetAllAsync())
            .Where(o => o.ParentId == parentId)
            .OrderByDescending(o => o.Code).FirstOrDefault();
    }

    public async Task<string> GetCodeAsync(Guid id)
    {
        return (await _organizationRepository.GetAsync(id)).Code;
    }

    public async Task<List<Organization>> FindChildrenAsync(Guid? parentId, bool recursive = false)
    {
        if (!recursive)
        {
            return await _organizationRepository.GetAllListAsync(o => o.ParentId == parentId);
        }

        if (!parentId.HasValue)
        {
            return await _organizationRepository.GetAllListAsync();
        }

        var code = await GetCodeAsync(parentId.Value);
        return await _organizationRepository.GetAllListAsync(o => o.Code.StartsWith(code) && o.Id != parentId.Value);
    }

    protected async Task ValidateOrganizationAsync(Organization organization)
    {
        var siblings = (await FindChildrenAsync(organization.ParentId))
            .Where(o => o.Id != organization.Id).ToList();
        if (siblings.Any(o => o.DisplayName == organization.DisplayName))
        {
            throw new WheelException($"已经存在名称为 {organization.DisplayName} 的下级部门");
        }
    }
}