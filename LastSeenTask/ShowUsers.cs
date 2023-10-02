using System;

namespace LastSeenTask;

public class ShowUsers
{
    private readonly IUserDataLoader usersLoader;
    private readonly ILastSeenFormatter formatter;

    public ShowUsers(IUserDataLoader usersLoader, ILastSeenFormatter formatter)
    {
        this.usersLoader = usersLoader;
        this.formatter = formatter;
    }

    public void UsersShow(string lang)
    {
        var offset = 0;

        while (true)
        {
            var userData = usersLoader.LoadUsers(offset);
            var userCount = userData.data?.Length ?? 0;

            if (userCount == 0 || userData.data == null || userData.data.Length == 0)
            {
                break;
            }

            foreach (User user in userData.data)
            {
                var formattedLastSeen = formatter.Format(DateTimeOffset.Now, user.LastSeenDate.GetValueOrDefault(), lang);
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