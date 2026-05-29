using SistemaBase.Helpers;
using Microsoft.EntityFrameworkCore;
using SistemaBase.Models;
using System.Configuration;
using Microsoft.AspNetCore.Identity;
using SistemaBase.Interface.Pdf;
using SistemaBase.Service.Pdf;
using Grand.Web.Common.ViewRender;
using Wkhtmltopdf.NetCore;
using DinkToPdf.Contracts;
using DinkToPdf;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

var builder = WebApplication.CreateBuilder(args);

var cbrpySettings = builder.Configuration.GetSection("cbrpyAPISettings");

builder.Services.AddRazorPages()
    .AddRazorRuntimeCompilation();
// Add services to the container.
builder.Services.AddControllersWithViews();
//builder.Services.AddScoped<IViewCreate, ViewCreate>();
builder.Services.AddWkhtmltopdf();

builder.Services.AddScoped<IViewRenderService, ViewRenderService>();
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
builder.Services.AddScoped<IGeneratePdf, GeneratePdf>();
builder.Services.AddScoped<IPdfService, HtmlToPdfService>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, config =>
{
    config.AccessDeniedPath = "/Login/ErrorAcceso";
    config.LoginPath = "/Login/Index";
});




builder.Services.AddControllers();
//var connectionString = builder.Configuration.GetConnectionString("AppDb");
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();
builder.Services.AddDbContext<UAADbContext>(options => options.UseSqlServer("Data Source=DESKTOP-KP48E0B\\SQLEXPRESS; Integrated Security=SSPI; Initial Catalog=GestionTesis;"));
builder.Services.AddControllersWithViews(options => options.EnableEndpointRouting = false).AddSessionStateTempDataProvider();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ADMINISTRADORES", policy => policy.RequireRole("ADMIN"));
});

// Agregar la configuraci�n de HttpClientFactory
builder.Services.AddHttpClient("cbrpyClient", client =>
{
    client.BaseAddress = new Uri(cbrpySettings.GetValue<string>("BaseApiUrl")); 
});

var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();    
app.UseAuthorization();
app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");
app.Run();
