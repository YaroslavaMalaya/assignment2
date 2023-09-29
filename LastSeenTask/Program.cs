using LastSeenTask;

using (HttpClient client = new HttpClient())
{
    var usersLoader = new UsersLoader(client);
    var offset = 0;

    while (true)
    {
        var userData = usersLoader.LoadUsers(offset);

        if (userData.data == null || userData.data.Length == 0)
        {
            break;
        }

        var formatter = new LastSeenFormatter();
        foreach (User user in userData.data)
        {
            var formattedLastSeen = formatter.Format(DateTimeOffset.Now, user.LastSeenDate.GetValueOrDefault());
            Console.WriteLine($"{user.Nickname} was online {formattedLastSeen}.");
        }

        offset += 5;
    }
}