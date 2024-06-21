namespace BilliardTimeTracker.LogicAndPartialModels;

public class UserSession
{
    private static UserSession _instance;

    public static UserSession Instance => _instance ??= new UserSession();

    public int UserId { get; private set; }
    public string Username { get; private set; }
    public string Role { get; private set; }

    private UserSession() { }

    public void SetUser(int userId, string username, string role)
    {
        UserId = userId;
        Username = username;
        Role = role;
    }
}