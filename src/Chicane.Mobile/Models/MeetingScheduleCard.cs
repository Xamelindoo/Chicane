namespace Chicane.Mobile.Models;

public class MeetingScheduleCard
{
    public string MeetingName { get; init; } = string.Empty;
    public string CircuitName { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public DateTime DateStart { get; init; }
    public DateTime DateEnd { get; init; }
    public string CircuitImageUrl { get; init; } = string.Empty;
    public int MeetingKey { get; init; }
    public bool IsCompleted { get; init; }
    public List<SessionTime> Sessions { get; init; } = new();
    public List<Top3Driver> Top3Drivers { get; set; } = new();
}

public class SessionTime
{
    public string SessionName { get; init; } = string.Empty;
    public DateTime DateTime { get; init; }
    public int SessionKey { get; init; }
    public bool IsPast => DateTime < System.DateTime.UtcNow;
}

public class Top3Driver
{
    public string HeadshotUrl { get; init; } = string.Empty;
    public string Initials { get; init; } = string.Empty;
    public string TimeOrDnf { get; init; } = string.Empty;
    public int Position { get; init; }
}
