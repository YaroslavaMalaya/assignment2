using LastSeenTask;
using Microsoft.AspNetCore.Mvc;
namespace LastSeenTaskAPI.Controllers;

[ApiController]
[Route("api/report")]
public class ReportController : ControllerBase
{
    private readonly IReports _reports;

    public ReportController(IReports reports)
    {
        _reports = reports;
    }

    [HttpPost("{reportName}")]
    public IActionResult CreateReport(string reportName, [FromBody] ReportRequest request)
    {
        try
        {
            _reports.CreateReport(reportName, request.Metrics, request.Users, request.StartDate, request.EndDate);
            return Ok(new {});
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{reportName}")]
    public IActionResult GetReport(string reportName)
    {
        try
        {
            var report = _reports.GetReport(reportName);
            return Ok(report);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
