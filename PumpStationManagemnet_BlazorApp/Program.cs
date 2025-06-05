using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PumpStationManagemnet_BlazorApp;
using PumpStationManagemnet_BlazorApp.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
// Cấu hình HttpClient với base address của API
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://localhost:5009/") // Base URL cho tất cả request
});
// Đăng ký UserService
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<PumpStationService>();
builder.Services.AddScoped<PumpService>();
builder.Services.AddScoped<OperatingService>();
builder.Services.AddScoped<MaintenanceService>();
builder.Services.AddScoped<AlertService>();
await builder.Build().RunAsync();
