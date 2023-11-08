using RadzenUI.Pages.Auth.Models;

namespace RadzenUI.Data
{
    public interface IDataProvider
	{
			public Result<User> AddUser(User user);

		public Result<User> GetUser(string username);

	}
}
