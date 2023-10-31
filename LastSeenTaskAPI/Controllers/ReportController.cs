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

    [HttpPost("/create/{reportName}")]
    public IActionResult CreateReport(string reportName, [FromBody] ReportRequest request)
    {
        try
        {
            _reports.CreateReport(reportName, request.Metrics, request.Users, request.StartDate, request.EndDate);
            return Ok(new {message = "The report was created successfully!"});
        }
        catch (Exception ex)
        { 
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("/get/{reportName}")]
    public IActionResult GetReport(string reportName, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        try
        {
            var userResults = _reports.GetReport(reportName);
            var globalMetrics = _reports.GetGlobalMetrics(reportName, from, to);

            var result = new
            {
                users = userResults.Select(ur => new 
                {
                    userId = ur.Value.UserId,
                    metrics = ur.Value.Metrics.Select(m => new { metric = m.Key, value = m.Value }).ToList()
                }).ToList(),
                globalMetrics
            };

            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
