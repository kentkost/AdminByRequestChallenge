using AdminByRequestChallenge.DataContext.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminByRequestChallenge.DataContext.Configurations;
internal class GuestUserConfiguration : IEntityTypeConfiguration<GuestUser>
{
    public void Configure(EntityTypeBuilder<GuestUser> builder)
    {
        builder.ToTable("GuestAccesses");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.HostUserId).IsRequired();
        builder.Property(e => e.Username).IsRequired().HasMaxLength(250);
        builder.Property(e => e.PasswordHash).IsRequired().HasColumnType("varbinary(max)");
        builder.Property(e => e.Salt).IsRequired().HasColumnType("varbinary(max)");
    }
}