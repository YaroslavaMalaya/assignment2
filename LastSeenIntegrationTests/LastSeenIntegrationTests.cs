using System.Net;
using System.Text;
using LastSeenTask;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LastSeenIntegrationTests;

public class LastSeenIntegrationTests
{
    private PredictionsController _controller;
    private Mock<IHistoricalDataStorage> _mockStorage;
    private Mock<IHistoricalDataStorageConcrete> _mockStorageConcrete;
    private PredictionsControllerConcrete _controllerConcrete;
    private StatsController _controllerS;
    private Mock<IHistoricalDataStorage> _mockHistoricalDataStorage;
    private UsersLoader usersLoader;
    private MockHttpMessageHandler mockHttpHandler;
    private IHistoricalDataStorage mockHistoricalDataStorage;
    private IHistoricalDataStorageConcrete mockHistoricalDataStorageConcrete;

    [SetUp]
    public void Setup()
    {
        _mockStorageConcrete = new Mock<IHistoricalDataStorageConcrete>();
        _mockStorage = new Mock<IHistoricalDataStorage>();
        _controller = new PredictionsController(_mockStorage.Object);
        _mockHistoricalDataStorage = new Mock<IHistoricalDataStorage>();
        _controllerS = new StatsController(_mockHistoricalDataStorage.Object);
        
        mockHttpHandler = new MockHttpMessageHandler(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(
                "{\"data\": [{\"LastSeenDate\": \"2023-10-10T10:10:10+00:00\",\"Nickname\": \"testuser\"}]}", 
                Encoding.UTF8, 
                "application/json")
        });
        var httpClient = new HttpClient(mockHttpHandler);
            
        usersLoader = new UsersLoader(httpClient, mockHistoricalDataStorage, mockHistoricalDataStorageConcrete);
    }

    [Test]
    public void GetPredictedUsersOnlineConcrete_ReturnsExpectedValue()
    {
        var mockHistoricalDataStorageConcrete = new Mock<IHistoricalDataStorageConcrete>();
        mockHistoricalDataStorageConcrete.Setup(m => m.CalculateOnlineChance(It.IsAny<DateTime>(), 
                It.IsAny<double>(), It.IsAny<string>()))
            .Returns(0.6);

        var controller = new PredictionsControllerConcrete(mockHistoricalDataStorageConcrete.Object);
        var date = DateTime.UtcNow;
        var tolerance = 0.5;
        var userId = "sampleUserId";

        var result = controller.GetPredictedUserOnline(date, tolerance, userId) as OkObjectResult;
        var resultValue = result?.Value as Dictionary<string, object>;

        Assert.IsNotNull(result);
        Assert.IsTrue((bool)resultValue["willBeOnline"]);
        Assert.That((double)resultValue["onlineChance"], Is.EqualTo(0.6));
    }
    
    [Test]
    public void GetPredictedUserOnline_ReturnsExpectedValue()
    {
        var testDate = new DateTime(2023, 10, 30);
        _mockStorage.Setup(m => m.GetAverageUsersForDayOfWeek(DayOfWeek.Monday)).Returns(10);

        var result = _controller.GetPredictedUsersOnline(testDate);
        var okResult = result as OkObjectResult;
        
        Assert.IsNotNull(okResult);
        var returnData = JObject.FromObject(okResult?.Value);
        Assert.That(returnData["onlineUsers"].Value<int>(), Is.EqualTo(10));
    }
    
    [Test]
    public void GetUsersOnline_ReturnsExpectedValue()
    {
        var expectedDate = new DateTime(2023, 10, 10);
        var expectedCount = 100;
        var data = new Dictionary<DateTime, int>
        {
            { expectedDate, expectedCount }
        };
        _mockHistoricalDataStorage.SetupGet(storage => storage.UsersOnlineData).Returns(data);

        var result = _controllerS.GetUsersOnline(expectedDate);
        var okResult = result as OkObjectResult;
        var returnedValue = JsonConvert.DeserializeObject<Dictionary<string, int>>(JsonConvert.SerializeObject(okResult?.Value));

        Assert.NotNull(returnedValue);
        Assert.True(returnedValue.ContainsKey("usersOnline"));
        Assert.That(returnedValue["usersOnline"], Is.EqualTo(expectedCount));
    }

    [Test]
    public void LoadUsers_InvalidResponse_ReturnsEmptyUserData()
    {
        var offset = 0;
        mockHttpHandler = new MockHttpMessageHandler(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.BadRequest
        });
        var httpClient = new HttpClient(mockHttpHandler);
        usersLoader = new UsersLoader(httpClient, mockHistoricalDataStorage, mockHistoricalDataStorageConcrete);

        var userData = usersLoader.LoadUsers(offset);

        Assert.IsNotNull(userData);
        Assert.IsNull(userData.data);
    }
}
