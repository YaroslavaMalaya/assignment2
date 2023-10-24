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
}

public interface IUserDataLoader
{
    UserData LoadUsers(int offset, List<string> forgottenUsers, DateTime currentDate);
}

public class UsersLoader : IUserDataLoader
{
    private readonly HttpClient _client;
    private readonly IHistoricalDataStorage _historicalDataStorage;
    private readonly IHistoricalDataStorageConcrete _historicalDataStorageConcrete;

    public UsersLoader(HttpClient httpClient, IHistoricalDataStorage historicalDataStorage, 
        IHistoricalDataStorageConcrete historicalDataStorageConcrete)
    {
        _client = httpClient;
        _historicalDataStorage = historicalDataStorage;
        _historicalDataStorageConcrete = historicalDataStorageConcrete;
    }
    
    public UserData LoadUsers(int offset, List<string> forgottenUsers, DateTime currentDate)
    {
        var apiUrl = $"https://sef.podkolzin.consulting/api/users/lastSeen?offset={offset}";
        var response = _client.GetAsync(apiUrl).Result;

        if (response.IsSuccessStatusCode)
        {
            var json = response.Content.ReadAsStringAsync().Result;
            var userData = JsonConvert.DeserializeObject<UserData>(json);
            UpdateHistoricalData(userData.data, forgottenUsers, currentDate);
            return userData;
        }
        else
        {
            return new UserData();
        }
    }
    
    private void UpdateHistoricalData(User[] users, List<string> forgottenUsers, DateTime currentDate)
    {
        foreach (var user in users)
        {
            if (!forgottenUsers.Contains(user.Nickname))
                _historicalDataStorageConcrete.AddUserData(currentDate, user);
        }
        var countOfOnlineUsers = users.Count(user => user.LastSeenDate.HasValue && 
                                                     (currentDate - user.LastSeenDate.Value).TotalMinutes <= 60);
        if (_historicalDataStorage.UsersOnlineData.ContainsKey(currentDate.Date))
            _historicalDataStorage.UsersOnlineData[currentDate.Date] += countOfOnlineUsers;
        else
        {
            _historicalDataStorage.UsersOnlineData[currentDate.Date] = countOfOnlineUsers;
        }
    }
}

public class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly HttpResponseMessage _responseMessage;
    public MockHttpMessageHandler(HttpResponseMessage responseMessage)
    {
        _responseMessage = responseMessage;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_responseMessage);
    }
}