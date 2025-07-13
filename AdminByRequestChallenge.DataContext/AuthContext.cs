using AdminByRequestChallenge.DataContext.Configurations;
using AdminByRequestChallenge.DataContext.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AdminByRequestChallenge.DataContext;

public class AuthContext : DbContext
{
    private readonly DbContextOptions options;
    protected readonly ILoggerFactory loggerFactory;

    public AuthContext(DbContextOptions options, ILoggerFactory loggerFactory) : base(options)
    {
        this.options = options;
        this.loggerFactory = loggerFactory;
    }

    public DbSet<User>  Users { get; set; }
    public DbSet<Session> Sessions { get; set; }

    //public DbSet<ClaimFunction> ClaimFunctions { get; set; }
    //public DbSet<UserClaims> UserClaims { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new SessionConfiguration());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLoggerFactory(loggerFactory);
    }
}
