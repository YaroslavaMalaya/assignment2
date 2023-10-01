namespace LastSeenTask;

public class LoadUsers
{
    private readonly UsersLoader _usersLoader;
    private readonly LastSeenFormatter _formatter;

    public LoadUsers(UsersLoader usersLoader, LastSeenFormatter formatter)
    {
        _usersLoader = usersLoader ?? throw new ArgumentNullException(nameof(usersLoader));
        _formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
    }

    public void usersShow(string lang)
    {
        var offset = 0;

        while (true)
        {
            var userData = _usersLoader.LoadUsers(offset);
            var userCount = userData.data?.Length ?? 0;

            if (userCount == 0 || userData.data == null || userData.data.Length == 0)
            {
                break;
            }

            foreach (User user in userData.data)
            {
                var formattedLastSeen = _formatter.Format(DateTimeOffset.Now, user.LastSeenDate.GetValueOrDefault(), lang);
                ConsoleUser(user, formattedLastSeen, lang);
            }

            offset += userCount;
        }
    }

    private void ConsoleUser(User user, string? lastSeen, string? lang)
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
}