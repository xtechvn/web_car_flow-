﻿//https://learn.microsoft.com/vi-vn/aspnet/core/tutorials/first-mvc-app/adding-controller?view=aspnetcore-6.0&tabs=visual-studio
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Memory;
using XTECH_FRONTEND.IRepositories;
using XTECH_FRONTEND.Repositories;
using XTECH_FRONTEND.Services;

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
builder.Services.AddSingleton<IMongoService, MongoService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.MapRazorPages();
app.UseAuthentication();
app.Run();