using Microsoft.AspNetCore.Mvc;
namespace LastSeenTask;

public interface IHistoricalDataStorage
{
    Dictionary<DateTime, int> UsersOnlineData { get; set; }
    int GetAverageUsersForDayOfWeek(DayOfWeek dayOfWeek);
}

public class HistoricalDataStorage : IHistoricalDataStorage
{
    public Dictionary<DateTime, int> UsersOnlineData { get; set; } = new Dictionary<DateTime, int>();

    public void DisplayHistoricalData()
    {
        Console.WriteLine("\nHistorical Data of Users Online:");
        foreach (var element in UsersOnlineData)
        {
            Console.WriteLine($"Date: {element.Key}, Users Online: {element.Value}");
        }
    }
    
    public int GetAverageUsersForDayOfWeek(DayOfWeek dayOfWeek)
    {
        var filteredData = UsersOnlineData.Where(element => element.Key.DayOfWeek == dayOfWeek);
        var keyValuePairs = filteredData.ToList();
        if (!keyValuePairs.Any()) return 0;
        return (int)keyValuePairs.Average(element => element.Value);
    }
}
 