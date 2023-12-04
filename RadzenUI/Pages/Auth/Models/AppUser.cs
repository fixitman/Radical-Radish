﻿namespace RadzenUI.Pages.Auth.Models;
using System.Security.Claims;

public class AppUser
{
    public string? Id { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? Role { get; set; }
    public string LastCalendar { get; set; }
    
}
