using AdminByRequestChallenge.DataContext.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminByRequestChallenge.DataContext.Configurations;

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Username).HasMaxLength(250).HasColumnType("varchar(250)").IsRequired();
        builder.Property(u => u.PasswordHash).IsRequired().HasColumnType("varbinary(max)");
        builder.Property(u => u.Salt).IsRequired().HasColumnType("varbinary(max)");
    }
}
