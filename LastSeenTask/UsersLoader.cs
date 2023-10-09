using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
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
    UserData LoadUsers(int offset);
}

public class UsersLoader : IUserDataLoader
{
    private readonly HttpClient _client;
    private readonly IHistoricalDataStorage _historicalDataStorage;

    public UsersLoader(HttpClient httpClient, IHistoricalDataStorage historicalDataStorage)
    {
        _client = httpClient;
        _historicalDataStorage = historicalDataStorage;
    }
    
    public UserData LoadUsers(int offset)
    {
        var apiUrl = $"https://sef.podkolzin.consulting/api/users/lastSeen?offset={offset}";
        var response = _client.GetAsync(apiUrl).Result;

        if (response.IsSuccessStatusCode)
        {
            var json = response.Content.ReadAsStringAsync().Result;
            var userData = JsonConvert.DeserializeObject<UserData>(json);
            UpdateHistoricalData(userData.data);
            return userData;
        }
        else
        {
            return new UserData();
        }
    }
    
    private void UpdateHistoricalData(User[] users)
    {
        var currentDate = DateTime.UtcNow;
        var countOfOnlineUsers = users.Count(user => user.LastSeenDate.HasValue && (currentDate - user.LastSeenDate.Value).TotalMinutes <= 60);
        _historicalDataStorage.UsersOnlineData[currentDate] = countOfOnlineUsers;
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