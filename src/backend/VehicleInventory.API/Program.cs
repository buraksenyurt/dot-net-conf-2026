using Microsoft.EntityFrameworkCore;
using VehicleInventory.Application.Commands;
using VehicleInventory.Domain.Interfaces;
using VehicleInventory.Infrastructure.Persistence;
using VehicleInventory.Infrastructure.Repositories;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

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

// CORS - Allow both HTTP and HTTPS ports
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000", 
                "http://localhost:5173",
                "https://localhost:5173")  // If frontend ever uses HTTPS
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// IMPORTANT: CORS must come first in development
app.UseCors("AllowFrontend");

// Skip HTTPS redirection in development to avoid CORS issues
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
