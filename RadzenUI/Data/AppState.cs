using Microsoft.AspNetCore.Mvc.TagHelpers;
using RadzenUI.Pages.Auth.Models;



namespace RadzenUI.Data;

public class AppState
{
	private AppUser? user;

	public AppUser? User { 
		get => user; 
		set
		{
			user = value;
			Persist();
		}  
	}



}

