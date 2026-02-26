using Microsoft.EntityFrameworkCore;
using ProyectoRestauranteC_.Data;
using ProyectoRestauranteC_.Models;
using ProyectoRestauranteC_.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession();

builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<RestauranteContext>
    (options =>options.UseSqlServer(connectionString: builder.Configuration.GetConnectionString("RestauranteDB")));

builder.Services.AddTransient<RepositoryUsuarios>();
builder.Services.AddScoped<IMenuRepository, MenuRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
