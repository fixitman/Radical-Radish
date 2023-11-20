using RadzenUI.Pages.Auth.Models;

namespace RadzenUI.Data
{
    
	
	public interface IDataProvider
	{
		public Result<User> AddUser(User user);

		public Result<User> GetUser(string username);



		public Result<Calendar> AddCalendar(string userId, string? name);

		public Result<IEnumerable<CalendarDTO>> GetCalendars(string userId);

		public Result<CalendarRole> AddCalendarRole(string userId, string calenmdarId, string Role);

	}
}
