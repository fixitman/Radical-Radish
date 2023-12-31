﻿using Dapper;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace RadzenUI.Data;

public class SqliteDataProvider : IDataProvider
{
	private string connectionString;
	public SqliteDataProvider(IConfiguration configuration)
    {
		connectionString = configuration.GetConnectionString("Default");
	}


	// Users
	public Task<Result<User>> AddUser(User user)
	{
		using SqliteConnection conn = new SqliteConnection(connectionString);
		try
		{
			var sql = "insert into Users (Id, Username, PWHash, Email) values (@Id, @Username, @PWHash, @Email) Returning *;";
			conn.Open();
			var inserted = conn.QuerySingle <User>(sql, new {Id = user.Id,Username = user.Username,PWHash = user.PWHash, Email = user.Email });
			conn.Close();
			return Task.FromResult(Result.Ok(inserted));
		}
		catch (Exception e)
		{
			conn.Close();
			return Task.FromResult(Result.Fail<User>(e.Message));
		}
		
	}
    public Task<Result<User>> GetUser(string username)
	{
		using SqliteConnection conn = new SqliteConnection(connectionString);
		try
		{
			var sql = "select * from Users where Lower(Username) = LOWER(@Username);";
			conn.Open();
			var u = conn.QuerySingleOrDefault<User>(sql, new { Username = username});
			conn.Close();
			return u is null ? Task.FromResult(Result.Empty<User>()) : Task.FromResult(Result.Ok<User>(u));
		}
		catch (Exception e)
		{
            conn.Close();
            return Task.FromResult(Result.Fail<User>(e.Message));			
		}
	}
	public Task<Result> SetUsersLastCalendar(string userId, string calendarId)
	{
        using SqliteConnection conn = new SqliteConnection(connectionString);
        try
        {
            var sql = "UPDATE Users SET LastCalendar = @CalId WHERE Id = @UserId;";
            conn.Open();
            var updated = conn.ExecuteAsync (sql, new { UserId = userId, CalId = calendarId});
            conn.Close();
            return Task.FromResult(Result.Ok());
        }
        catch (Exception e)
        {
            conn.Close();
            return Task.FromResult(Result.Fail(e.Message));
        }
    }


	//Calendars
    public Task<Result<Calendar>> AddCalendar(string userId, string? name = "Default")
    {
		using SqliteConnection conn = new SqliteConnection(connectionString);
		conn.Open();
		var transaction = conn.BeginTransaction();
        try
        {
			var sql = "insert into Calendars (Id, Name) values (@Id, @Name) returning *;";
			var c = conn.QuerySingleOrDefault<Calendar>(sql, new { Id = Guid.NewGuid().ToString(), Name = name });
			if(c == null) 
			{ 
				transaction.Rollback();
				conn.Close();
				return Task.FromResult(Result.Fail<Calendar>("Error Inserting Calendar"));
			}

			sql = "insert into CalendarRoles (Role, UserId, CalendarId) values (@Role, @UserId, @CalendarId) returning *";
			var r = conn.QuerySingleOrDefault<CalendarRole>(sql, new { Role = "OWNER", UserId = userId, CalendarId = c.Id });
			if (r == null)
			{
				transaction.Rollback();
				conn.Close();
				return Task.FromResult(Result.Fail<Calendar>("Error Inserting CalendarRole"));
			}

			sql = "UPDATE Users SET LastCalendar = @CalId WHERE Id = @Id;";
            var u = conn.Execute(sql, new { Id = userId, CalId = c.Id });
            if (u != 1)
            {
				transaction.Rollback();
				conn.Close();
                return Task.FromResult(Result.Fail<Calendar>("Error Updating LastCalendar"));
            }

			transaction.Commit();
			conn.Close();
			return Task.FromResult(Result.Ok<Calendar>(c));

        }
        catch (Exception e)
        {
			transaction.Rollback();
			conn.Close();
            return Task.FromResult(Result.Fail<Calendar>(e.Message));
        }
    }
	public Task<Result<CalendarRole>> AddCalendarRole(string userId, string calenmdarId, string role)
	{
		using SqliteConnection conn = new SqliteConnection(connectionString);
		try
		{
			var sql = "insert into CalendarRoles (Role, UserId, CalendarId) values (@Role, @UserId, @CalendarId) returning *";
			var r = conn.QuerySingleOrDefault<CalendarRole>(sql, new { Role = role, UserId = userId, CalendarId = calenmdarId });
			if (r == null)
			{				
				conn.Close();
				return Task.FromResult(Result.Fail<CalendarRole>("Error Inserting CalendarRole"));
			}
			return Task.FromResult(Result.Ok<CalendarRole>(r));
		}
		catch (Exception e)
		{
			conn.Close();
			return Task.FromResult(Result.Fail<CalendarRole>(e.Message));
		}
	}
	public Task<Result<IEnumerable<CalendarDTO>>> GetCalendars(string _userId)
	{
			var sql = 
@"SELECT Calendars.id as CalendarId, Calendars.Name as CalendarName, CalendarRoles.Role as CalendarRole, OwnerId, OwnerName
FROM Calendars inner join CalendarRoles on CalendarRoles.CalendarId = Calendars.Id
inner join 
	(SELECT Users.Id as OwnerId, Users.Username as OwnerName, CalendarRoles.CalendarId as OwnedCalendarId
	from CalendarRoles Inner Join Users on Users.Id = CalendarRoles.UserId
	where CalendarRoles.Role = 'OWNER') 
on Calendars.Id = OwnedCalendarId
where CalendarRoles.UserId = @UserId;";

		using SqliteConnection conn = new SqliteConnection(connectionString);
		try
		{
			conn.Open();
			var u = conn.Query<CalendarDTO>(sql, new { UserId = _userId });
			conn.Close();
			return u is	null ? Task.FromResult(Result.Empty<IEnumerable<CalendarDTO>>()) : Task.FromResult(Result.Ok<IEnumerable<CalendarDTO>>(u));
		}
		catch (Exception e)
		{
			conn.Close();
			return Task.FromResult(Result.Fail<IEnumerable<CalendarDTO>>(e.Message));
        }
    }


	// Appointments
    public Task<Result<IEnumerable<Appointment>>> GetAppointments(string calendarId, DateTime start, DateTime end)
	{
        var sql = @"SELECT * from Appointments where (CalendarId = @CalId) AND
	((Start BETWEEN @Start AND @End) or (End BETWEEN @Start AND @End))";

        using SqliteConnection conn = new SqliteConnection(connectionString);
        try
        {
            conn.Open();
            var u = conn.Query<Appt>(sql, new {CalId = calendarId, Start = start.Ticks, End = end.Ticks });
            conn.Close();
			if(u is null)
			{
				return Task.FromResult(Result.Empty<IEnumerable<Appointment>>());
            }
			else
			{
				var appointments = u.Select(a => a.ToAppointment());
				
				return  Task.FromResult(Result.Ok<IEnumerable<Appointment>>(appointments));
			}
        }
        catch (Exception e)
        {
            conn.Close();
            return Task.FromResult(Result.Fail<IEnumerable<Appointment>>(e.Message));
        }
    }
    public Task<Result<Appointment>> AddAppointment(Appointment appointment)
	{
        using SqliteConnection conn = new SqliteConnection(connectionString);
        try
        {
            var sql = @"INSERT INTO Appointments 
					( Id, CalendarId, Text, Color, IsAllDay, Start, End) VALUES 
					(  @Id, @CalendarId, @Text, @Color, @IsAllDay, @Start, @End) returning *";

			var appt = Appt.FromAppointment(appointment);
			appt.Id = Guid.NewGuid().ToString();
            
			var r = conn.QuerySingleOrDefault<Appt>(sql, appt);
            if (r == null)
            {
                conn.Close();
                return Task.FromResult(Result.Fail<Appointment>("Error Creating Appointment"));
            }
			conn.Close ();
            return Task.FromResult(Result.Ok<Appointment>( r.ToAppointment()));
        }
        catch (Exception e)
        {
            conn.Close();
            return Task.FromResult(Result.Fail<Appointment>(e.Message));
        }
    }
    public Task<Result<Appointment>> UpdateAppointment(Appointment appointment)
	{
        using SqliteConnection conn = new SqliteConnection(connectionString);
        try
        {
            var sql = @"UPDATE Appointments set 
				Start = @Start,
				End = @End,
				Text = @Text,
				IsAllDay = @IsAllDay,
				CalendarId = @CalendarId,
				Color = @Color
				where Id = @Id returning *";

            var appt = Appt.FromAppointment(appointment);            

            var r = conn.QuerySingleOrDefault<Appt>(sql, appt);
            if (r == null)
            {
                conn.Close();
                return Task.FromResult(Result.Fail<Appointment>("Error Updating Appointment"));
            }
            conn.Close();
            return Task.FromResult(Result.Ok<Appointment>(r.ToAppointment()));
        }
        catch (Exception e)
        {
            conn.Close();
            return Task.FromResult(Result.Fail<Appointment>(e.Message));
        }
    }
    public Task<Result> DeleteAppointment(Appointment appointment)
	{
        using SqliteConnection conn = new SqliteConnection(connectionString);
        try
        {
            var sql = @"DELETE FROM Appointments WHERE Id = @Id;";

			var r = conn.Execute(sql, new {Id = appointment.Id});
            if (r < 1)
            {
                conn.Close();
                return Task.FromResult(Result.Fail("Error Deleting Appointment"));
            }
            conn.Close();
            return Task.FromResult(Result.Ok());
        }
        catch (Exception e)
        {
            conn.Close();
            return Task.FromResult(Result.Fail(e.Message));
        }
    }

    private class Appt
	{
        public string Id { get; set; }
        public long Start { get; set; }
        public long End { get; set; }
        public string Text { get; set; }
        public bool IsAllDay { get; set; } = false;
        public string Color { get; set; } = "Green";
		public string CalendarId { get; set; }

		public Appointment ToAppointment()
		{
			return new Appointment
			{
				Id = this.Id,
				CalendarId = this.CalendarId,
				Color = this.Color,
				IsAllDay = this.IsAllDay,
				Text = this.Text,
				Start = new DateTime(this.Start),
				End = new DateTime(this.End)
			};
		}

		public static Appt FromAppointment(Appointment a)
		{
			return new Appt
			{
				Id = a.Id,
				CalendarId = a.CalendarId,
				Color = a.Color,
				IsAllDay = a.IsAllDay,
				Text = a.Text,
				Start = a.Start.Ticks,
				End = a.End.Ticks
			};
		}





    }
}


//var sql = @"SELECT Calendars.Id as CalendarId, Calendars.Name as CalendarName, Users.Id as UserId, Users.Username,CalendarRoles.Role as CalendarRole
//FROM Calendars INNER JOIN CalendarRoles on Calendars.Id = CalendarRoles.CalendarId
//INNER JOIN Users ON CalendarRoles.UserId = Users.Id
//Where CalendarRoles.UserId = @UserId;";



