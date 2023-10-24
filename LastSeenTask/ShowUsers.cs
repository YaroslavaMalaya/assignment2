namespace LastSeenTask;

public class UserResponse
{
    public string Nickname { get; set; }
    public string LastSeen { get; set; }
}

public class ShowUsers
{
    private readonly IUserDataLoader usersLoader;
    private readonly ILastSeenFormatter formatter;
    private List<string> forgottenUsers;

    public ShowUsers(IUserDataLoader usersLoader, ILastSeenFormatter formatter, List<string> forgottenUsers)
    {
        this.usersLoader = usersLoader;
        this.formatter = formatter;
        this.forgottenUsers = forgottenUsers;
    }

    public List<UserResponse> UsersShow(string lang)
    {
        var usersResponses = new List<UserResponse>();

        var offset = 0;
        while (true)
        {
            var userData = usersLoader.LoadUsers(offset, forgottenUsers);
            var userCount = userData.data?.Length ?? 0;

            if (userCount == 0 || userData.data == null || userData.data.Length == 0)
            {
                break;
            }

            foreach (User user in userData.data)
            {
                if (!forgottenUsers.Contains(user.Nickname))
                {
                    var formattedLastSeen = formatter.Format(DateTimeOffset.Now, user.LastSeenDate.GetValueOrDefault(), lang);
                    usersResponses.Add(new UserResponse 
                    {
                        Nickname = user.Nickname, 
                        LastSeen = GenerateUserMessage(user, formattedLastSeen, lang)
                    });
                    ConsoleUser(user, formattedLastSeen, lang);
                }
            }
            offset += userCount;
        }
        return usersResponses;
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
    
    private string GenerateUserMessage(User user, string? lastSeen, string? lang)
    {
        if (user.LastSeenDate == null)
        {
            if (lang == "ua")
                return $"{user.Nickname} онлайн.";
            else if (lang == "es")
                return $"{user.Nickname} en línea.";
            else
                return $"{user.Nickname} is online.";
        }
        else
        {
            if (lang == "ua")
                return $"{user.Nickname} був/була онлайн {lastSeen}.";
            else if (lang == "es")
                return $"{user.Nickname} estaba en línea {lastSeen}.";
            else
                return $"{user.Nickname} was online {lastSeen}.";
        }
    }
}