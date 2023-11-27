using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using RadzenUI.Pages.Auth.Models;

namespace RadzenUI.Data;

public class AppState
{
	private static readonly string APPSTATE = "AppState";
	
	public static AppUser Anonymous { get; private set; } = new AppUser { Email = "", Id = "", Role = "", Username = "" };
	private AppUser _user = Anonymous;
    private Calendar? _currentCalendar;

    public event EventHandler<AppUser>? UserChanged;
    public event EventHandler<Calendar?>? CalendarChanged;

	public ProtectedSessionStorage? Session { get; set; } = null;

    public Calendar? CurrentCalendar { 
		get => _currentCalendar;
		set
		{
			_currentCalendar = value;
			CalendarChanged?.Invoke(this, _currentCalendar);
			_ = SaveToSession();
		}
	} 

    public AppUser User { 
		get => _user; 
		set
		{
			_user = value;
			UserChanged?.Invoke(this, _user);
            _ = SaveToSession();	
		}  
	}

    public async Task LoadFromSession()
	{
		if(Session != null)
		{
			var r = await Session.GetAsync<AppState>(APPSTATE);
			if (r.Success)
			{
				_user = r.Value!.User;
				_currentCalendar = r.Value.CurrentCalendar;

			}
		}
	}

    public async Task SaveToSession()
    {
       if(Session != null && User != null)
		{
			await Session.SetAsync(APPSTATE, this);
		}
        
    }

	public async Task InitializeAsync(ProtectedSessionStorage session)
	{
		Session = session;
		await LoadFromSession();
	}

}

