using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
using TodoApi.Services;
using TodoApi.Hubs; // 1. Agrega el using para los Hubs

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<TodoContext>(opt =>
        opt.UseSqlServer(builder.Configuration.GetConnectionString("TodoContext"))
    )
    .AddEndpointsApiExplorer()
    .AddControllers();

builder.Services.AddScoped<TodoCompletionService>();

builder.Services.AddSingleton<IBackgroundJobService, BackgroundJobService>();

builder.Services.AddSignalR();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

//CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.MapHub<TodoHub>("/todohub");

app.Run();
