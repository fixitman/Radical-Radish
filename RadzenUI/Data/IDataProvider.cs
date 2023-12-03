using RadzenUI.Pages.Auth.Models;

namespace RadzenUI.Data
{
    
	
	public interface IDataProvider
	{
		public Task<Result<User>> AddUser(User user);

		public Task<Result<User>> GetUser(string username);



		public Task<Result<Calendar>> AddCalendar(string userId, string? name);

		public Task<Result<IEnumerable<CalendarDTO>>> GetCalendars(string userId);

		public Task<Result<CalendarRole>> AddCalendarRole(string userId, string calenmdarId, string Role);

	}
}
