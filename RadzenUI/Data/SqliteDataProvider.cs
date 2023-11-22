using Dapper;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Globalization;

namespace RadzenUI.Data;

public class SqliteDataProvider : IDataProvider
{
	private string connectionString;
	public SqliteDataProvider(IConfiguration configuration)
    {
		connectionString = configuration.GetConnectionString("Default");
	}


	// Users
	public Result<User> AddUser(User user)
	{
		using SqliteConnection conn = new SqliteConnection(connectionString);
		try
		{
			var sql = "insert into Users (Id, Username, PWHash, Email) values (@Id, @Username, @PWHash, @Email) Returning *;";
			conn.Open();
			var inserted = conn.QuerySingle <User>(sql, new {Id = user.Id,Username = user.Username,PWHash = user.PWHash, Email = user.Email });
			conn.Close();
			return Result.Ok(inserted);
		}
		catch (Exception e)
		{
			conn.Close();
			return Result.Fail<User>(e.Message);
		}
		
	}
    public Result<User> GetUser(string username)
	{
		using SqliteConnection conn = new SqliteConnection(connectionString);
		try
		{
			var sql = "select * from Users where Lower(Username) = LOWER(@Username);";
			conn.Open();
			var u = conn.QuerySingleOrDefault<User>(sql, new { Username = username});
			conn.Close();
			return u is null ? Result.Empty<User>(): Result.Ok<User>(u);
		}
		catch (Exception e)
		{
            conn.Close();
            return Result.Fail<User>(e.Message);			
		}
	}


	//Calendars
    public Result<Calendar> AddCalendar(string userId, string? name = "Default")
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
				return Result.Fail<Calendar>("Error Inserting Calendar");
			}

			sql = "insert into CalendarRoles (Role, UserId, CalendarId) values (@Role, @UserId, @CalendarId) returning *";
			var r = conn.QuerySingleOrDefault<CalendarRole>(sql, new { Role = "OWNER", UserId = userId, CalendarId = c.Id });
			if (r == null)
			{
				transaction.Rollback();
				conn.Close();
				return Result.Fail<Calendar>("Error Inserting CalendarRole");
			}

			sql = "UPDATE Users SET LastCalendar = @CalId WHERE Id = @Id;";
            var u = conn.Execute(sql, new { Id = userId, CalId = c.Id });
            if (u != 1)
            {
				transaction.Rollback();
				conn.Close();
                return Result.Fail<Calendar>("Error Updating LastCalendar");
            }

			transaction.Commit();
			conn.Close();
			return Result.Ok<Calendar>(c);

        }
        catch (Exception e)
        {
			transaction.Rollback();
			conn.Close();
            return Result.Fail<Calendar>(e.Message);
        }
    }

	public Result<CalendarRole> AddCalendarRole(string userId, string calenmdarId, string role)
	{
		using SqliteConnection conn = new SqliteConnection(connectionString);
		try
		{
			var sql = "insert into CalendarRoles (Role, UserId, CalendarId) values (@Role, @UserId, @CalendarId) returning *";
			var r = conn.QuerySingleOrDefault<CalendarRole>(sql, new { Role = role, UserId = userId, CalendarId = calenmdarId });
			if (r == null)
			{				
				conn.Close();
				return Result.Fail<CalendarRole>("Error Inserting CalendarRole");
			}
			return Result.Ok<CalendarRole>(r);
		}
		catch (Exception e)
		{
			conn.Close();
			return Result.Fail<CalendarRole>(e.Message);
		}
	}

	public Result<IEnumerable<CalendarDTO>> GetCalendars(string _userId)
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
			return u is null ? Result.Empty<IEnumerable<CalendarDTO>>() : Result.Ok<IEnumerable<CalendarDTO>>(u);
		}
		catch (Exception e)
		{
			conn.Close();
			return Result.Fail<IEnumerable<CalendarDTO>>(e.Message);
        }
    }
}


//var sql = @"SELECT Calendars.Id as CalendarId, Calendars.Name as CalendarName, Users.Id as UserId, Users.Username,CalendarRoles.Role as CalendarRole
//FROM Calendars INNER JOIN CalendarRoles on Calendars.Id = CalendarRoles.CalendarId
//INNER JOIN Users ON CalendarRoles.UserId = Users.Id
//Where CalendarRoles.UserId = @UserId;";



