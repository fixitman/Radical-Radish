using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RadzenUI.Pages.Auth.Models;

public class RegisterRequest
{
    [Required, MaxLength(50)]
    public string Username { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; }

    [Required, PasswordPropertyText, MaxLength(50)]
    public string Password { get; set; }

    [Required, Compare(nameof(Password)), MaxLength(50)]
    public string Verify { get; set; }

}
