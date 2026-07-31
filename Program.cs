using Microsoft.EntityFrameworkCore;
using Npgsql.NameTranslation;
using P3Examen_AirportApp.Data;
using P3Examen_AirportApp.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AirportContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), npg =>
    {
        npg.MapEnum<EmployeeDepartment>("employee_department", "airportdb", new NpgsqlNullNameTranslator());
        npg.MapEnum<WeatherCondition>("weatherdata_weather", "airportdb", new WeatherConditionTranslator());
    }));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
