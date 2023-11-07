using Microsoft.AspNetCore.Mvc;
namespace LastSeenTask;

public class Report
{
    public string Name { get; set; }
    public List<string> Metrics { get; set; }
    public List<string> UserIds { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public Report(string name, List<string> metrics, List<string> userIds, DateTime startDate, DateTime endDate)
    {
        Name = name;
        Metrics = metrics;
        UserIds = userIds;
        StartDate = startDate;
        EndDate = endDate;
    }
}

public class ReportResult
{
    public string UserId { get; set; }
    public Dictionary<string, double> Metrics { get; set; }

    public ReportResult(string userId, Dictionary<string, double> metrics)
    {
        UserId = userId;
        Metrics = metrics;
    }
}

public class ReportRequest
{
    public List<string> Metrics { get; set; }
    public List<string> Users { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public interface IReports
{
    void CreateReport(string name, List<string> metrics, List<string> userIds, DateTime startDate, DateTime endDate);
    Dictionary<string, ReportResult> GetReport(string name);
    Dictionary<string, double> GetGlobalMetrics(string name, DateTime? fromDate, DateTime? toDate);
    List<Report> GetAllReports();
}

public class Reports : IReports
{
    private readonly Dictionary<string, Report> _reports = new Dictionary<string, Report>();
    private readonly IHistoricalDataStorageConcrete _historicalDataStorageConcrete;

    public Reports(IHistoricalDataStorageConcrete historicalDataStorageConcrete)
    {
        _historicalDataStorageConcrete = historicalDataStorageConcrete;
    }

    public void CreateReport(string name, List<string> metrics, List<string> userIds, DateTime startDate, DateTime endDate)
    {
        if (!_reports.ContainsKey(name))
        {
            var report = new Report(name, metrics, userIds, startDate, endDate);
            _reports[name] = report;
        }
        else
        {
            throw new InvalidOperationException("A report with the same name already exists.");
        }
    }

    public Dictionary<string, ReportResult> GetReport(string name)
    {
        if (_reports.TryGetValue(name, out var report))
        {
            var results = new Dictionary<string, ReportResult>();
            foreach (var userId in report.UserIds)
            {
                var metrics = new Dictionary<string, double>();
                foreach (var metric in report.Metrics)
                {
                    var value = CalculateMetric(metric, userId, report.StartDate, report.EndDate);
                    metrics[metric] = value;
                }

                results[userId] = new ReportResult(userId, metrics);
            }

            return results;
        }
        else
        {
            throw new KeyNotFoundException("The requested report does not exist.");
        }
    }
    
    public Dictionary<string, double> GetGlobalMetrics(string name, DateTime? fromDate = null, DateTime? toDate = null)
    {
        if (_reports.TryGetValue(name, out var report))
        {
            var globalMetrics = new Dictionary<string, double>();
            var userMetrics = new List<Dictionary<string, double>>();

            foreach (var userId in report.UserIds)
            {
                var metrics = new Dictionary<string, double>();
                foreach (var metric in report.Metrics)
                {
                    var value = CalculateMetric(metric, userId, fromDate ?? report.StartDate, toDate ?? report.EndDate);
                    metrics[metric] = value;
                }
                userMetrics.Add(metrics);
            }

            foreach (var metric in report.Metrics)
            {
                var total = userMetrics.Sum(um => um[metric]);
                var average = total / userMetrics.Count;
                globalMetrics[metric] = average;
            }

            return globalMetrics;
        }
        else
        {
            throw new KeyNotFoundException("The requested report does not exist.");
        }
    }
    
    public List<Report> GetAllReports()
    {
        return _reports.Values.ToList();
    }
    
    private double CalculateMetric(string metric, string userId, DateTime startDate, DateTime endDate)
    {
    // check if the user has any online history data
    if (!_historicalDataStorageConcrete.UserOnlineHistory.ContainsKey(userId))
    {
        return 0.0;
    }

    var userOnlineHistory = _historicalDataStorageConcrete.UserOnlineHistory[userId];
    var relevantData = userOnlineHistory
        .Where(kvp => kvp.Key >= startDate && kvp.Key <= endDate)
        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

    switch (metric)
    {
        case "dailyAverage":
            var dailyAverage = relevantData
                .GroupBy(kvp => kvp.Key.Date)
                .Select(group => group.Count(kvp => kvp.Value))
                .DefaultIfEmpty(0)
                .Average();
            return dailyAverage;

        case "weeklyAverage":
            var totalWeeks = (endDate - startDate).Days / 7 + 1;
            var weeklyAverage = relevantData.Count(kvp => kvp.Value) / (double)totalWeeks;
            return weeklyAverage;

        case "total":
            var totalOnlineTime = relevantData.Count(kvp => kvp.Value);
            return totalOnlineTime;

        case "min":
            var minDailyOnlineTime = relevantData
                .GroupBy(kvp => kvp.Key.Date)
                .Select(group => group.Count(kvp => kvp.Value))
                .DefaultIfEmpty(0)
                .Min();
            return minDailyOnlineTime;

        case "max":
            var maxDailyOnlineTime = relevantData
                .GroupBy(kvp => kvp.Key.Date)
                .Select(group => group.Count(kvp => kvp.Value))
                .DefaultIfEmpty(0)
                .Max();
            return maxDailyOnlineTime;
        default:
            throw new InvalidOperationException($"Unknown metric: {metric}");
    }
}

}

// [ApiController]
// [Route("api/report")]
// public class ReportController : ControllerBase
// {
//     private readonly IReports _reports;
//
//     public ReportController(IReports reports)
//     {
//         _reports = reports;
//     }
//
//     [HttpPost("/create/{reportName}")]
//     public IActionResult CreateReport(string reportName, [FromBody] ReportRequest request)
//     {
//         try
//         {
//             _reports.CreateReport(reportName, request.Metrics, request.Users, request.StartDate, request.EndDate);
//             return Ok(new {});
//         }
//         catch (Exception ex)
//         {
//             return BadRequest(ex.Message);
//         }
//     }
//
//     [HttpGet("/get/{reportName}")]
//     public IActionResult GetReport(string reportName, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
//     {
//         try
//         {
//             var userResults = _reports.GetReport(reportName);
//             var globalMetrics = _reports.GetGlobalMetrics(reportName, from, to);
//
//             var result = new
//             {
//                 users = userResults.Select(ur => new 
//                 {
//                     userId = ur.Value.UserId,
//                     metrics = ur.Value.Metrics.Select(m => new { metric = m.Key, value = m.Value }).ToList()
//                 }).ToList(),
//                 globalMetrics
//             };
//
//             return Ok(result);
//         }
//         catch (KeyNotFoundException)
//         {
//             return NotFound();
//         }
//     }
// }
