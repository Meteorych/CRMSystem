﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace WebApplication1.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser, ApplicationRole, Guid, IdentityUserClaim<Guid>, ApplicationUserRole,
        IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>(options)
{
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectComment> ProjectComments { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(GetType().Assembly);

        builder.Entity<ApplicationUser>()
            .Property(u => u.Id)
            .ValueGeneratedOnAdd();

        builder.Entity<ApplicationRole>()
            .Property(r => r.Id)
            .ValueGeneratedOnAdd();
        
        builder.Entity<ApplicationUser>(b =>
        {
            // Each User can have many entries in the UserRole join table
            b.HasMany(e => e.UserRoles)
                .WithOne(e => e.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();
        });

        builder.Entity<ApplicationUserRole>(b =>
        {
            b.HasOne(ar => ar.Role)
                .WithMany()
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();
        });
    }
    
    public override int SaveChanges()
    {
        SetCreatedTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        SetCreatedTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }
    
    private void SetCreatedTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e is { State: EntityState.Added, Entity: Project or ProjectComment })
            .ToList();
        
        var projectEntries = entries
            .Where(e => e.Entity is Project)
            .Select(e => (Project)e.Entity);

        var commentEntries = entries
            .Where(e => e.Entity is ProjectComment)
            .Select(e => (ProjectComment)e.Entity);

        foreach (var entry in projectEntries)
        {
            entry.CreatedAt = DateTimeOffset.UtcNow;
        }

        foreach (var entry in commentEntries)
        {
            entry.CreatedAt = DateTimeOffset.UtcNow;
        }
    }
}