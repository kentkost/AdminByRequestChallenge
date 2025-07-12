namespace AdminByRequestChallenge.DataContext.Entities;

public class User
{
    public int Id { get; set; } 
    public required string Username { get; set; }
    public required byte[] PasswordHash { get; set; }
    public required byte[] Salt { get; set; } 
}