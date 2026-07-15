using EnterpriseSalesPredictor.Application;
using EnterpriseSalesPredictor.Web.Configuration;
using EnterpriseSalesPredictor.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddApplication();
builder.Services.Configure<ApiOptions>(builder.Configuration.GetSection(ApiOptions.SectionName));
builder.Services.Configure<JwtClientOptions>(builder.Configuration.GetSection(JwtClientOptions.SectionName));

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Home/Forbidden";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient<IWebAuthApiClient, WebAuthApiClient>((serviceProvider, httpClient) =>
{
    var apiOptions = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptionsMonitor<ApiOptions>>().CurrentValue;
    httpClient.BaseAddress = new Uri(apiOptions.BaseUrl);
});

builder.Services.AddHttpClient<AccessManagementApiClient>((serviceProvider, httpClient) =>
{
    var apiOptions = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptionsMonitor<ApiOptions>>().CurrentValue;
    httpClient.BaseAddress = new Uri(apiOptions.BaseUrl);
});

builder.Services.AddHttpClient<UploadsApiClient>((serviceProvider, httpClient) =>
{
    var apiOptions = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptionsMonitor<ApiOptions>>().CurrentValue;
    httpClient.BaseAddress = new Uri(apiOptions.BaseUrl);
});

builder.Services.AddHttpClient<AuditApiClient>((serviceProvider, httpClient) =>
{
    var apiOptions = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptionsMonitor<ApiOptions>>().CurrentValue;
    httpClient.BaseAddress = new Uri(apiOptions.BaseUrl);
});

builder.Services.AddHttpClient<SalesApiClient>((serviceProvider, httpClient) =>
{
    var apiOptions = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptionsMonitor<ApiOptions>>().CurrentValue;
    httpClient.BaseAddress = new Uri(apiOptions.BaseUrl);
});

builder.Services.AddHttpClient<DashboardApiClient>((serviceProvider, httpClient) =>
{
    var apiOptions = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptionsMonitor<ApiOptions>>().CurrentValue;
    httpClient.BaseAddress = new Uri(apiOptions.BaseUrl);
});

builder.Services.AddHttpClient<ReportsApiClient>((serviceProvider, httpClient) =>
{
    var apiOptions = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptionsMonitor<ApiOptions>>().CurrentValue;
    httpClient.BaseAddress = new Uri(apiOptions.BaseUrl);
});

builder.Services.AddHttpClient<ExportsApiClient>((serviceProvider, httpClient) =>
{
    var apiOptions = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptionsMonitor<ApiOptions>>().CurrentValue;
    httpClient.BaseAddress = new Uri(apiOptions.BaseUrl);
});

builder.Services.AddHttpClient<ForecastsApiClient>((serviceProvider, httpClient) =>
{
    var apiOptions = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptionsMonitor<ApiOptions>>().CurrentValue;
    httpClient.BaseAddress = new Uri(apiOptions.BaseUrl);
});

builder.Services.AddHttpClient<ReplenishmentApiClient>((serviceProvider, httpClient) =>
{
    var apiOptions = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptionsMonitor<ApiOptions>>().CurrentValue;
    httpClient.BaseAddress = new Uri(apiOptions.BaseUrl);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler("/Home/Error");
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseStatusCodePagesWithReExecute("/Home/NotFoundPage");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
