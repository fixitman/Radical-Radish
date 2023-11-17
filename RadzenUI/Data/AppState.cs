using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using RadzenUI.Pages.Auth.Models;



namespace RadzenUI.Data;

public class AppState
{
	private static readonly string APPUSER = "AppUser";
	
	private AppUser? user;

	public ProtectedSessionStorage? Session { get; set; } = null;

    public AppUser? User { 
		get => user; 
		set
		{
			user = value;
			Save();	
		}  
	}

	public async Task Load()
	{
		if(Session != null)
		{
			var r = await Session.GetAsync<AppUser>(APPUSER);
			if(r.Success) user = r.Value;
		}
	}

    public async Task Save()
    {
       if(Session != null && User != null)
		{
			await Session.SetAsync(APPUSER,User);
		}
        
    }

	public async Task InitializeAsync(ProtectedSessionStorage session)
	{
		Session = session;
		await Load();
	}

}

