using EvidenceRezervaceMistnosti.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;

namespace EvidenceRezervaceMistnosti
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<ReservationContext>(options =>
                options.UseInMemoryDatabase("RezervaceDB"));

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ReservationContext>();

                db.Database.EnsureCreated();
                // Neptam se databaze zda existuji values, protoze je to InMemory databaze

                db.Room.AddRange(
                    new Room
                    {
                        Id = 1,
                        Location = "1. patro",
                        Gear = "Projektor, Tabule",
                        Name = "Konferencni mistnost A",
                        Capacity = 10,
                    },
                    new Room
                    {
                        Id = 2,
                        Location = "2. patro",
                        Gear = "Projektor, Tabule",
                        Name = "Konferencni mistnost B",
                        Capacity = 5,
                    },
                    new Room
                    {
                        Id = 3,
                        Location = "3. patro",
                        Gear = "Projektor, reproduktor, Popcorn",
                        Name = "Kino Sál A",
                        Capacity = 20,
                    },
                    new Room
                    {
                        Id = 4,
                        Location = "3. patro",
                        Gear = "Projektor, reproduktor",
                        Name = "Kino Sál B",
                        Capacity = 15,
                    }
                );

                db.Reservation.AddRange(
                    new Reservation
                    {
                        Id = 1,
                        Email = "JanNovak@gmail.com",
                        DateReservation = DateTime.Now,
                        TimeFrom = DateTime.Now.AddDays(-1).TimeOfDay,
                        TimeTo = DateTime.Now.AddDays(-1).TimeOfDay.Add(TimeSpan.FromHours(2)),
                        RoomId = 1,
                        NumberOfPeople = 3,
                        Description = "Rezervace pro prezentaci mé práce",
                        LastName = "Novak",
                        Name = "Jan",
                    }
                );
                db.SaveChanges();
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
