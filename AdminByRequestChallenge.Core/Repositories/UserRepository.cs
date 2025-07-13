using AdminByRequestChallenge.Core.Interfaces;
using AdminByRequestChallenge.DataContext;
using AdminByRequestChallenge.DataContext.Entities;
using Microsoft.EntityFrameworkCore;

namespace AdminByRequestChallenge.Core.Repositories;

public class UserRepository(AuthContext context) : IUserRepository
{
    public async Task<bool> CreateGuestUser(User host, DateTime expiration, byte[] hashedPassword, byte[] salt)
    {
        var guest = new GuestUser() {
            HostUserId = host.Id,
            Username = host.Username.ToLower().Trim(),
            BeenUsed = false,
            Expiration = expiration,
            PasswordHash = hashedPassword,
            Salt = salt,
        };

        await context.GuestUsers.AddAsync(guest);
        return await context.SaveChangesAsync() == 1;
    }

    public async Task<bool> CreateUser(User entUser)
    {
        entUser.Username = entUser.Username.ToLower().Trim();
        await context.Users.AddAsync(entUser);
        return await context.SaveChangesAsync() == 1;
    }

    public async Task<List<GuestUser>> GetGuestUsers(string username)
    {
        var now = DateTime.UtcNow;
        return await context.GuestUsers.AsNoTracking().Where(x => x.BeenUsed == false && x.Username == username.ToLower().Trim() && x.Expiration >= now).ToListAsync();
    }

    public async Task<User> GetUser(string username)
        => await context.Users.AsNoTracking().Where(x => x.Username == username.ToLower().Trim()).FirstAsync();

    public void MarkGuestUserPasswordAsUsed(int id)
        => context.GuestUsers.Where(x => x.Id == id).ExecuteUpdate(x => x.SetProperty(x => x.BeenUsed, true));
}

