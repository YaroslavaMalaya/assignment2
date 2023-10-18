using LastSeenTask;
using Microsoft.AspNetCore.Mvc;
namespace LastSeenIntegrationTests;

public class HistoricalDataStorageConcreteIntegrationTests
{
    private StatsControllerConcrete _statsController;
    private PredictionsControllerConcrete _predictionsController;
    private HistoricalDataStorageConcrete _dataStorage;

    [SetUp]
    public void Setup()
    {
        _dataStorage = new HistoricalDataStorageConcrete();
        _statsController = new StatsControllerConcrete(null, _dataStorage);
        _predictionsController = new PredictionsControllerConcrete(_dataStorage);
    }

    [Test]
    public void ForgetUserIntegration_ReturnsExpectedValue()
    {
        string userId = "User123";
        _dataStorage.AddUserData(DateTimeOffset.UtcNow, new User { Nickname = userId, LastSeenDate = DateTimeOffset.UtcNow});

        var result = _statsController.ForgetUser(userId) as OkObjectResult;
        var returnedUserId = result?.Value.GetType().GetProperty("userId").GetValue(result.Value) as string;
        
        Assert.IsNotNull(result);
        Assert.That(returnedUserId, Is.EqualTo(userId));
        Assert.IsFalse(_dataStorage.UserOnlineHistory.ContainsKey(userId));
    }

    [Test]
    public void QueryUserTotalOnlineTimeIntegration_ReturnsPositiveValue()
    {
        string userId = "User1";
        _dataStorage.AddUserData(DateTimeOffset.UtcNow, new User { Nickname = userId, LastSeenDate = DateTimeOffset.UtcNow});

        var result = _statsController.GetUserTotalOnlineTime(userId) as OkObjectResult;
        var totalTime = (long)result?.Value.GetType().GetProperty("totalTime").GetValue(result.Value);
        
        Assert.IsNotNull(result);
        Assert.IsTrue(totalTime > 0);
    }

    [Test]
    public void GetUserAverageOnlineTimeIntegration_ReturnsPositiveValue()
    {
        string userId = "User2";
        _dataStorage.AddUserData(DateTimeOffset.UtcNow, new User { Nickname = userId, LastSeenDate = DateTimeOffset.UtcNow });

        var result = _statsController.GetUserAverageOnlineTime(userId) as OkObjectResult;
        var dailyAverage = (long)result?.Value.GetType().GetProperty("dailyAverage").GetValue(result.Value);

        Assert.IsNotNull(result);
        Assert.IsTrue(dailyAverage > 0);
    }
}