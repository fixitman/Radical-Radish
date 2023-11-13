using Dapper;
using Microsoft.Data.Sqlite;
using RadzenUI.Pages.Auth.Models;

namespace RadzenUI.Data;

public class SqliteDataProvider : IDataProvider
{
	private readonly IConfiguration _configuration;

	public SqliteDataProvider(IConfiguration configuration)
    {
		_configuration = configuration;
	}

	public Result<User> AddUser(User user)
	{
		var sql = "insert into Users (Id, Username, PWHash, Email) values (@Id, @Username, @PWHash, @Email) Returning *;";
		try
		{
			using SqliteConnection conn = new SqliteConnection(_configuration.GetConnectionString("Default"));
			conn.Open();
			var inserted = conn.QuerySingle <User>(sql, new {Id = user.Id,Username = user.Username,PWHash = user.PWHash, Email = user.Email });
			conn.Close();
			return Result.Ok(inserted);
		}
		catch (Exception e)
		{
			return Result.Fail<User>(e.Message);
		}
		
	}

	public Result<User> GetUser(string username)
	{
		try
		{
			var sql = "select * from Users where Lower(Username) = LOWER(@Username);";
			using SqliteConnection conn = new SqliteConnection(_configuration.GetConnectionString("Default"));
			conn.Open();
			var u = conn.QuerySingleOrDefault<User>(sql, new { Username = username});
			conn.Close();
			return u is null ? Result.Empty<User>(): Result.Ok<User>(u);
		}
		catch (Exception e)
		{
			return Result.Fail<User>(e.Message);			
		}
	}

	
	
}
