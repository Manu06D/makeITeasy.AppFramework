using Serilog;
using makeITeasy.AppFramework.Web.Helpers;
using FluentValidation;
using ContosoUniversity.WebApplication.BackgroundServices;
using System.Threading.Channels;
using makeITeasy.AppFramework.Models;
using ContosoUniversity.WebApplication.Modules.Startup;
using ContosoUniversity.WebApplication.Models.ApplicationModels;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) => config.WriteTo.Console().WriteTo.Debug());

builder.Services.AddControllersWithViews();
builder.Services.AddOptions();

builder.Services.AddOptions<ApplicationConfiguration>()
    .BindConfiguration("ApplicationConfiguration")
    .ValidateDataAnnotations()
    .Validate(conf => !string.IsNullOrWhiteSpace(conf.Name), "Invalid Configuration")
    .ValidateOnStart()
    ;

builder.Services.RegisterDatatablesService();
builder.Services.AddValidatorsFromAssembly(typeof(ContosoUniversity.Models.Instructor).Assembly);

builder.ConfigureDatabase();
builder.ConfigureAutofac();

//Channels sample
builder.Services.AddSingleton(Channel.CreateUnbounded<IBaseEntity>(new UnboundedChannelOptions() { SingleReader = true }));
builder.Services.AddSingleton(service => service.GetRequiredService<Channel<IBaseEntity>>().Reader);
builder.Services.AddSingleton(service => service.GetRequiredService<Channel<IBaseEntity>>().Writer);
builder.Services.AddHostedService<IBaseEntityReaderService>();

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
