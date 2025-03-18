using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebApplication1.Data.Configurations;

public class ProjectCommentConfiguration : IEntityTypeConfiguration<ProjectComment>
{
    public void Configure(EntityTypeBuilder<ProjectComment> builder)
    {
        builder
            .HasOne(pc => pc.User)
            .WithMany(u => u.UserComments)
            .HasForeignKey(pc => pc.UserId);
    }
}