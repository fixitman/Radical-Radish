﻿namespace RadzenUI.Data;

public class User
{
    public string Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PWHash { get; set; }
    public string LastCalendar { get; set; } = "";   
    public string AppRole { get; set; }
    
}