using EvidenceRezervaceMistnosti.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using System.Globalization;

namespace EvidenceRezervaceMistnosti
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            builder.Services.AddDbContext<ReservationContext>(options =>
                options
                .UseLazyLoadingProxies()
                .UseSqlite(connection)); // replacement za inmemory pamet, z duvodu nepodpory indexace


            builder.Services.AddLocalization(opts =>
            {
                opts.ResourcesPath = "Resources";
            });

            builder.Services
                .AddControllersWithViews()
                .AddDataAnnotationsLocalization();

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Evidence rezervací místností API",
                    Version = "v1",
                    Description = "API pro správu místností, rezervací, umístění a vybavení."
                });
            });

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ReservationContext>();
                db.Database.EnsureCreated();
            }

            var supportedCultures = new[]
           {
                new CultureInfo("cs-CZ"),
                new CultureInfo("en-US"),
                new CultureInfo("de-DE")
            };

            var localizationOptions = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("cs-CZ"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            };

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Evidence rezervací místností API v1");
                    options.DocumentTitle = "Evidence rezervací místností API";
                });
            }
            else
            {
                app.UseExceptionHandler("/error");
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/error/{0}");
            app.UseHttpsRedirection();

            app.UseRequestLocalization(localizationOptions);
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllers().WithStaticAssets();
            app.Run();
        }
    }
}
