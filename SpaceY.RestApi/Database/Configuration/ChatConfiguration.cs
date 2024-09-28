using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpaceY.RestApi.Entities;

namespace SpaceY.RestApi.Database.Configuration;

public class ChatConfiguration : IEntityTypeConfiguration<Chat>
{
    public void Configure(EntityTypeBuilder<Chat> builder)
    {
        builder.HasMany(c => c.Messages)
            .WithOne(m => m.Chat)
            .HasForeignKey(c => c.ChatId);


        builder.HasMany(c => c.Screenshots)
            .WithOne(m => m.Chat)
            .HasForeignKey(c => c.ChatId);
    }
}
