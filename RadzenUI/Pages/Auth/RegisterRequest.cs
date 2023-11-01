using System.ComponentModel.DataAnnotations;

namespace RadzenUI.Pages.Auth;

public class RegisterRequest
{
    [Required]   public  string Username { get; set; }
    [Required]   public  string Email { get; set; }
    [Required]   public string Password { get; set; }
    [Required]   public string Verify { get; set; }

}
