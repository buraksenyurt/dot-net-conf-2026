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

// CORS - Allow frontend origins
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000", 
                "http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// CORS must be first
app.UseCors();

app.UseAuthorization();
app.MapControllers();

app.Run();
