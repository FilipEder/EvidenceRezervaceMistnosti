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
                
                db.Location.AddRange(
                    new Location
                    {
                        LocationId = 1,
                        Name = "1. patro",
                    },
                    new Location
                    {
                        LocationId = 2,
                        Name = "2. patro",
                    },
                    new Location
                    {
                        LocationId = 3,
                        Name = "3. patro",
                    },
                    new Location
                    {
                        LocationId = 4,
                        Name = "4. patro",
                    },
                    new Location
                    {
                        LocationId = 5,
                        Name = "5. patro",
                    },
                    new Location
                    {
                        LocationId = 6,
                        Name = "6. patro",
                    }
                );

                db.Equipment.AddRange(
                    new Equipment
                    {
                        EquipmentId = 1,
                        Name = "Stůl",
                    },
                    new Equipment
                    {
                        EquipmentId = 2,
                        Name = "Židle",
                    },
                    new Equipment
                    {
                        EquipmentId = 3,
                        Name = "Projektor",
                    },
                    new Equipment
                    {
                        EquipmentId = 4,
                        Name = "Projekční plátno",
                    },
                    new Equipment
                    {
                        EquipmentId = 5,
                        Name = "Televize nebo monitor",
                    },
                    new Equipment
                    {
                        EquipmentId = 6,
                        Name = "Klimatizace",
                    },
                    new Equipment
                    {
                        EquipmentId = 7,
                        Name = "Mikrofon",
                    },
                    new Equipment
                    {
                        EquipmentId = 8,
                        Name = "Počítač",
                    },
                    new Equipment
                    {
                        EquipmentId = 9,
                        Name = "Postel",
                    }
                );

                db.Room.AddRange(
                    new Room
                    {
                        RoomId = 1,
                        LocationId = 1,
                        Gear = "Projektor, Tabule",
                        Name = "Konferencni mistnost A",
                        Capacity = 10,
                    },
                    new Room
                    {
                        RoomId = 2,
                        LocationId = 2,
                        Gear = "Projektor, Tabule",
                        Name = "Konferencni mistnost B",
                        Capacity = 5,
                    },
                    new Room
                    {
                        RoomId = 3,
                        LocationId = 3,
                        Gear = "Projektor, reproduktor, Popcorn",
                        Name = "Kino Sál A",
                        Capacity = 20,
                    },
                    new Room
                    {
                        RoomId = 4,
                        LocationId = 4,
                        Gear = "Projektor, reproduktor",
                        Name = "Kino Sál B",
                        Capacity = 15,
                    }
                );

                db.RoomEquipment.AddRange(
                    new RoomEquipment
                    {
                        RoomId = 1,
                        EquipmentId = 3,
                    },
                    new RoomEquipment
                    {
                        RoomId = 1,
                        EquipmentId = 4,
                    }
                );

                db.Reservation.AddRange(
                    new Reservation
                    {
                        ReservationId = 1,
                        Email = "JanNovak@gmail.com",
                        DateReservation = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)),
                        TimeFrom = TimeOnly.FromDateTime(DateTime.Now.AddDays(-1)),
                        TimeTo = TimeOnly.FromDateTime(DateTime.Now.AddDays(-1).Add(TimeSpan.FromHours(2))),
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
