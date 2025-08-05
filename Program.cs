using CourseService.Models.Database;
using EvacuationPlanning.Services;
using EvacuationPlanning.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using EvacuationPlanning.Repositories;
using EvacuationPlanning.Repositories.Interfaces;
using EvacuationPlanning.Mapper;
using EvacuationPlanning.Infrastructures.Interfaces;
using EvacuationPlanning.Infrastructures;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = builder.Configuration;

// Database config
builder.Services.AddDbContext<DatabaseContext>
    (options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

// Redis config
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = configuration.GetConnectionString("RedisConnection");
    options.InstanceName = "EvacuationPlanning:";
});

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

// Add Services
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IEvacuationZoneService, EvacuationZoneService>();
builder.Services.AddScoped<IEvacuationService, EvacuationService>();

// Add Infrastructure
builder.Services.AddScoped<IRedisService, RedisService>();

// Add Repository
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IEvacuationZoneRepository, EvacuationZoneRepository>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
