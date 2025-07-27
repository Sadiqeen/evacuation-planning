using CourseService.Models.Database;
using EvacuationPlanning.Services;
using EvacuationPlanning.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using EvacuationPlanning.Repositories;
using EvacuationPlanning.Repositories.Interfaces;
using EvacuationPlanning.Mapper;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = builder.Configuration;

// Database config
builder.Services.AddDbContext<DatabaseContext>
    (options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

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

// Add Repository
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IEvacuationZoneRepository, EvacuationZoneRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
