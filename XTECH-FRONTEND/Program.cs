﻿//https://learn.microsoft.com/vi-vn/aspnet/core/tutorials/first-mvc-app/adding-controller?view=aspnetcore-6.0&tabs=visual-studio
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Memory;
using XTECH_FRONTEND.IRepositories;
using XTECH_FRONTEND.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            // Cấu hình các tùy chọn khác cho cookie, ví dụ:
            options.LoginPath = "/login";
        });

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IGoogleSheetsService, GoogleSheetsService>();
builder.Services.AddSingleton<IGoogleFormsService, GoogleFormsService>();
builder.Services.AddSingleton<IValidationService, ValidationService>();
builder.Services.AddSingleton<IMemoryCache, MemoryCache>();
builder.Services.AddSingleton<IZaloService, ZaloOfficialAccountService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.MapControllerRoute(
    name: "News",
    pattern: "/dang-ky-dang-nhap",
    defaults: new { controller = "LoginOrRegister", action = "Index" });
app.MapControllerRoute(
    name: "News",
    pattern: "/tin-tuc",
    defaults: new { controller = "News", action = "Index" });
app.MapControllerRoute(
    name: "News",
    pattern: "/tin-tuc/{id}",
    defaults: new { controller = "News", action = "Detail" });
app.MapControllerRoute(
    name: "AboutUs",
    pattern: "/ve-chung-toi",
    defaults: new { controller = "AboutUs", action = "Index" });
app.MapControllerRoute(
    name: "pricing",
    pattern: "/bang-gia-thiet-ke-website",
    defaults: new { controller = "Pricing", action = "Index" });
app.MapControllerRoute(
    name: "home",
    pattern: "/trang-chu",
    defaults: new { controller = "Home", action = "Index" });
app.MapControllerRoute(
    name: "Contact",
    pattern: "/lien-he",
    defaults: new { controller = "Contact", action = "Index" });
app.MapControllerRoute(
    name: "faq",
    pattern: "/dich-vu",
    defaults: new { controller = "FAQ", action = "Index" });
app.MapControllerRoute(
    name: "faq",
    pattern: "/dich-vu/chi-tiet-du-an",
    defaults: new { controller = "FAQ", action = "DetailDeginwebsite" });
app.MapControllerRoute(
    name: "faq",
    pattern: "/dich-vu/vps",
    defaults: new { controller = "FAQ", action = "DetailVPS" });
app.MapControllerRoute(
    name: "faq",
    pattern: "/dich-vu/dang-ky-vps",
    defaults: new { controller = "FAQ", action = "DangkyVPS" });
app.MapControllerRoute(
    name: "faq",
    pattern: "/dich-vu/crawl-api",
    defaults: new { controller = "FAQ", action = "CrawlAPIindex" });
app.MapControllerRoute(
    name: "home",
    pattern: "/",
    defaults: new { controller = "Home", action = "Index" });
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.MapRazorPages();
app.UseAuthentication();
app.Run();