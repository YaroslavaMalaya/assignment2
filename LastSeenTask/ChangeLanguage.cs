namespace LastSeenTask;

public class ChangeLanguage
{
    public string result1 = "online";                 
    public string result2 = "just now";               
    public string result3 = "less than a minute ago"; 
    public string result4 = "couple of minutes ago";  
    public string result5 = "an hour ago";            
    public string result6 = "today";                  
    public string result7 = "yesterday";              
    public string result8 = "this week";              
    public string result9= "long time ago";           
    
    private void ChangeLanguageUA()
    {
        result1 = "онлайн";
        result2 = "тільки що";
        result3 = "менше хвилини тому";
        result4 = "декілька хвилин тому";
        result5 = "годину тому";
        result6 = "сьогодні";
        result7 = "вчора";
        result8 = "на цьому тижні";
        result9= "давно";
    }
    
    private void ChangeLanguageES()
    {
        result1 = "en línea";
        result2 = "en este momento";
        result3 = "hace menos de un minuto";
        result4 = "hace un par de minutos";
        result5 = "hace una hora";
        result6 = "hoy";
        result7 = "ayer";
        result8 = "esta semana";
        result9 = "hace mucho tiempo";
    }

    public void ChangeLang(string? language)
    {
        if (language == "ua")
        {
            ChangeLanguageUA();
        }
        else if (language == "es")
        {
            ChangeLanguageES();
        }
        else if (language == "en")
        {
        }
    }
}