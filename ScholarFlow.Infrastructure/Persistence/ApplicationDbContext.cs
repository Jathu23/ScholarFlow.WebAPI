using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ScholarFlow.Domain.Entities;
using ScholarFlow.Domain.Entities.Base;
using ScholarFlow.Domain.Interfaces;
using System.Linq.Expressions;

namespace ScholarFlow.Infrastructure.Persistence;

/// <summary>
/// Application database context with Identity integration
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets
    public DbSet<StudentProfile> StudentProfiles { get; set; }
    public DbSet<TeacherProfile> TeacherProfiles { get; set; }
    public DbSet<AcademicStream> Streams { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<SubjectStream> SubjectStreams { get; set; }
    public DbSet<Topic> Topics { get; set; }
    public DbSet<SubTopic> SubTopics { get; set; }
    public DbSet<Paper> Papers { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Option> Options { get; set; }
    public DbSet<Explanation> Explanations { get; set; }
    public DbSet<ExamSession> ExamSessions { get; set; }
    public DbSet<UserResponse> UserResponses { get; set; }
    
    // Expose Users from IdentityDbContext
    public new DbSet<ApplicationUser> Users { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from the Configurations folder
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Global query filter for soft delete
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted));
                var filter = Expression.Lambda(Expression.Equal(property, Expression.Constant(false)), parameter);
                
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
            }
        }
    }

    /// <summary>
    /// Override SaveChanges to automatically handle audit fields
    /// </summary>
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        OnBeforeSaving();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    /// <summary>
    /// Override SaveChangesAsync to automatically handle audit fields
    /// </summary>
    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        OnBeforeSaving();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    /// <summary>
    /// Automatically set audit fields before saving
    /// </summary>
    private void OnBeforeSaving()
    {
        var entries = ChangeTracker.Entries<IAuditable>();
        var utcNow = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = utcNow;
                    // TODO: Set CreatedBy from current user context
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = utcNow;
                    // TODO: Set UpdatedBy from current user context
                    break;
            }
        }

        // Handle soft delete
        var deletedEntries = ChangeTracker.Entries<ISoftDeletable>()
            .Where(e => e.State == EntityState.Deleted);

        foreach (var entry in deletedEntries)
        {
            entry.State = EntityState.Modified;
            entry.Entity.IsDeleted = true;
            entry.Entity.DeletedAt = utcNow;
            // TODO: Set DeletedBy from current user context
        }
    }
}
