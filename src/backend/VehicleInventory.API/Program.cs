using Microsoft.EntityFrameworkCore;
using VehicleInventory.Application.Abstractions;
using VehicleInventory.Application.Commands;
using VehicleInventory.Domain.Interfaces;
using VehicleInventory.Infrastructure.Persistence;
using VehicleInventory.Infrastructure.Repositories;
using VehicleInventory.Infrastructure.Security;
using VehicleInventory.API.Extensions;
using FluentValidation;
using Serilog;

// Bootstrap logger
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try 
{
    Log.Information("Starting web application");

    var builder = WebApplication.CreateBuilder(args);

    // Add Serilog with colorful console output
    builder.Host.UseSerilog((context, configuration) =>
        configuration.ReadFrom.Configuration(context.Configuration)
                     .Enrich.FromLogContext()
                     .WriteTo.Console(theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Code));


    // Add services to the container
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // DbContext
    builder.Services.AddDbContext<VehicleInventoryDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

    // MediatR
    builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AddVehicleCommand).Assembly));

    // FluentValidation
    builder.Services.AddValidatorsFromAssemblyContaining<AddVehicleCommand>();

    // Repositories
    builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
    builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
    builder.Services.AddScoped<IVehicleOptionRepository, VehicleOptionRepository>();
    builder.Services.AddScoped<IServiceAdvisorRepository, ServiceAdvisorRepository>();

    // Security
    builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

    // CORS - Allow ANY origin for debugging
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.AllowAnyOrigin()  // Nuclear option for debugging
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    });

    var app = builder.Build();
    
    // Enable Serilog Request Logging
    app.UseSerilogRequestLogging();

    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseRouting(); // Explicitly add routing
    app.UseCors();    // CORS must be after Routing for some scenarios, but before Auth

    app.UseAuthorization();
    app.MapControllers();

    // Seed demo service advisors on first run
    await app.SeedServiceAdvisorsAsync();

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}
