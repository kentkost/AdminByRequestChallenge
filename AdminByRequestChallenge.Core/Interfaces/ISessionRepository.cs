using AdminByRequestChallange.Contracts;
using AdminByRequestChallenge.DataContext.Entities;

namespace AdminByRequestChallenge.Core.Interfaces;

public interface ISessionRepository
{
    Task<bool> AddSession(string username, string key, long expiration, bool isGuest = false);
    Task<Session?> GetNonExpiredSession(string key);
    Task<bool> MarkSessionAsInvalid(string key);
    Task InvalidateSessions(string username);
}