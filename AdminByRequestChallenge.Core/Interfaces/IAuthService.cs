using AdminByRequestChallange.Contracts;

namespace AdminByRequestChallenge.Core.Interfaces;

public interface IAuthService
{
    Task<SessionResponse> CreateSession(string username, string password);
    Task<SessionResponse> CreateGuestSession(string invitee, List<string> claims);
    Task<string> CreateJwt(string username, string password);
}