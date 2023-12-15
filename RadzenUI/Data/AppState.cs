using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using RadzenUI.Pages.Auth.Models;
using System.Collections.ObjectModel;

namespace RadzenUI.Data;

public class AppState : IDisposable
{
	public static readonly string APPSTATE = "AppState";
	public static readonly string APPUSER = "AppUser";
	public static AppUser Anonymous { get; private set; } = new AppUser { Email = "", Id = "", Role = "", Username = "" };
    private readonly IDataProvider data;

    public event EventHandler<AppUser>? UserChanged;
    public event EventHandler<CalendarDTO?>? CalendarChanged;
	
	private AppUser _user = Anonymous;
    private CalendarDTO? _currentCalendar;

    public AppUser User { 
		get => _user; 
		set
		{
			_user = value;
			
			UserChanged?.Invoke(this, _user);
		}  
	}
    public CalendarDTO? CurrentCalendar { 
		get => _currentCalendar;
		set
		{
			_currentCalendar = value;
			data.SetUsersLastCalendar(User.Id!, value?.CalendarId ?? "");
			User.LastCalendar = value?.CalendarId ?? "";
			CalendarChanged?.Invoke(this, _currentCalendar);			
			_ = SaveToSession();
		}
	}

	public ProtectedSessionStorage? Session { get; set; } = null;
	public ObservableCollection<CalendarDTO> Calendars { get; set; } = new ObservableCollection<CalendarDTO>();



    public AppState(IDataProvider data)
    {
        this.data = data;
		UserChanged += OnUserChanged;
    }

	public void Initialize(ProtectedSessionStorage session)
	{
		Session = session;		
	}
	    
    private async Task GetUserData()
    {
        await LoadCalendars();

		// Set Current Calendar
				
		if(Calendars.Any(c => c.CalendarId == User.LastCalendar))
		{
			CurrentCalendar = Calendars.OrderBy(c => c.OwnerName)
				.OrderBy(c => c.CalendarName)
				.First(c => c.CalendarId == User.LastCalendar);
		}
		else if(Calendars.Any(c => c.OwnerId == User.Id)) 
		{
			CurrentCalendar = Calendars.OrderBy(c => c.OwnerName).First(c => c.OwnerId == User.Id);
		}
		else
		{
             CurrentCalendar = null;			
		}

        //todo finish this
    }

    public async Task LoadCalendars()
    {
        //  Load Calendars
        var cals = await data.GetCalendars(User.Id!);
        if (cals.Success)
        {
            Calendars.Clear();
            foreach (var cal in cals.Value)
            {
                Calendars.Add(cal);
            }
        }
    }

    public async Task LoadFromSession()
	{
		if(Session != null)
		{
			var r = await Session.GetAsync<AppUser>(APPUSER);
			if (r.Success)
			{
				User = r.Value!;
			}
		}
	}

    public async Task SaveToSession()
    {
       if(Session != null && User != null)
		{
			await Session.SetAsync(APPUSER, User);
		}
        
    }

	public async Task DeleteSessionData()
	{
		if(Session != null)
		{
			await Session.DeleteAsync(APPUSER);
		}
	}

	public void OnUserChanged(object? sender, AppUser user)
	{
        if (user.Id == "")
        {
            CurrentCalendar = null;
            Calendars.Clear();			
			_ = DeleteSessionData();			
        }
        else
        {
			_= GetUserData();
			_= SaveToSession();
        }
    }

	public void Dispose()
	{
        UserChanged -= OnUserChanged;
    }

}








	
