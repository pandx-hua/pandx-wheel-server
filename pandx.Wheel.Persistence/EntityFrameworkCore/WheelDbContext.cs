using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using pandx.Wheel.Auditing;
using pandx.Wheel.Authorization;
using pandx.Wheel.Authorization.Logins;
using pandx.Wheel.Authorization.Roles;
using pandx.Wheel.Authorization.Users;
using pandx.Wheel.BackgroundJobs;
using pandx.Wheel.Domain.Entities;
using pandx.Wheel.Events;
using pandx.Wheel.Notifications;
using pandx.Wheel.Organizations;
using pandx.Wheel.Storage;

namespace pandx.Wheel.Persistence.EntityFrameworkCore;

public abstract class WheelDbContext<TUser, TRole, TUserClaim, TRoleClaim> : IdentityDbContext<TUser, TRole, Guid,
    TUserClaim,
    IdentityUserRole<Guid>, IdentityUserLogin<Guid>, TRoleClaim, IdentityUserToken<Guid>>
    where TUser : WheelUser
    where TRole : WheelRole
    where TRoleClaim : WheelRoleClaim
    where TUserClaim : WheelUserClaim
{
    private readonly ICurrentUser _currentUser;

    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<WheelDbContext<TUser, TRole, TUserClaim, TRoleClaim>> _logger;

    protected WheelDbContext(DbContextOptions options, IEventPublisher eventPublisher, ICurrentUser currentUser,
        ILogger<WheelDbContext<TUser, TRole, TUserClaim, TRoleClaim>> logger) :
        base(options)
    {
        _eventPublisher = eventPublisher;
        _currentUser = currentUser;
        _logger = logger;
    }

    public DbSet<Notification> Notifications { get; set; } = default!;
    public DbSet<UserNotification> UserNotifications { get; set; } = default!;
    public DbSet<NotificationSubscription> NotificationSubscriptions { get; set; } = default!;

    public DbSet<Organization> Organizations { get; set; } = default!;
    public DbSet<UserOrganization> UserOrganizations { get; set; } = default!;

    public DbSet<AuditingInfo> Auditing { get; set; } = default!;
    public DbSet<BackgroundJobInfo> BackgroundJobs { get; set; } = default!;
    public DbSet<JobExecutionInfo> JobExecutions { get; set; } = default!;

    public DbSet<LoginAttempt> LoginAttempts { get; set; } = default!;
    public DbSet<BinaryObject> BinaryObjects { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //应用全局查询过滤器，需要放置在base.OnModelCreating之前
        modelBuilder.ConfigureGlobalQueryFilter<ISoftDelete>(e => !e.IsDeleted);

        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<IdentityUserLogin<Guid>>(e => e.ToTable("UserLogins"));
        modelBuilder.Entity<IdentityUserToken<Guid>>(e => e.ToTable("UserTokens"));
        modelBuilder.Entity<IdentityUserRole<Guid>>(e => e.ToTable("UserRoles"));
        modelBuilder.Entity<UserOrganization>().HasIndex(e => new { e.OrganizationId, e.UserId });
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = new())
    {
        var events = await DispatchDomainEventsBeforeSaveChangesAsync();
        await HandleAuditedBeforeSaveChanges(_currentUser.GetUserId());
        var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        await DispatchDomainEventsAfterSaveChangesAsync(events);
        return result;
    }

    private async Task HandleAuditedBeforeSaveChanges(Guid? userId)
    {
        foreach (var entry in ChangeTracker.Entries<IAudited>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.Creator = userId!.Value;
                    entry.Entity.CreationTime = DateTime.Now;
                    break;
                case EntityState.Modified:
                    entry.Entity.Modifier = userId!.Value;
                    entry.Entity.ModificationTime = DateTime.Now;
                    break;
                case EntityState.Deleted:
                    if (entry.Entity is ISoftDelete softDeleteEntity)
                    {
                        entry.Entity.Deleter = userId!.Value;
                        softDeleteEntity.IsDeleted = true;
                        entry.Entity.DeletionTime = DateTime.Now;
                        entry.State = EntityState.Modified;
                    }
                    break;
            }

            ChangeTracker.DetectChanges();
        }
    }

    private Task<List<IEvent>> DispatchDomainEventsBeforeSaveChangesAsync()
    {
        List<IEvent> events = new();
        var entities = ChangeTracker.Entries<IEntity>().ToList();
        foreach (var entry in entities)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    events.Add((IEvent)Activator.CreateInstance(
                        typeof(EntityAddedEvent<>).MakeGenericType(entry.Entity.GetType()),
                        entry.Entity)!);
                    break;
                case EntityState.Modified:
                    events.Add((IEvent)Activator.CreateInstance(
                        typeof(EntityModifiedEvent<>).MakeGenericType(entry.Entity.GetType()),
                        entry.Entity)!);
                    break;
                case EntityState.Deleted:
                    events.Add((IEvent)Activator.CreateInstance(
                        typeof(EntityDeletedEvent<>).MakeGenericType(entry.Entity.GetType()),
                        entry.Entity)!);
                    break;
            }
        }

        return Task.FromResult(events);
    }

    private async Task DispatchDomainEventsAfterSaveChangesAsync(List<IEvent> events)
    {
        foreach (var @event in events)
        {
            await _eventPublisher.PublishAsync(@event);
        }
    }
}