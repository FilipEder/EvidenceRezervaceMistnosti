using EvidenceRezervaceMistnosti.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EvidenceRezervaceMistnosti
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            builder.Services.AddDbContext<ReservationContext>(options =>
                options
                .UseLazyLoadingProxies()
                .UseSqlite(connection)); // replacement za inmemory pamet, z duvodu nepodpory indexace

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ReservationContext>();

                db.Database.EnsureCreated(); //vytvoreni databaze v schemata
                // Neptam se databaze zda existuji values, protoze je to InMemory databaze
                
                db.Location.AddRange(
                    new Location
                    {
                        LocationId = 1,
                        Name = "1. patro",
                        IsActive = true
                    },
                    new Location
                    {
                        LocationId = 2,
                        Name = "2. patro",
                        IsActive = true 
                    },
                    new Location
                    {
                        LocationId = 3,
                        Name = "3. patro",
                        IsActive = true
                    },
                    new Location
                    {
                        LocationId = 4,
                        Name = "4. patro",
                        IsActive = true
                    },
                    new Location
                    {
                        LocationId = 5,
                        Name = "5. patro",
                        IsActive = true
                    },
                    new Location
                    {
                        LocationId = 6,
                        Name = "6. patro",
                        IsActive = true
                    }
                );

                db.Equipment.AddRange(
                    new Equipment
                    {
                        EquipmentId = 1,
                        Name = "Stůl",
                        IsActive = true
                    },
                    new Equipment
                    {
                        EquipmentId = 2,
                        Name = "Židle",
                        IsActive = true
                    },
                    new Equipment
                    {
                        EquipmentId = 3,
                        Name = "Projektor",
                        IsActive = true
                    },
                    new Equipment
                    {
                        EquipmentId = 4,
                        Name = "Projekční plátno",
                        IsActive = true
                    },
                    new Equipment
                    {
                        EquipmentId = 5,
                        Name = "Televize nebo monitor",
                        IsActive = true
                    },
                    new Equipment
                    {
                        EquipmentId = 6,
                        Name = "Klimatizace",
                        IsActive = true
                    },
                    new Equipment
                    {
                        EquipmentId = 7,
                        Name = "Mikrofon",
                        IsActive = true
                    },
                    new Equipment
                    {
                        EquipmentId = 8,
                        Name = "Počítač",
                        IsActive = true
                    },
                    new Equipment
                    {
                        EquipmentId = 9,
                        Name = "Postel",
                        IsActive = true
                    }
                );

                db.Room.AddRange(
                    new Room
                    {
                        RoomId = 1,
                        LocationId = 1,
                        Name = "Konferencni mistnost A",
                        Capacity = 10,
                        IsActive = true
                    },
                    new Room
                    {
                        RoomId = 2,
                        LocationId = 2,
                        Name = "Konferencni mistnost B",
                        Capacity = 5,
                        IsActive = true
                    },
                    new Room
                    {
                        RoomId = 3,
                        LocationId = 3,
                        Name = "Kino Sál A",
                        Capacity = 20,
                        IsActive = true
                    },
                    new Room
                    {
                        RoomId = 4,
                        LocationId = 4,
                        Name = "Kino Sál B",
                        Capacity = 15,
                        IsActive = true
                    }
                );

                db.RoomEquipment.AddRange(
                    new RoomEquipment
                    {
                        RoomId = 1,
                        EquipmentId = 3,
                        Count = 10
                    },
                    new RoomEquipment
                    {
                        RoomId = 1,
                        EquipmentId = 4,
                        Count = 10
                    }
                );

                for(int i = 1; i < 20; i++)
                {
                    db.Reservation.Add(
                    new Reservation
                    {
                        ReservationId = i,
                        Email = "JanNovak@gmail.com",
                        DateReservation = DateOnly.FromDateTime(DateTime.Now.AddDays(-i)),
                        TimeFrom = TimeOnly.FromDateTime(DateTime.Now.AddDays(-i)),
                        TimeTo = TimeOnly.FromDateTime(DateTime.Now.AddDays(-i).Add(TimeSpan.FromHours(i))),
                        RoomId = 1,
                        NumberOfPeople = 3,
                        Description = "Rezervace pro prezentaci mé práce",
                        LastName = "Novak",
                        Name = "Jan",
                        IsActive = true
                    }
                    );
                }
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
