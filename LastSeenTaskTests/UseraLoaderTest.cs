using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
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

        var handler = new MockHttpMessageHandler(response);
        var httpClient = new HttpClient(handler);
        var usersLoader = new UsersLoader(httpClient);
        var result = usersLoader.LoadUsers(0);

        Assert.IsNotNull(result);
        Assert.That(result.data.Length, Is.EqualTo(expectedUserData.data.Length));
    }
}
