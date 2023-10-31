using Dapper;
using Microsoft.Data.Sqlite;
using WebApplication2.Models;

namespace WebApplication2;

public class AuthData
{
    private string ConnectionString { get; }
    

    public AuthData(string connectionString)
    {
        ConnectionString = connectionString;
    }


    public List<string> GetData()
    {
        var sql = "select * from users";
        using var conn = new SqliteConnection(ConnectionString);
        var users = conn.Query<UserModel>(sql);
        var names = users.Select(x => x.UserName).ToList();
        return names;
    }

    public void AddUser(UserModel userModel)
    {
        var sql = "insert into users (username, password) values (@UserName, @Password);";
        using var conn = new SqliteConnection(ConnectionString);
        conn.ExecuteAsync(sql,userModel);
    }

    public UserModel? GetUserByName(string username) {
        var sql = "select * from users where Username = @UserName;";
        using var conn = new SqliteConnection(ConnectionString);
        var user = conn.QueryFirstOrDefault<UserModel>(sql, new {UserName = username});
        
        return user;
    }
}
