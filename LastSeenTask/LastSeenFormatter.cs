namespace LastSeenTask;

public class LastSeenFormatter
{
    public string Format(DateTimeOffset now, DateTimeOffset lastSeen)
    {
        var span = now - lastSeen;
        if (span == TimeSpan.Zero)
        {
            return "online";
        }
        else if (span < TimeSpan.FromSeconds(30))
        {
            return "just now";
        }
        else if (now.Date == lastSeen.Date)
        {
            return "today";
        }
        else if (now.Date - lastSeen.Date == TimeSpan.FromDays(1))
        {
            return "yesterday";
        }
        else if (span < TimeSpan.FromDays(7))
        {
            return "this week";
        }
        else
            return "long time ago";
    }
}