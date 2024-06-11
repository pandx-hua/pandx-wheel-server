using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using pandx.Wheel.Authorization;
using pandx.Wheel.Events;
using pandx.Wheel.Persistence.EntityFrameworkCore;
using Sample.Domain.Authorization.Roles;
using Sample.Domain.Authorization.Users;
using Sample.Domain.Books;

namespace Sample.EntityFrameworkCore;

public class
    SampleDbContext : WheelDbContext<ApplicationUser, ApplicationRole, ApplicationUserClaim, ApplicationRoleClaim>
{
    public SampleDbContext(DbContextOptions options, IEventPublisher eventPublisher, ICurrentUser currentUser,
        ILogger<WheelDbContext<ApplicationUser, ApplicationRole, ApplicationUserClaim, ApplicationRoleClaim>> logger) :
        base(options, eventPublisher, currentUser, logger)
    {
       
    }

    public DbSet<Book> Books { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<ApplicationUser>(e => e.ToTable("Users"));
        modelBuilder.Entity<ApplicationRole>(e => e.ToTable("Roles"));
        modelBuilder.Entity<ApplicationUserClaim>(e => e.ToTable("UserClaims"));
        modelBuilder.Entity<ApplicationRoleClaim>(e => e.ToTable("RoleClaims"));
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SampleDbContext).Assembly);
    }
}