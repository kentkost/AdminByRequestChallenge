using AdminByRequestChallenge.DataContext.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace AdminByRequestChallenge.DataContext.Configurations;

public class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.ToTable("Sessions");
        builder.HasKey(s => s.SessionKey);
        builder.Property(s => s.SessionKey).IsRequired().HasMaxLength(256);
        builder.Property(s => s.Username).IsRequired().HasMaxLength(250);
        builder.Property(s => s.Expiration).IsRequired();
        builder.Property(s => s.IsGuest).IsRequired();
        builder.Property(s => s.HasBeenUsed).IsRequired();
    }
}

