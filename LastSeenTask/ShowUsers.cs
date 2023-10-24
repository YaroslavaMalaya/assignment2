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
        var currentDate = DateTime.UtcNow;
        
        var offset = 0;
        while (true)
        {
            var userData = usersLoader?.LoadUsers(offset, forgottenUsers, currentDate);
            if(userData == null) return new List<UserResponse>();
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
                        LastSeen = GenerateAndPrintUserMessage(user, formattedLastSeen, lang)
                    });
                }
            } 
            offset += userCount;
        }
        return usersResponses;
    }

    private string GenerateAndPrintUserMessage(User user, string? lastSeen, string? lang)
    {
        string message;
    
        if (user.LastSeenDate == null)
        {
            if (lang == "ua")
                message = $"{user.Nickname} онлайн.";
            else if (lang == "es")
                message = $"{user.Nickname} en línea.";
            else
                message = $"{user.Nickname} is online.";
        }
        else
        {
            if (lang == "ua")
                message = $"{user.Nickname} був/була онлайн {lastSeen}.";
            else if (lang == "es")
                message = $"{user.Nickname} estaba en línea {lastSeen}.";
            else
                message = $"{user.Nickname} was online {lastSeen}.";
        }
    
        Console.WriteLine(message);
        return message;
    }
}