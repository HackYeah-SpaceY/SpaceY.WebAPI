using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpaceY.RestApi.Entities;

namespace SpaceY.RestApi.Database.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasMany(u => u.Chats)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId);
    }
}
