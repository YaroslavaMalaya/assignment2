using System.Net;
using Newtonsoft.Json;
using LastSeenTask;
using NUnit.Framework;
namespace LastSeenTaskTests;

public class E2ETests
{
    private readonly HttpClient _client;
    
    public E2ETests()
    {
        _client = new HttpClient { BaseAddress = new System.Uri("http://localhost:5000") };
    }

    [Test]
    public async Task TestGetUserOnlineData()
    {
        var response = await _client.GetAsync("/api/stats/user?date=2023-10-19T00:00:00.000Z&userId=Margarita85");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var responseContent = await response.Content.ReadAsStringAsync();
        var userOnlineData = JsonConvert.DeserializeObject<UserOnlineData>(responseContent);

        Assert.NotNull(userOnlineData);
    }

    [Test]
    public async Task TestGetUserTotalOnlineTime()
    {
        var response = await _client.GetAsync("/api/stats/user/total?userId=Margarita85");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var responseContent = await response.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<Dictionary<string, long>>(responseContent);

        Assert.True(data["totalTime"] >= 0);
    }

    [Test]
    public async Task TestGetUserAverageOnlineTime()
    {
        var response = await _client.GetAsync("/api/stats/user/average?userId=Margarita85");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var responseContent = await response.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<Dictionary<string, long>>(responseContent);

        Assert.True(data["weeklyAverage"] >= 0);
        Assert.True(data["dailyAverage"] >= 0);
    }

    [Test]
    public async Task TestGetPredictedUserOnline()
    {
        var response = await _client.GetAsync("/api/predictions/user?date=2023-10-19T00:00:00.000Z&tolerance=0.5&userId=Margarita85");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var responseContent = await response.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseContent);

        Assert.True((double)data["onlineChance"] >= 0);
    }
}