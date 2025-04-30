using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebApplication1.Data.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder
            .HasOne(pr => pr.Client)
            .WithMany(c => c.ClientProjects)
            .HasForeignKey(pr => pr.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(pr => pr.Manager)
            .WithMany(c => c.ManagedProjects)
            .HasForeignKey(pr => pr.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasMany(pr => pr.ProjectComments)
            .WithOne()
            .HasForeignKey(pr => pr.ProjectId);

        builder
            .OwnsMany(pr => pr.Videos, v =>
            {
                v.WithOwner().HasForeignKey("ProjectId");
                v.Property<int>("Id");
                v.HasKey("Id");
            });
    }
}