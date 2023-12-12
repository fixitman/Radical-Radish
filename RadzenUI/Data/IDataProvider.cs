using RadzenUI.Pages.Auth.Models;

namespace RadzenUI.Data
{
    
	
	public interface IDataProvider
	{
		public Task<Result<User>> AddUser(User user);

		public Task<Result<User>> GetUser(string username);

		public Task<Result> SetUsersLastCalendar(string userId, string calendarId);



		public Task<Result<Calendar>> AddCalendar(string userId, string? name);

		public Task<Result<IEnumerable<CalendarDTO>>> GetCalendars(string userId);

		public Task<Result<CalendarRole>> AddCalendarRole(string userId, string calenmdarId, string Role);



		public Task<Result<IEnumerable<Appointment>>> GetAppointments(string calendarId, DateTime start, DateTime end);
		public Task<Result<Appointment>> AddAppointment(Appointment appointment);
		public Task<Result<Appointment>> UpdateAppointment(Appointment appointment);
		public Task<Result> DeleteAppointment(Appointment appointment);
    }
}
