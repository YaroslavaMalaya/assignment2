using LastSeenTask;
namespace LastSeenTaskTests;

public class HistoricalDataStorageConcreteTests
{
    private IHistoricalDataStorageConcrete _storageConcrete;

    [SetUp]
    public void SetUp()
    {
        _storageConcrete = new HistoricalDataStorageConcrete();
    }

    [Test]
    public void When_AddingUserData_Expect_UserOnlineHistoryIsUpdated()
    {
        var user = new User { Nickname = "User1", LastSeenDate = DateTime.Today.AddMinutes(-5) };
        var time = DateTimeOffset.Now;

        _storageConcrete.AddUserData(time, user);

        Assert.True(_storageConcrete.UserOnlineHistory.ContainsKey(user.Nickname));
        Assert.True(_storageConcrete.UserOnlineHistory[user.Nickname].ContainsKey(time.DateTime));
        Assert.True(_storageConcrete.UserOnlineHistory[user.Nickname][time.DateTime]);
    }

    [Test]
    public void When_QueryingUserHistoricalData_Expect_CorrectNearestOnlineTime()
    {
        var user = new User { Nickname = "User1", LastSeenDate = DateTime.Today.AddMinutes(-5) };
        var time = DateTimeOffset.Now;
        var queriedDate = time.DateTime.AddMinutes(-3);

        _storageConcrete.AddUserData(time, user);
        
        var result = _storageConcrete.GetUserHistoricalData(queriedDate, user.Nickname);

        Assert.NotNull(result);
        Assert.Null(result.WasUserOnline);
        Assert.That(result.NearestOnlineTime?.Minute, Is.EqualTo(time.Minute));
    }
}