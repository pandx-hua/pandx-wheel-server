using System.Security.Claims;
using AutoMapper;
using pandx.Wheel.Auditing;
using pandx.Wheel.Authorization.Logins;
using pandx.Wheel.Authorization.Permissions;
using pandx.Wheel.BackgroundJobs;
using pandx.Wheel.Menus;
using pandx.Wheel.Notifications;
using pandx.Wheel.Organizations;
using Sample.Application.Auditing.Dto;
using Sample.Application.Authorization.Permissions.Dto;
using Sample.Application.Authorization.Roles.Dto;
using Sample.Application.Authorization.Users.Dto;
using Sample.Application.Authorization.Users.Importing.Dto;
using Sample.Application.BackgroundJobs.Dto;
using Sample.Application.Menus.Dto;
using Sample.Application.Notifications.Dto;
using Sample.Application.Organizations.Dto;
using Sample.Application.Personal.Dto;
using Sample.Domain.Authorization.Roles;
using Sample.Domain.Authorization.Users;

namespace Sample.Application;

public class CustomMapperProfile : Profile
{
    public CustomMapperProfile()
    {
        CreateMap<ApplicationUser, UserDto>().ForMember(dto => dto.Password, options => options.Ignore())
            .ReverseMap()
            .ForMember(user => user.PasswordHash, options => options.Ignore());
        CreateMap<ApplicationUser, UsersDto>();
        CreateMap<ApplicationRole, UserRolesDto>();
        CreateMap<ApplicationUser, OrganizationUsersDto>();
        CreateMap<Organization, UserOrganizationsDto>();
        CreateMap<ImportedUserDto, ApplicationUser>();
        CreateMap<ApplicationUser, PersonalDto>().ReverseMap();

        CreateMap<LoginAttempt, LoginAttemptDto>();
        CreateMap<ApplicationRole, RolesDto>();
        CreateMap<ApplicationUser, RoleUsersDto>();
        CreateMap<ApplicationRole, RoleDto>().ReverseMap();


        CreateMap<Organization, OrganizationDto>().ReverseMap();


        CreateMap<Permission, PermissionWithLevelDto>();

        CreateMap<UserNotification, UserNotificationsDto>();

        CreateMap<Menu, MenuDto>();
        CreateMap<Meta, MetaDto>();

        CreateMap<Claim, RoleClaimsDto>();

        CreateMap<BackgroundJobInfo, BackgroundJobDto>().ReverseMap();
        CreateMap<BackgroundJobInfo, BackgroundJobsDto>();
        CreateMap<JobExecutionInfo, JobExecutionsDto>();

        CreateMap<AuditingInfo, AuditingDto>();

        //此处的Map用于深拷贝
        CreateMap<Menu, Menu>();
    }
}