using LastSeenTask;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen(c =>
{
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<ShowUsers>();
builder.Services.AddTransient<IUserDataLoader, UsersLoader>();
builder.Services.AddSingleton<IHistoricalDataStorage, HistoricalDataStorage>();
builder.Services.AddSingleton<IHistoricalDataStorageConcrete, HistoricalDataStorageConcrete>();
builder.Services.AddHttpClient<IUserDataLoader, UsersLoader>();
builder.Services.AddTransient<ShowUsers>();
builder.Services.AddTransient<ILastSeenFormatter, LastSeenFormatter>();
builder.Services.AddSingleton<IReports, Reports>();
builder.Services.AddSingleton(new List<string>());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();