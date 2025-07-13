using AdminByRequestChallange.Contracts;

namespace AdminByRequestChallenge.Core.Interfaces;

public interface IAuthService
{
    Task<string> CreateJwt(string username, string password);
    Task<string> CreateGuestJwt(string username, string password);
}