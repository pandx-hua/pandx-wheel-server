using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using pandx.Wheel.Application.Dto;
using pandx.Wheel.Domain.Repositories;
using pandx.Wheel.Exceptions;
using pandx.Wheel.Extensions;
using pandx.Wheel.Models;
using pandx.Wheel.Organizations;
using Sample.Application.Authorization.Users.Dto;
using Sample.Application.Organizations.Dto;
using Sample.Domain.Authorization.Users;

namespace Sample.Application.Organizations;

public class OrganizationAppService : SampleAppServiceBase, IOrganizationAppService
{
    private readonly IOrganizationManager _organizationManager;
    private readonly IRepository<Organization, Guid> _organizationRepository;
    private readonly IRepository<UserOrganization> _userOrganizationRepository;

    public OrganizationAppService(IRepository<Organization, Guid> organizationRepository,
        IRepository<UserOrganization> userOrganizationRepository, IOrganizationManager organizationManager)
    {
        _organizationRepository = organizationRepository;
        _userOrganizationRepository = userOrganizationRepository;
        _organizationManager = organizationManager;
    }

    public async Task<ListResponse<OrganizationDto>> GetOrganizationsAsync()
    {
        var organizations = (await _organizationRepository.GetAllListAsync()).OrderBy(o => o.Code);
        var organizationMembers = await (await _userOrganizationRepository.GetAllAsync())
            .GroupBy(uo => uo.OrganizationId)
            .Select(groupedUsers => new
            {
                organizationId = groupedUsers.Key,
                count = groupedUsers.Count()
            }).ToDictionaryAsync(x => x.organizationId, y => y.count);
        return new ListResponse<OrganizationDto>(organizations.Select(o =>
        {
            var dto = Mapper.Map<OrganizationDto>(o);
            dto.MemberCount = organizationMembers.TryGetValue(o.Id, out var count) ? count : 0;
            return dto;
        }).ToList());
    }

    public async Task<PagedResponse<OrganizationUsersDto>> GetOrganizationUsersAsync(
        GetOrganizationUsersRequest2 request)
    {
        IQueryable<UserOrganizations> query;
        if (request.OrganizationId is not null)
        {
            query = from oUser in await _userOrganizationRepository.GetAllAsync()
                join user in UserService.UserManager.Users on oUser.UserId equals user.Id
                where oUser.OrganizationId == request.OrganizationId
                select new UserOrganizations
                {
                    UserOrganization = oUser,
                    User = user
                };
        }
        else
        {
            query = from user in UserService.UserManager.Users
                select new UserOrganizations
                {
                    UserOrganization = null,
                    User = user
                };
        }

        query = query
            .WhereIf(request.Gender.Count > 0, x => request.Gender.Contains(x.User.Gender))
            .WhereIf(request.IsActive.Count > 0, x => request.IsActive.Contains(x.User.IsActive))
            .WhereIf(request.IsWeixin.Count == 1 && request.IsWeixin.Contains(true),
                x => !string.IsNullOrWhiteSpace(x.User.OpenId))
            .WhereIf(request.IsWeixin.Count == 1 && request.IsWeixin.Contains(false),
                x => string.IsNullOrWhiteSpace(x.User.OpenId))
            .WhereIf(request.IsLockout.Count == 1 && request.IsLockout.Contains(true),
                x => x.User.LockoutEnd != null && x.User.LockoutEnd > DateTimeOffset.Now)
            .WhereIf(request.IsLockout.Count == 1 && request.IsLockout.Contains(false),
                x => x.User.LockoutEnd == null || x.User.LockoutEnd <= DateTimeOffset.Now)
            .WhereIf(!string.IsNullOrWhiteSpace(request.Filter),
                x => x.User.UserName!.Contains(request.Filter!) ||
                     x.User.Name.Contains(request.Filter!) ||
                     x.User.Email!.Contains(request.Filter!) ||
                     x.User.PhoneNumber!.Contains(request.Filter!));
        var totalCount = await query.CountAsync();
        var items = await query.OrderBy(request.Sorting!).PageBy(request).ToListAsync();
        var organizationUsersDtos = items.Select(item =>
        {
            var dto = Mapper.Map<OrganizationUsersDto>(item.User);
            dto.AddedTime = item.UserOrganization?.CreationTime;
            dto.IsLockout = item.User.LockoutEnd != null && item.User.LockoutEnd > DateTimeOffset.Now;
            dto.IsWeixin = !string.IsNullOrWhiteSpace(item.User.OpenId);
            return dto;
        }).ToList();
        await FillOrganizationsAsync(organizationUsersDtos);
        await FillRolesAsync(organizationUsersDtos);
        return new PagedResponse<OrganizationUsersDto>(totalCount, organizationUsersDtos);
    }

    public async Task<PagedResponse<OrganizationUsersDto>> GetOrganizationUsersAsync(
        GetOrganizationUsersRequest1 request)
    {
        var query = from oUser in await _userOrganizationRepository.GetAllAsync()
            join user in UserService.UserManager.Users on oUser.UserId equals user.Id
            where oUser.OrganizationId == request.OrganizationId
            select new UserOrganizations
            {
                UserOrganization = oUser,
                User = user
            };

        query = query
            .WhereIf(request.Gender.Count > 0, x => request.Gender.Contains(x.User.Gender))
            .WhereIf(request.IsActive.Count > 0, x => request.IsActive.Contains(x.User.IsActive))
            .WhereIf(request.IsWeixin.Count == 1 && request.IsWeixin.Contains(true),
                x => !string.IsNullOrWhiteSpace(x.User.OpenId))
            .WhereIf(request.IsWeixin.Count == 1 && request.IsWeixin.Contains(false),
                x => string.IsNullOrWhiteSpace(x.User.OpenId))
            .WhereIf(request.IsLockout.Count == 1 && request.IsLockout.Contains(true),
                x => x.User.LockoutEnd != null && x.User.LockoutEnd > DateTimeOffset.Now)
            .WhereIf(request.IsLockout.Count == 1 && request.IsLockout.Contains(false),
                x => x.User.LockoutEnd == null || x.User.LockoutEnd <= DateTimeOffset.Now)
            .WhereIf(!string.IsNullOrWhiteSpace(request.Filter),
                x => x.User.UserName!.Contains(request.Filter!) ||
                     x.User.Name.Contains(request.Filter!) ||
                     x.User.Email!.Contains(request.Filter!) ||
                     x.User.PhoneNumber!.Contains(request.Filter!));
        var totalCount = await query.CountAsync();
        var items = await query.OrderBy(request.Sorting!).PageBy(request).ToListAsync();
        var organizationUsersDtos = items.Select(item =>
        {
            var dto = Mapper.Map<OrganizationUsersDto>(item.User);
            dto.AddedTime = item.UserOrganization?.CreationTime;
            dto.IsLockout = item.User.LockoutEnd != null && item.User.LockoutEnd > DateTimeOffset.Now;
            dto.IsWeixin = !string.IsNullOrWhiteSpace(item.User.OpenId);
            return dto;
        }).ToList();
        await FillOrganizationsAsync(organizationUsersDtos);
        await FillRolesAsync(organizationUsersDtos);
        return new PagedResponse<OrganizationUsersDto>(totalCount, organizationUsersDtos);
    }

    public async Task<OrganizationDto> CreateOrganizationAsync(CreateOrganizationRequest request)
    {
        var organization = new Organization
        {
            DisplayName = request.DisplayName,
            Address = request.Address,
            ParentId = request.ParentId,
            Phone = request.Phone,
            Head = request.Head
        };
        // var organization = Mapper.Map<Organization>(request);
        await _organizationManager.CreateAsync(organization);
        return Mapper.Map<OrganizationDto>(organization);
    }

    public async Task<OrganizationDto> UpdateOrganizationAsync(UpdateOrganizationRequest request)
    {
        var organization = await _organizationRepository.GetAsync(request.Id);
        organization.DisplayName = request.DisplayName;
        organization.Address = request.Address;
        organization.Phone = request.Phone;
        organization.Head = request.Head;
        await _organizationManager.UpdateAsync(organization);
        return await CreateOrganizationDtoAsync(organization);
    }

    public async Task<OrganizationDto> MoveOrganizationAsync(MoveOrganizationRequest request)
    {
        if (request.NewId.HasValue)
        {
            switch (request.Position)
            {
                case "before":
                    var newOrganization1 = await _organizationRepository.GetAsync(request.NewId.Value);
                    var newParentId1 = newOrganization1.ParentId;
                    await _organizationManager.Move(request.Id, newParentId1);
                    break;
                case "after":
                    var newOrganization2 = await _organizationRepository.GetAsync(request.NewId.Value);
                    var newParentId2 = newOrganization2.ParentId;
                    await _organizationManager.Move(request.Id, newParentId2);
                    break;
                case "inner":
                    await _organizationManager.Move(request.Id, request.NewId);
                    break;
            }
        }

        return await CreateOrganizationDtoAsync(await _organizationRepository.GetAsync(request.Id));
    }

    public async Task UpAsync(EntityDto<Guid> request)
    {
        var organization = await _organizationRepository.GetAsync(request.Id);
        await _organizationManager.UpAsync(organization);
    }

    public async Task DownAsync(EntityDto<Guid> request)
    {
        var organization = await _organizationRepository.GetAsync(request.Id);
        await _organizationManager.DownAsync(organization);
    }

    public async Task DeleteOrganizationAsync(EntityDto<Guid> request)
    {
        if (await _organizationRepository.CountAsync(o => o.ParentId == request.Id) > 0)
        {
            throw new WheelException("删除失败，存在下级部门");
        }

        await _userOrganizationRepository.DeleteAsync(x => x.OrganizationId == request.Id);
        await _organizationRepository.DeleteAsync(request.Id);
    }

    public async Task RemoveUserFromOrganizationAsync(UserToOrganizationRequest request)
    {
        await UserService.RemoveFromOrganizationAsync(request.UserId, request.OrganizationId);
    }

    public async Task RemoveUsersFromOrganizationAsync(UsersToOrganizationRequest request)
    {
        foreach (var userId in request.UserIds)
        {
            await UserService.RemoveFromOrganizationAsync(userId, request.OrganizationId);
        }
    }

    public async Task AddUsersToOrganizationAsync(UsersToOrganizationRequest request)
    {
        foreach (var userId in request.UserIds)
        {
            await UserService.AddToOrganizationAsync(userId, request.OrganizationId);
        }
    }

    public async Task<PagedResponse<UsersDto>> FindUsersAsync(FindOrganizationUsersRequest request)
    {
        var userIdsInOrganization = (await _userOrganizationRepository.GetAllAsync())
            .Where(oUser => oUser.OrganizationId == request.OrganizationId)
            .Select(oUser => oUser.UserId);
        var query = from user in UserService.UserManager.Users
                .Where(u => !userIdsInOrganization.Contains(u.Id))
            select new
            {
                User = user
            };
        query = query
            .WhereIf(request.Gender.Count > 0, x => request.Gender.Contains(x.User.Gender))
            .WhereIf(request.IsActive.Count > 0, x => request.IsActive.Contains(x.User.IsActive))
            .WhereIf(request.IsWeixin.Count == 1 && request.IsWeixin.Contains(true),
                x => !string.IsNullOrWhiteSpace(x.User.OpenId))
            .WhereIf(request.IsWeixin.Count == 1 && request.IsWeixin.Contains(false),
                x => string.IsNullOrWhiteSpace(x.User.OpenId))
            .WhereIf(request.IsLockout.Count == 1 && request.IsLockout.Contains(true),
                x => x.User.LockoutEnd != null && x.User.LockoutEnd > DateTimeOffset.Now)
            .WhereIf(request.IsLockout.Count == 1 && request.IsLockout.Contains(false),
                x => x.User.LockoutEnd == null || x.User.LockoutEnd <= DateTimeOffset.Now)
            .WhereIf(!string.IsNullOrWhiteSpace(request.Filter),
                x => x.User.UserName!.Contains(request.Filter!) ||
                     x.User.Name.Contains(request.Filter!) ||
                     x.User.Email!.Contains(request.Filter!) ||
                     x.User.PhoneNumber!.Contains(request.Filter!));
        var totalCount = await query.CountAsync();
        var userOrganizations = await query.OrderBy(request.Sorting!).PageBy(request).ToListAsync();
        var usersDtos = userOrganizations.Select(item =>
        {
            var dto = Mapper.Map<UsersDto>(item.User);
            dto.IsLockout = item.User.LockoutEnd != null && item.User.LockoutEnd > DateTimeOffset.Now;
            dto.IsWeixin = !string.IsNullOrWhiteSpace(item.User.OpenId);
            return dto;
        }).ToList();
        await FillRolesAsync(usersDtos);
        await FillOrganizationsAsync(usersDtos);
        return new PagedResponse<UsersDto>(totalCount, usersDtos);
    }

    public async Task<ValidationResponse> ValidateOrganizationAsync(ValidationRequest<string, string> request)
    {
        if (string.IsNullOrWhiteSpace(request.Id))
        {
            var one = await _organizationRepository.FirstOrDefaultAsync(x =>
                x.DisplayName == request.Value && x.ParentId.ToString() == request.ParentId);
            if (one is null)
            {
                return new ValidationResponse
                {
                    Status = true
                };
            }

            return new ValidationResponse
            {
                Status = false,
                Message = $"同级别下已存在名称为 {request.Value} 的部门"
            };
        }
        else
        {
            var one = await _organizationRepository.FirstOrDefaultAsync(x =>
                x.DisplayName == request.Value && x.Id.ToString() != request.Id &&
                x.ParentId.ToString() == request.ParentId);
            if (one is null)
            {
                return new ValidationResponse
                {
                    Status = true
                };
            }

            return new ValidationResponse
            {
                Status = false,
                Message = $"同级别下已存在名称为 {request.Value} 的部门"
            };
        }
    }

    private async Task<OrganizationDto> CreateOrganizationDtoAsync(Organization organization)
    {
        var dto = Mapper.Map<OrganizationDto>(organization);
        dto.MemberCount =
            await _userOrganizationRepository.CountAsync(oUser => oUser.OrganizationId == organization.Id);
        return dto;
    }

    private async Task FillOrganizationsAsync(IReadOnlyCollection<OrganizationUsersDto> dtos)
    {
        foreach (var dto in dtos)
        {
            var query = from userOrganization in _userOrganizationRepository.GetAll()
                    .Where(uo => uo.UserId == dto.Id)
                join o in _organizationRepository.GetAll()
                    on userOrganization.OrganizationId equals o.Id
                select o;
            dto.Organizations = Mapper.Map<List<UserOrganizationsDto>>(await query.ToListAsync());
        }
    }

    private async Task FillOrganizationsAsync(IReadOnlyCollection<UsersDto> dtos)
    {
        foreach (var dto in dtos)
        {
            var query = from userOrganization in _userOrganizationRepository.GetAll()
                    .Where(uo => uo.UserId == dto.Id)
                join o in _organizationRepository.GetAll()
                    on userOrganization.OrganizationId equals o.Id
                select o;
            dto.Organizations = Mapper.Map<List<UserOrganizationsDto>>(await query.ToListAsync());
        }
    }

    private async Task FillRolesAsync(IReadOnlyCollection<OrganizationUsersDto> dtos)
    {
        foreach (var dto in dtos)
        {
            var userRoleDtos = new List<UserRolesDto>();
            foreach (var roleName in await UserService.UserManager.GetRolesAsync(
                         await UserService.UserManager.FindByIdAsync(dto.Id.ToString()) ??
                         throw new InvalidOperationException("没有发现用户")))
            {
                userRoleDtos.Add(Mapper.Map<UserRolesDto>(await RoleService.RoleManager.FindByNameAsync(roleName)));
            }

            dto.Roles = userRoleDtos;
        }
    }

    private async Task FillRolesAsync(IReadOnlyCollection<UsersDto> dtos)
    {
        foreach (var dto in dtos)
        {
            var userRoleDtos = new List<UserRolesDto>();
            foreach (var roleName in await UserService.UserManager.GetRolesAsync(
                         await UserService.UserManager.FindByIdAsync(dto.Id.ToString()) ??
                         throw new InvalidOperationException("没有发现用户")))
            {
                userRoleDtos.Add(Mapper.Map<UserRolesDto>(await RoleService.RoleManager.FindByNameAsync(roleName)));
            }

            dto.Roles = userRoleDtos;
        }
    }

    private class UserOrganizations
    {
        public UserOrganization? UserOrganization { get; set; }
        public ApplicationUser User { get; set; } = default!;
        public IEnumerable<Organization> Organizations { get; set; } = default!;
    }
}