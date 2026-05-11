using Microsoft.AspNetCore.Authentication.Cookies;
using ProyectoRestauranteC_.Repositories;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient("ApiTopMeal", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiTopMeal:BaseUrl"]!);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddTransient<RepositoryUsuarios>();
builder.Services.AddTransient<RepositoryReservas>();
builder.Services.AddTransient<RepositoryValoraciones>();
builder.Services.AddTransient<RepositoryHome>();
builder.Services.AddTransient<RepositoryAdmin>();
builder.Services.AddScoped<IMenuRepository, MenuRepository>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;  
}).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.LoginPath = "/Acceso/Login";
    options.AccessDeniedPath = "/Acceso/Denegado";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
});

builder.Services.AddAuthorization();

// Session solo para el carrito
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
