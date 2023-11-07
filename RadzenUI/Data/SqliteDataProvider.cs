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
			var sql = "select * from Users where Username = @Username;";
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

	public Result<LoginResponse> ValidateUser(LoginRequest req)
	{
		try
		{
			string sql = "select @ from Users where Username = @Username and Password = @Password LIMIT 1";
			using SqliteConnection conn = new SqliteConnection(_configuration.GetConnectionString("Default"));
			conn.Open();
			User? user = conn.QueryFirstOrDefault<User>(sql,req);
			conn.Close();
			return user is null ? Result.Empty<LoginResponse>() 
				:Result.Ok( new LoginResponse { Id = user.Id, Username = user.Username });
		}
		catch (Exception e)
		{			
			return Result.Fail<LoginResponse>(e.Message);			
		}	
		

	}
	
}
