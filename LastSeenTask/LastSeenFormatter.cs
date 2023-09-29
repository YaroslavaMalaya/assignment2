namespace LastSeenTask;
public class LastSeenFormatter
{
    public string Format(DateTimeOffset now, DateTimeOffset lastSeen)
    {
        var span = now - lastSeen;
        if (span.TotalSeconds == 0)
        {
            return "online";
        }
        else if (span.TotalSeconds <= 30)
        {
            return "just now";
        }
        else if (span.TotalSeconds <= 60)
        {
            return "less than a minute ago";
        }
        else if (span.TotalMinutes <= 59)
        {
            return "couple of minutes ago";
        }
        else if (span.TotalMinutes <= 119)
        {
            return "an hour ago";
        }
        else if (span.TotalMinutes <= 23 * 60)
        {
            return "today";
        }
        else if (span.TotalMinutes <= 47 * 60)
        {
            return "yesterday";
        }
        else if (span.TotalDays < 7)
        {
            return "this week";
        }
        else
        {
            return "long time ago";
        }
    }
}