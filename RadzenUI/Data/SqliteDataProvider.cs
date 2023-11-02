using Dapper;
using Microsoft.Data.Sqlite;
using RadzenUI.Pages.Auth;


namespace RadzenUI.Data;

public class SqliteDataProvider : IDataProvider
{
	private readonly IConfiguration _configuration;

	public SqliteDataProvider(IConfiguration configuration)
    {
		_configuration = configuration;
	}

	public User? AddUser(User user)
	{
		var sql = "insert into Users (Id, Username, PWHash, Email) values (@Id, @Username, @PWHash, @Email) Returning *;";
		using SqliteConnection conn = new SqliteConnection(_configuration.GetConnectionString("Default"));
		conn.Open();
		var inserted = conn.QuerySingle <User>(sql, new {Id = user.Id,Username = user.Username,PWHash = user.PWHash, Email = user.Email });
		return inserted;
	}

	public User? GetUser(string username)
	{
		var sql = "select * from Users where Username = @Username;";
		using SqliteConnection conn = new SqliteConnection(_configuration.GetConnectionString("Default"));
		conn.Open();
		var u = conn.QuerySingleOrDefault<User>(sql, new { Username = username});
		return u;
	}

	public LoginResponse? ValidateUser(LoginRequest req)
	{
		string sql = "select @ from Users where Username = @Username and Password = @Password LIMIT 1";
		using SqliteConnection conn = new SqliteConnection(_configuration.GetConnectionString("Default"));
		conn.Open();
		User? user = conn.QueryFirstOrDefault<User>(sql,req);
		if (user == null)
		{
			return null;
		}
		return new LoginResponse { Id = user.Id , Username = user.Username };

	}
	
}
