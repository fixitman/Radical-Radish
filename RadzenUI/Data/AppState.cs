using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using RadzenUI.Pages.Auth.Models;
using System.Collections.ObjectModel;

namespace RadzenUI.Data;

public class AppState
{
	public static readonly string APPSTATE = "AppState";
	public static readonly string APPUSER = "AppUser";
    private readonly IDataProvider data;

    public static AppUser Anonymous { get; private set; } = new AppUser { Email = "", Id = "", Role = "", Username = "" };
	private AppUser _user = Anonymous;
    private Calendar? _currentCalendar;

    public event EventHandler<AppUser>? UserChanged;
    public event EventHandler<Calendar?>? CalendarChanged;
	

	public ProtectedSessionStorage? Session { get; set; } = null;
	public ObservableCollection<CalendarDTO> Calendars { get; set; } = new ObservableCollection<CalendarDTO>();


    public AppUser User { 
		get => _user; 
		set
		{
			_user = value;
			if(User.Id == "")
			{
				CurrentCalendar = null;
				Calendars.Clear();
			}
			else
			{
				_ = GetUserData();
			}
			UserChanged?.Invoke(this, _user);
            _ = SaveToSession();	
		}  
	}

    public AppState(IDataProvider data)
    {
        this.data = data;
    }

    private async Task GetUserData()
    {
        await LoadCalendars();


        //  Set Current Calendar


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

    public Calendar? CurrentCalendar { 
		get => _currentCalendar;
		set
		{
			_currentCalendar = value;
			CalendarChanged?.Invoke(this, _currentCalendar);
			
			// Load Appointments
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
			await Session.SetAsync(APPUSER, User);
		}
        
    }

	public async Task InitializeAsync(ProtectedSessionStorage session)
	{
		Session = session;
		await LoadFromSession();
	}

    
}

//	ToDo listen for change in login state and load all data
//  Get/clear user, calendar list, current calendar and and appointments when user changes
//  todo load appointments when currentCalendar changes
//  



	
