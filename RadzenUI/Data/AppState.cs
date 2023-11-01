namespace RadzenUI.Data;

public class AppState
{
    public User? User { get; set; }
    public string? CurrentCalendar { get; set; }
}

public class User
{
    public Guid? Id { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? PWHash { get; set; }
}