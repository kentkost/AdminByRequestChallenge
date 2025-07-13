namespace AdminByRequestChallenge.DataContext.Entities;

public class Session
{
    public required string Username { get; set; }
    public required string SessionKey { get; set; }
    public long Expiration { get; set; }
    public bool IsGuest { get; set; }
    public bool HasBeenUsed { get; set; }
}
