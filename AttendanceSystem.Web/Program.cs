using Microsoft.EntityFrameworkCore;
using AttendanceSystem.Data;

namespace AttendanceSystem.Web
    {
    public class Program
        {
        /// <summary>
        /// //
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
            {
            var builder = WebApplication.CreateBuilder(args);

            // Configure API Client
            builder.Services.AddHttpClient<AttendanceSystem.Web.Services.IApiClient, AttendanceSystem.Web.Services.ApiClient>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:5056/api/");
            });
            
            builder.Services.AddAuthentication("Cookies")
                .AddCookie("Cookies", options =>
                {
                    options.LoginPath = "/Auth/Login";
                    options.LogoutPath = "/Auth/Logout";
                    options.AccessDeniedPath = "/Auth/AccessDenied";
                    options.ExpireTimeSpan = TimeSpan.FromHours(12);
                });

            builder.Services.AddAuthorization(options =>
                {
                    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                    options.AddPolicy("LecturerOnly", policy => policy.RequireRole("Lecturer"));
                    options.AddPolicy("StudentOnly", policy => policy.RequireRole("Student"));
                });

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
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

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
            }
        }
    }
