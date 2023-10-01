using Newtonsoft.Json;
namespace  LastSeenTask;

public class UserData
{
    public User[] data { get; set; }
}

public class User
{
    public DateTimeOffset? LastSeenDate { get; set; }
    public string Nickname { get; set; }
    public bool IsOnline { get; set; }
}

public interface IUserDataLoader
{
    UserData LoadUsers(int offset);
}

public class UsersLoader : IUserDataLoader
{
    private readonly HttpClient _client;
    private readonly LastSeenFormatter _formatter;
 
    public UsersLoader(HttpClient httpClient)
    {
        _client = httpClient;
        _formatter = new LastSeenFormatter();
    }

    public UserData LoadUsers(int offset)
    {
        var apiUrl = $"https://sef.podkolzin.consulting/api/users/lastSeen?offset={offset}";
        var response = _client.GetAsync(apiUrl).Result;

        if (response.IsSuccessStatusCode)
        {
            var json = response.Content.ReadAsStringAsync().Result;
            var userData = JsonConvert.DeserializeObject<UserData>(json);
            return userData;
        }
        else
        {
            return new UserData();
        }
    }
}