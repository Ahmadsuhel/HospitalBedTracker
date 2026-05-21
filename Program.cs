using HospitalBedTracker.Application.Interfaces;
using HospitalBedTracker.Infrastructure.Persistence;
using HospitalBedTracker.Infrastructure.Realtime;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// PostgreSQL + EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Redis cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration =
        builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "HospitalBedTracker:";
});

// MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(
        typeof(HospitalBedTracker.Application.Commands
            .UpdateBedCount.UpdateBedCountCommand).Assembly));

// Repository
builder.Services.AddScoped<IBedRepository, BedRepository>();

// SignalR
builder.Services.AddSignalR();

// CORS — Angular ke liye
builder.Services.AddCors(options =>
{
    options.AddPolicy("Angular", policy =>
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

// PostgreSQL background listener
builder.Services.AddHostedService<PostgresListenerService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Hospital Bed API");
    });
}

app.UseCors("Angular");
app.UseAuthorization();
app.MapControllers();

// SignalR Hub
app.MapHub<BedAvailabilityHub>("/hubs/beds");

app.Run();