namespace RadzenUI.Data;

public class Appointment
{
	public string Id { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string Text { get; set; }
    public bool IsAllDay { get; set; } = false;
    public string Color { get; set; } = "Green";
    public string CalendarId { get; set; }
}
