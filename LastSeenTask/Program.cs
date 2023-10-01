using LastSeenTask;

Console.WriteLine($"Which language are you prefer? (You can choose en, ua, es. The default language is English.): ");
var lang = Console.ReadLine();

using (HttpClient client = new HttpClient())
{
    var usersLoader = new UsersLoader(client);
    var offset = 0;

    while (true)
    {
        var userData = usersLoader.LoadUsers(offset);
        var userCount = userData.data?.Length ?? 0;

        if (userCount == 0)
        {
            break;
        }
        
        if (userData.data == null || userData.data.Length == 0)
        {
            break;
        }

        var formatter = new LastSeenFormatter();
        foreach (User user in userData.data)
        {
            var formattedLastSeen = formatter.Format(DateTimeOffset.Now, user.LastSeenDate.GetValueOrDefault(), lang);
            console(user, formattedLastSeen, lang);
        }

        offset += userCount;
    }
}

void console(User user, string? lastSeen, string? lang)
{
    if (user.LastSeenDate == null)
    {
        if (lang == "ua")
            Console.WriteLine($"{user.Nickname} онлайн.");
        else if (lang == "es")
            Console.WriteLine($"{user.Nickname} en línea.");
        else
            Console.WriteLine($"{user.Nickname} is online.");
    }
    else
    {
        if (lang == "ua")
            Console.WriteLine($"{user.Nickname} був/була онлайн {lastSeen}.");
        else if (lang == "es")
            Console.WriteLine($"{user.Nickname} estaba en línea {lastSeen}.");
        else
            Console.WriteLine($"{user.Nickname} was online {lastSeen}.");
    }
}