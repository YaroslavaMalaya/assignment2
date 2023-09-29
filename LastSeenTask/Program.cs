using Newtonsoft.Json;
using LastSeenTask;

var offset = 0;
using (HttpClient client = new HttpClient())
{
    while (true)
    {
        var apiUrl = $"https://sef.podkolzin.consulting/api/users/lastSeen?offset={offset}";
        var response = client.GetAsync(apiUrl).Result;
        if (response.IsSuccessStatusCode)
        {
            var json =  response.Content.ReadAsStringAsync().Result;
            var userData = JsonConvert.DeserializeObject<UserData>(json);
            var formatter = new LastSeenFormatter();
            if (userData.data == null || userData.data.Length == 0)
            {
                return;
            }
            
            foreach (User user in userData.data)
            {
                var formattedLastSeen = formatter.Format(DateTimeOffset.Now, user.LastSeenDate.GetValueOrDefault());
                Console.WriteLine($"Nickname: {user.Nickname}");
                Console.WriteLine($"Last Seen: {formattedLastSeen}");
                Console.WriteLine($"Is Online: {user.IsOnline}\n");
            }
            offset += 5;
        }
        else
        {
            break;
        }
    }
}
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