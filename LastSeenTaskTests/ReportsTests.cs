using LastSeenTask;
using LastSeenTaskAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
namespace LastSeenTaskTests;

public class ReportsTests
{
    private Mock<IReports> _mockReports;
    private ReportController _controller;

    [SetUp]
    public void Setup()
    {
        _mockReports = new Mock<IReports>();
        _controller = new ReportController(_mockReports.Object);
    }

    [Test]
    public void When_CreatingReport_Expect_SuccessfulResponse()
    {
        string reportName = "TestReport";
        var reportRequest = new ReportRequest
        {
            Metrics = new List<string> { "dailyAverage" },
            Users = new List<string> { "User1" },
            StartDate = DateTime.Now.AddDays(-7),
            EndDate = DateTime.Now
        };

        _mockReports.Setup(r => r.CreateReport(reportName, reportRequest.Metrics, reportRequest.Users, reportRequest.StartDate, reportRequest.EndDate));

        var actionResult = _controller.CreateReport(reportName, reportRequest);
        var okResult = actionResult as OkObjectResult;

        Assert.IsNotNull(okResult);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
    }

    [Test]
    public void When_GettingReport_Expect_SuccessfulResponse()
    {
        string reportName = "TestReport";

        var expectedReport = new Dictionary<string, ReportResult>
        {
            {
                "User1", new ReportResult("User1", new Dictionary<string, double>
                {
                    { "dailyAverage", 5.0 }
                })
            }
        };

        _mockReports.Setup(r => r.GetReport(reportName)).Returns(expectedReport);

        var actionResult = _controller.GetReport(reportName, null, null);
        var okResult = actionResult as OkObjectResult;

        Assert.IsNotNull(okResult);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
    }
    
    [Test]
    public void When_GettingAllReports_Expect_SuccessfulResponse()
    {
        var expectedReports = new List<Report>
        {
            new Report("Report1", new List<string> { "dailyAverage" }, new List<string> { "User1" }, DateTime.Now.AddDays(-7), DateTime.Now),
            new Report("Report2", new List<string> { "total" }, new List<string> { "User2" }, DateTime.Now.AddMonths(-1), DateTime.Now)
        };

        _mockReports.Setup(r => r.GetAllReports()).Returns(expectedReports);

        var actionResult = _controller.GetReports();
        var okResult = actionResult as OkObjectResult;

        Assert.IsNotNull(okResult);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
    }
}
