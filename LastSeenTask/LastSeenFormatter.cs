using LastSeenTask;
namespace LastSeenTask;

public class LastSeenFormatter
{
    private ChangeLanguage localization = new ChangeLanguage();
    
    public string Format(DateTimeOffset now, DateTimeOffset lastSeen, string? language)
    {
        localization.ChangeLang(language);
        
        var span = now - lastSeen;
        if (span.TotalSeconds == 0)
        {
            return localization.result1;
        }
        else if (span.TotalSeconds <= 30)
        {
            return localization.result2;
        }
        else if (span.TotalSeconds <= 60)
        {
            return localization.result3;
        }
        else if (span.TotalMinutes <= 59)
        {
            return localization.result4;
        }
        else if (span.TotalMinutes <= 119)
        {
            return localization.result5;
        }
        else if (span.TotalMinutes <= 23 * 60)
        {
            return localization.result6;
        }
        else if (span.TotalMinutes <= 47 * 60)
        {
            return localization.result7;
        }
        else if (span.TotalDays < 7)
        {
            return localization.result8;
        }
        else
        {
            return localization.result9;
        }
    }
}