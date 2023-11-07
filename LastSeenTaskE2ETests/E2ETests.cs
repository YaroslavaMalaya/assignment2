using LastSeenTaskAPI.Controllers;
using LastSeenTask;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using ReportController = LastSeenTaskAPI.Controllers.ReportController;

namespace LastSeenTaskAPI.Tests;

[TestFixture]
public class E2ETests
{
    private PredictionsController _controllerPrediction;
    private Mock<IHistoricalDataStorage> _mockHistoricalDataStorage;
    private Mock<IHistoricalDataStorageConcrete> _mockHistoricalDataStorageConcrete;
    private PredictionsController _controllerConcrete;
    private StatsController _controllerS;
    private Mock<IUserDataLoader> _mockUserDataLoader;
    private MockHttpMessageHandler mockHttpHandler;
    private ShowUsers _mockShowUsers;
    private Mock<ILastSeenFormatter> _mockLastSeenFormatter;
    private List<string> _mockForgottenUsers;
    private UsersLoader usersLoader;
    private ChangeLanguage _changeLanguage;
    private Mock<IReports> _mockReports;
    private ReportController _controllerReport;

    [SetUp]
    public void Setup()
    {
        _changeLanguage = new ChangeLanguage();
        _mockUserDataLoader = new Mock<IUserDataLoader>();
        _mockLastSeenFormatter = new Mock<ILastSeenFormatter>();
        _mockForgottenUsers = new List<string>();
        _mockHistoricalDataStorageConcrete = new Mock<IHistoricalDataStorageConcrete>();
        _mockHistoricalDataStorage = new Mock<IHistoricalDataStorage>();
        _controllerPrediction = new PredictionsController(_mockHistoricalDataStorage.Object, _mockHistoricalDataStorageConcrete.Object);
        _controllerS = new StatsController(_mockHistoricalDataStorage.Object, _mockShowUsers, _mockHistoricalDataStorageConcrete.Object);
        _mockShowUsers = new ShowUsers(_mockUserDataLoader.Object, _mockLastSeenFormatter.Object, _mockForgottenUsers);
        _mockReports = new Mock<IReports>();
        _controllerReport = new ReportController(_mockReports.Object);
    }
    
    [Test]
    public void EndToEndTest()
    {
        // Setting Language and Checking Last Seen Formatter
        var languageEN = "fr";
        _changeLanguage.ChangeLang(languageEN);
        Assert.That(_changeLanguage.result1, Is.EqualTo("online"));
        var languageUA = "ua";
        _changeLanguage.ChangeLang(languageUA);
        Assert.That(_changeLanguage.result1, Is.EqualTo("онлайн"));
        var languageES = "es";
        _changeLanguage.ChangeLang(languageES);
        Assert.That(_changeLanguage.result1, Is.EqualTo("en línea"));

        // Checking Users' Nickname and Last Seen Date
        var userData1 = new UserData { data = new[] 
            { new User { Nickname = "User1", LastSeenDate = DateTime.Today.AddMinutes(-5)}}};
        _mockUserDataLoader.Setup(loader => loader.LoadUsers(It.IsAny<int>(), _mockForgottenUsers, DateTime.Now)).Returns(userData1);
        DateTimeOffset check1 = DateTime.Today.AddMinutes(-5);
        Assert.That(userData1.data[0].Nickname, Is.EqualTo("User1"));
        Assert.That(userData1.data[0].LastSeenDate, Is.EqualTo(check1));

        // Querying Specific Date and Expecting Correct Users Online Count
        var expectedDate = new DateTime(2023, 10, 10);
        var expectedCount = 100;
        var data = new Dictionary<DateTime, int> {{ expectedDate, expectedCount }};
        _mockHistoricalDataStorage.SetupGet(storage => storage.UsersOnlineData).Returns(data);
        var result = _controllerS.GetUsersOnline(expectedDate);
        var okResult = result as OkObjectResult;
        var returnedValue = JsonConvert.DeserializeObject<Dictionary<string, int>>(JsonConvert.SerializeObject(okResult?.Value));
        Assert.That(returnedValue["usersOnline"], Is.EqualTo(expectedCount));

        // Predicting User Online Status with High Chance
        var userId = "testUser";
        var date = new DateTime(2023, 10, 11, 14, 30, 0);      
        var tolerance = 0.1;
        _mockHistoricalDataStorageConcrete.Setup(x => x.CalculateOnlineChance(date, tolerance, userId)).Returns(0.9);
        var predictedResult = _controllerPrediction.GetPredictedUserOnline(date, tolerance, userId) as OkObjectResult;
        var responseData = predictedResult.Value as Dictionary<string, object>;
        Assert.That(responseData["onlineChance"], Is.EqualTo(0.9));
        Assert.That(responseData["willBeOnline"], Is.EqualTo(true));

        // Querying User's Average Online Time
        var expectedWeeklyAverage = 2500L;
        var expectedDailyAverage = 500L;
        _mockHistoricalDataStorageConcrete.Setup(storage => storage.CalculateAverages(userId)).Returns((expectedWeeklyAverage, expectedDailyAverage));
        var averageTimeResult = _controllerS.GetUserAverageOnlineTime(userId) as OkObjectResult;
        var weeklyAverageResult = (long)(averageTimeResult.Value.GetType().GetProperty("weeklyAverage").GetValue(averageTimeResult.Value));
        var dailyAverageResult = (long)(averageTimeResult.Value.GetType().GetProperty("dailyAverage").GetValue(averageTimeResult.Value));
        Assert.That(weeklyAverageResult, Is.EqualTo(expectedWeeklyAverage));
        Assert.That(dailyAverageResult, Is.EqualTo(expectedDailyAverage));

        // Querying User's Total Online Time
        var expectedTotalTime = 10000L;
        _mockHistoricalDataStorageConcrete.Setup(storage => storage.GetTotalOnlineTime(userId)).Returns(expectedTotalTime);
        var totalTimeResult = _controllerS.GetUserTotalOnlineTime(userId) as OkObjectResult;
        var totalOnlineTime = (long)(totalTimeResult.Value.GetType().GetProperty("totalTime").GetValue(totalTimeResult.Value));
        Assert.That(totalOnlineTime, Is.EqualTo(expectedTotalTime));
        
        // Getting a Report
        var reportName = "TestReport";
        var expectedReport = new Dictionary<string, ReportResult>{
            {        
                "User1", new ReportResult("User1", new Dictionary<string, double>{{ "dailyAverage", 5.0 }})}
        };
        _mockReports.Setup(r => r.GetReport(reportName)).Returns(expectedReport);
        var reportActionResult = _controllerReport.GetReport(reportName, null, null) as OkObjectResult;
        var reportResult = reportActionResult.Value as Dictionary<string, ReportResult>;
        Assert.That(reportResult["User1"].Metrics["dailyAverage"], Is.EqualTo(expectedReport["User1"].Metrics["dailyAverage"]));
    }
}