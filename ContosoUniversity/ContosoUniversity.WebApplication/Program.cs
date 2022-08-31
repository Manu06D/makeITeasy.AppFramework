using Serilog;
using ContosoUniversity.WebApplication.WebAppElements.Startup;
using makeITeasy.AppFramework.Web.Helpers;
using Autofac.Core;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) => config.WriteTo.Console().WriteTo.Debug());

builder.Services.AddControllersWithViews();
builder.Services.AddOptions();
builder.Services.RegisterDatatablesService();
builder.Services.AddValidatorsFromAssembly(typeof(ContosoUniversity.Models.Instructor).Assembly);

builder.ConfigureDatabase();
builder.ConfigureAutofac();

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
