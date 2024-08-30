using Fishzor.Client.Pages;
using Fishzor.Components;
using Fishzor.Services;
using Fishzor.Client.State;
using System.Net.Http;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Fishzor.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();
builder.Services.AddControllers();
builder.Services.AddSingleton<FishService>();
builder.Services.AddScoped<FishTankState>();
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Fishzor.Client._Imports).Assembly);
app.MapHub<FishHub>("/fishhub");

app.Run();
