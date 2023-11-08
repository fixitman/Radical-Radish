using RadzenUI.Pages.Auth.Models;



namespace RadzenUI.Data;

public class AppState
{
    public AppUser? User { get; set; }

    public string ReturnTo { get; set; } = "";
   
}

