using RadzenUI.Pages.Auth;

namespace RadzenUI.Data
{
    public interface IDataProvider
	{
		public LoginResponse? ValidateUser(LoginRequest req);

		public RegisterResponse? RegisterUser(RegisterRequest req);

	}
}
