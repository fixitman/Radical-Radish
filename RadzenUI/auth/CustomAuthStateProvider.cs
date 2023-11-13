using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using static Dapper.SqlMapper;

namespace RadzenUI.auth;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
	private ClaimsPrincipal CurrentUser;

    public CustomAuthStateProvider()
    {
        CurrentUser = new ClaimsPrincipal(new ClaimsIdentity());
    }

    public Task<AuthenticationState> ChangeUser(string Id,  string UserName, string Role)
    {
        var identity = new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, UserName),
            new Claim(ClaimTypes.Sid, Id),
            new Claim(ClaimTypes.Role, Role)
        },"Custom");
        CurrentUser = new ClaimsPrincipal(identity);
        var task = GetAuthenticationStateAsync();
        NotifyAuthenticationStateChanged(task);
        return task;
    }

    public Task<AuthenticationState> Logout()
    {
		CurrentUser = new ClaimsPrincipal();
		var task = GetAuthenticationStateAsync();
		NotifyAuthenticationStateChanged(task);
		return task;
	}

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
	{
		return Task.FromResult<AuthenticationState>(new AuthenticationState(CurrentUser));
	}
}
