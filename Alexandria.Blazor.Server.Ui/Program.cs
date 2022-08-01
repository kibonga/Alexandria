using Alexandria.Blazor.Server.Ui.Providers;
using Alexandria.Blazor.Server.Ui.Services;
using Alexandria.Blazor.Server.Ui.Services.Auth;
using Alexandria.Blazor.Server.Ui.Services.Base;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

#region Register Services
//

#region Register HttpClient
builder.Services.AddHttpClient<IClient, Client>(c => c.BaseAddress = new Uri("https://localhost:7274"));
#endregion

#region Register Blazored.LocalStorage
builder.Services.AddBlazoredLocalStorage();
#endregion

#region Register Services
builder.Services.RegisterServicesExtension();
#endregion

#region Register Providers
builder.Services.RegisterProvidersExtension();
#endregion

//
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
