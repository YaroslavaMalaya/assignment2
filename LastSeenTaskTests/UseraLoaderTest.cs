using System.Net;
using Moq;
using Newtonsoft.Json;
namespace LastSeenTask.Tests;

[TestFixture]
public class UsersLoaderTests
{
    
    [Test]
    public async Task LoadUsers_ReturnsUserData_WhenHttpResponseIsSuccess()
    {
        var expectedUserData = new UserData { data = new[]
            { new User { Nickname = "User1", LastSeenDate = DateTime.Today.AddMinutes(-5) }, 
                new User { Nickname = "User2", LastSeenDate = DateTime.Today.AddMinutes(-60)}}};
        var json = JsonConvert.SerializeObject(expectedUserData);
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json)
        };
        
        var mockHistoricalDataStorage = new Mock<IHistoricalDataStorage>();
        var mockHistoricalDataStorageConcrete = new Mock<IHistoricalDataStorageConcrete>();
        mockHistoricalDataStorage.Setup(m => m.UsersOnlineData).Returns(new Dictionary<DateTime, int>());
        mockHistoricalDataStorageConcrete.Setup(m => m.UserOnlineHistory).Returns(new Dictionary<string, Dictionary<DateTime, bool>>());

        var handler = new MockHttpMessageHandler(response);
        var httpClient = new HttpClient(handler);
        var usersLoader = new UsersLoader(httpClient, mockHistoricalDataStorage.Object, mockHistoricalDataStorageConcrete.Object);
        var result = usersLoader.LoadUsers(0, new List<string>(), DateTime.Now);

        Assert.IsNotNull(result);
        Assert.That(result.data.Length, Is.EqualTo(expectedUserData.data.Length));
    }
}
