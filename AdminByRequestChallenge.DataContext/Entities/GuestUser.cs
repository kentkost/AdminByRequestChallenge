namespace AdminByRequestChallenge.DataContext.Entities;

public class GuestUser
{
    public int Id { get; set; }
    public int HostUserId { get; set; }
    public required string Username { get; set; }
    public bool BeenUsed { get; set; }
    public DateTime Expiration { get; set; }
    public required byte[] PasswordHash { get; set; }
    public required byte[] Salt { get; set; }
}
