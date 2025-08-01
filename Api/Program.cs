using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using test.DataAccess;
using test.Models;
using test.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container and configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

var storage = builder.Configuration["AppSettings:Storage"] ?? "Json";
var connStr = builder.Configuration["AppSettings:ConnectionString"] ?? string.Empty;
var jsonFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "user.json");

if (storage.Equals("Sql", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(connStr))
{
    builder.Services.AddSingleton<IUserDataService>(_ => new SqlUserDataService(connStr));
}
else
{
    builder.Services.AddSingleton<IUserDataService>(_ => new JsonUserDataService(jsonFile));
}

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/login", async (IUserDataService svc, UserModel creds) =>
{
    bool ok = await svc.VerifyUserPasswordAsync(creds.Username, creds.Password);
    return Results.Ok(new { success = ok });
});

app.MapGet("/users", async (IUserDataService svc) => await svc.GetAllUsersAsync());

app.MapGet("/users/{username}", async (IUserDataService svc, string username) =>
{
    var user = await svc.GetUserAsync(username);
    return user == null ? Results.NotFound() : Results.Ok(user);
});

app.MapPost("/users", async (IUserDataService svc, UserModel user) =>
{
    bool added = await svc.AddUserAsync(user);
    return added ? Results.Created($"/users/{user.Username}", user) : Results.Conflict();
});

app.MapPut("/users/{id}", async (IUserDataService svc, int id, UserModel user) =>
{
    user.Id = id;
    bool updated = await svc.UpdateUserAsync(user);
    return updated ? Results.Ok() : Results.NotFound();
});

app.MapDelete("/users/{username}", async (IUserDataService svc, string username) =>
{
    bool deleted = await svc.DeleteUserAsync(username);
    return deleted ? Results.Ok() : Results.NotFound();
});

app.MapPut("/users/{id}/description", async (IUserDataService svc, int id, string description) =>
{
    bool updated = await svc.UpdateUserDescriptionAsync(id, description);
    return updated ? Results.Ok() : Results.NotFound();
});

app.Run();
