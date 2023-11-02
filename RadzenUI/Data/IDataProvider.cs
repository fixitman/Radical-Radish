using RadzenUI.Pages.Auth;

namespace RadzenUI.Data
{
    public interface IDataProvider
	{
		public LoginResponse? ValidateUser(LoginRequest req);

		public User? AddUser(User user);

		public User GetUser(string username);

	}
}
