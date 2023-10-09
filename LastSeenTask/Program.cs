using LastSeenTask;

Console.WriteLine($"Which language are you prefer? (You can choose en, ua, es. The default language is English.): ");
var lang = Console.ReadLine();

var historicalDataStorage = new HistoricalDataStorage();

using (HttpClient client = new HttpClient())
{
    var usersLoader = new UsersLoader(client, historicalDataStorage);
    var users = new ShowUsers(usersLoader, new LastSeenFormatter());
    users.UsersShow(lang);
}

historicalDataStorage.DisplayHistoricalData();