using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RadzenUI.Pages.Auth.Models
{
    public class LoginRequest
    {
        [Required, StringLength(50)]
        public string Username { get; set; }
        [Required, StringLength(50), PasswordPropertyText]
        public string Password { get; set; }
    }
}
