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

	public RegisterResponse? RegisterUser(RegisterRequest req)
	{
		if(req.Password != req.Verify)
		{
			return null;
		}
		
		string sql = "select count(*) from users where Username = @Username;";
		using SqliteConnection conn = new SqliteConnection(_configuration.GetConnectionString("Default"));
		conn.Open();
		var c = conn.ExecuteScalar<int>(sql, new { Username = req.Username });
		if(c > 0)
		{
			conn.Close();
			return null;
		}

		sql = "insert into Users (Id, Username, Password, Email) values (@Id, @Username, @Password, @Email);";
		User newUser = new User()
		{
			Id = new Guid(),
			Email = req.Email,
			Username = req.Username,
			PWHash = BCrypt.Net.BCrypt.HashPassword(req.Password)

		};
		c = conn.ExecuteScalar<int>(sql, newUser);
		conn.Close();
		if(c < 1)
		{
			return null;
		}
		return new RegisterResponse { Id = newUser.Id, Username = newUser.Username };
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
