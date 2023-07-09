using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQWeb.WaterMark.BackgroundServices;
using RabbitMQWeb.WaterMark.Models;
using RabbitMQWeb.WaterMark.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSingleton(sp => new ConnectionFactory() { Uri = new Uri(builder.Configuration.GetConnectionString("RabbitMQ")), DispatchConsumersAsync = true }) ;
builder.Services.AddSingleton<RabbitMqClientService>();
builder.Services.AddSingleton<RabbitMQPublisher>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseInMemoryDatabase(databaseName:"productDb");
});

builder.Services.AddHostedService<ImageWatermarkProcessBackgroundService>();
builder.Services.AddHostedService<ImageResizeProcessBackgroundService >();

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
