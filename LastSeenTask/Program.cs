﻿using LastSeenTask;

Console.WriteLine($"Which language are you prefer? (You can choose en, ua, es. The default language is English.): ");
var lang = Console.ReadLine();

using (HttpClient client = new HttpClient())
{
    var usersLoader = new UsersLoader(client);
    var users = new LoadUsers(usersLoader, new LastSeenFormatter());
    users.usersShow(lang);
}