using pandx.Wheel.Authorization.Users;
using pandx.Wheel.Domain.Repositories;
using pandx.Wheel.Domain.UnitOfWork;
using pandx.Wheel.Organizations;
using Sample.Domain.Authorization.Roles;

namespace Sample.Domain.Authorization.Users;

public class UserService : WheelUserService<ApplicationUser, ApplicationRole>
{
    public UserService(IRepository<UserOrganization> userOrganizationRepository,
        IRepository<Organization, Guid> organizationRepository, IUnitOfWork unitOfWork) : base(
        userOrganizationRepository, organizationRepository, unitOfWork)
    {
    }
}