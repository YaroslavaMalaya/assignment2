using LastSeenTask;
using Microsoft.AspNetCore.Mvc;

Console.WriteLine("Which language are you prefer? (You can choose en, ua, es. The default language is English.): ");
var lang = Console.ReadLine();

var historicalDataStorage = new HistoricalDataStorage();
var historicalDataStorageConcrete = new HistoricalDataStorageConcrete();
var reports = new Reports(historicalDataStorageConcrete);
var reportController = new ReportController(reports);

using (HttpClient client = new HttpClient())
{
    var usersLoader = new UsersLoader(client, historicalDataStorage, historicalDataStorageConcrete);
    var users = new ShowUsers(usersLoader, new LastSeenFormatter());
    users.UsersShow(lang);
    
    var continueR = true;
    while (continueR)
    {
        Console.WriteLine("\nChoose an option:");
        Console.WriteLine("1 - Display overall historical data.");
        Console.WriteLine("2 - Display historical data for each user.");
        Console.WriteLine("3 - Predict number of users online for a specified date.");
        Console.WriteLine("4 - Calculate online chance for a specific user.");
        Console.WriteLine("5 - Display total online time for a user.");
        Console.WriteLine("6 - Display average online time for a user.");
        Console.WriteLine("7 - Display average online time for a user.");
        Console.WriteLine("8 - Display User Report.");
        Console.WriteLine("9 - Exit.");

        Console.WriteLine("Option: ");
        var choice = Console.ReadLine();
        switch (choice)
        {
            case "1":
                historicalDataStorage.DisplayHistoricalData();
                break;
            case "2":
                historicalDataStorageConcrete.DisplayHistoricalDataConcrete();
                break;
            case "3":
                PredictOnlineUsers(historicalDataStorage);
                break;
            case "4":
                CalculateOnlineChanceForUser(historicalDataStorageConcrete);
                break;
            case "5":
                DisplayTotalOnlineTimeForUser(historicalDataStorageConcrete);
                break;
            case "6":
                DisplayAverageOnlineTimeForUser(historicalDataStorageConcrete);
                break;
            case "7":
                DisplayForgetUser(historicalDataStorageConcrete);
                break;
            case "8":
                CreateAndDisplayReport(reportController);
                break;
            case "9":
                continueR = false;
                break;
            default:
                Console.WriteLine("Invalid choice. Please select a valid option.");
                break;
        }
    }
}
return;

void PredictOnlineUsers(IHistoricalDataStorage historicalDataStorage)
{
    // it can predict right now only with the same day of the week as today 
    Console.WriteLine("Enter a date for which you want to predict the number of online users (format: YYYY-MM-DD): ");
    var input = Console.ReadLine();
    if (DateTime.TryParse(input, out DateTime date))
    {
        var prediction = historicalDataStorage.GetAverageUsersForDayOfWeek(date.DayOfWeek);
        Console.WriteLine($"Predicted number of online users for {date:yyyy-MM-dd} ({date.DayOfWeek}): {prediction}");
    }
    else
    {
        Console.WriteLine("Invalid date format. Please try again.");
    }
}

void CalculateOnlineChanceForUser(IHistoricalDataStorageConcrete historicalDataStorageCon)
{
    Console.WriteLine("Enter user ID: ");
    var userId = Console.ReadLine();

    Console.WriteLine("Enter a date for which you want to predict the online chance (format: YYYY-MM-DD): ");
    var inputDate = Console.ReadLine();
    if (!DateTime.TryParse(inputDate, out DateTime date))
    {
        Console.WriteLine("Invalid date format. Please try again.");
        return;
    }

    Console.WriteLine("Enter tolerance (e.g., 0,1 for 10%): ");
    if (!double.TryParse(Console.ReadLine(), out var tolerance))
    {
        Console.WriteLine("Invalid tolerance value. Please try again.");
        return;
    }

    var chance = historicalDataStorageCon.CalculateOnlineChance(date, tolerance, userId);
    Console.WriteLine($"The chance for user {userId} to be online on {date:yyyy-MM-dd} is {chance * 100}%."); 
}

void DisplayTotalOnlineTimeForUser(IHistoricalDataStorageConcrete historicalDataStorageCon)
{
    Console.WriteLine("Enter user nickname: ");
    var userNickname = Console.ReadLine();
    var totalTime = historicalDataStorageCon.GetTotalOnlineTime(userNickname);
    Console.WriteLine($"User {userNickname} has been online for a total of {totalTime} seconds.");
}

void DisplayAverageOnlineTimeForUser(IHistoricalDataStorageConcrete historicalDataStorageCon)
{
    Console.WriteLine("Enter user nickname: ");
    var userNickname = Console.ReadLine();
    var averageTime = historicalDataStorageCon.CalculateAverages(userNickname);
    Console.WriteLine($"User {userNickname} has a weekly average online time - {averageTime.weeklyAverage} and" +
                      $" a daily average online time - {averageTime.dailyAverage}.");
}

void DisplayForgetUser(IHistoricalDataStorageConcrete historicalDataStorageCon)
{
    Console.WriteLine("Enter user nickname: ");
    var userNickname = Console.ReadLine();
    historicalDataStorageCon.ForgetUser(userNickname);
    Console.WriteLine($"User {userNickname} has been forgotten.");
}

void CreateAndDisplayReport(ReportController reportController)
{
    Console.WriteLine("Enter report name: ");
    var reportName = Console.ReadLine();
    Console.WriteLine("Enter metrics (comma-separated, e.g. total,dailyAverage): ");
    var metricsInput = Console.ReadLine();
    var metrics = metricsInput?.Split(',').ToList() ?? new List<string>();
    Console.WriteLine("Enter user IDs (comma-separated): ");
    var userIdsInput = Console.ReadLine();
    var userIds = userIdsInput?.Split(',').ToList() ?? new List<string>();
    Console.WriteLine("Enter start date (format: YYYY-MM-DD): ");
    var startDateInput = Console.ReadLine();
    if (!DateTime.TryParse(startDateInput, out var startDate))
    {
        Console.WriteLine("Invalid start date format. Please try again.");
        return;
    }
    Console.WriteLine("Enter end date (format: YYYY-MM-DD): ");
    var endDateInput = Console.ReadLine();
    if (!DateTime.TryParse(endDateInput, out var endDate))
    {
        Console.WriteLine("Invalid end date format. Please try again.");
        return;
    }

    var reportRequest = new ReportRequest
    {
        Metrics = metrics,
        Users = userIds,
        StartDate = startDate,
        EndDate = endDate
    };
    var createReportResult = reportController.CreateReport(reportName, reportRequest);
    if (createReportResult is OkResult)
    {
        Console.WriteLine("Report created successfully.");
    }
    else if (createReportResult is BadRequestObjectResult badRequestResult)
    {
        Console.WriteLine($"Failed to create report: {badRequestResult.Value}");
        return;
    }

    var getReportResult = reportController.GetReport(reportName);
    if (getReportResult is OkObjectResult okObjectResult)
    {
        if (okObjectResult.Value is Dictionary<string, ReportResult> reportResults)
        {
            foreach (var (userId, reportResult) in reportResults)
            {
                Console.WriteLine($"Report for User ID: {userId}");
                foreach (var (metric, value) in reportResult.Metrics)
                {
                    Console.WriteLine($"{metric}: {value}");
                }
            }
        }
    }
    else if (getReportResult is NotFoundResult)
    {
        Console.WriteLine("Report not found.");
    }
}
