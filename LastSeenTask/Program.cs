using LastSeenTask;

Console.WriteLine("Which language are you prefer? (You can choose en, ua, es. The default language is English.): ");
var lang = Console.ReadLine();

var historicalDataStorage = new HistoricalDataStorage();
var historicalDataStorageConcrete = new HistoricalDataStorageConcrete();

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
        Console.WriteLine("5 - Exit.");

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
            case "3" :
                PredictOnlineUsers(historicalDataStorage);
                break;
            case "4" :
                CalculateOnlineChanceForUser(historicalDataStorageConcrete);
                break;
            case "5":
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