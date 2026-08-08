using Microsoft.EntityFrameworkCore;

namespace EvidenceRezervaceMistnosti.Models
{
    public class ReservationContext : DbContext
    {
        public DbSet<Reservation> Reservation { get; set; }
        public DbSet<Room> Room { get; set; }
        public DbSet<Location> Location { get; set; }
        public DbSet<Equipment> Equipment { get; set; }
        public DbSet<RoomEquipment> RoomEquipment { get; set; }
        public ReservationContext(DbContextOptions<ReservationContext> options): base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Reservation>(e =>
            {
                e.HasKey(e => e.ReservationId);

                e.Property(e => e.ReservationId)
                    .ValueGeneratedOnAdd();

                e.HasOne(e => e.Room)
                    .WithMany()
                    .HasForeignKey(e => e.RoomId)
                    .OnDelete(DeleteBehavior.Cascade);


                e.HasData(
                      new Reservation
                      {
                          ReservationId = 21,
                          Email = "petr.svoboda@gmail.com",
                          DateReservation = new DateOnly(2026, 8, 10),
                          TimeFrom = new TimeOnly(9, 0),
                          TimeTo = new TimeOnly(11, 0),
                          RoomId = 1,
                          NumberOfPeople = 4,
                          Description = "Týmová porada k novému projektu",
                          LastName = "Svoboda",
                          Name = "Petr",
                          IsActive = true
                      },
                      new Reservation
                      {
                          ReservationId = 22,
                          Email = "jana.novotna@email.cz",
                          DateReservation = new DateOnly(2026, 8, 11),
                          TimeFrom = new TimeOnly(13, 30),
                          TimeTo = new TimeOnly(15, 0),
                          RoomId = 2,
                          NumberOfPeople = 6,
                          Description = "Prezentace výsledků marketingové kampaně",
                          LastName = "Novotná",
                          Name = "Jana",
                          IsActive = true
                      },
                      new Reservation
                      {
                          ReservationId = 23,
                          Email = "tomas.dvorak@gmail.com",
                          DateReservation = new DateOnly(2026, 8, 12),
                          TimeFrom = new TimeOnly(8, 0),
                          TimeTo = new TimeOnly(10, 30),
                          RoomId = 3,
                          NumberOfPeople = 8,
                          Description = "Školení nových zaměstnanců",
                          LastName = "Dvořák",
                          Name = "Tomáš",
                          IsActive = true
                      },
                      new Reservation
                      {
                          ReservationId = 24,
                          Email = "lucie.prochazkova@seznam.cz",
                          DateReservation = new DateOnly(2026, 8, 13),
                          TimeFrom = new TimeOnly(14, 0),
                          TimeTo = new TimeOnly(16, 0),
                          RoomId = 1,
                          NumberOfPeople = 3,
                          Description = "Konzultace závěrečné práce",
                          LastName = "Procházková",
                          Name = "Lucie",
                          IsActive = true
                      },
                      new Reservation
                      {
                          ReservationId = 25,
                          Email = "martin.kucera@email.cz",
                          DateReservation = new DateOnly(2026, 8, 14),
                          TimeFrom = new TimeOnly(10, 0),
                          TimeTo = new TimeOnly(12, 0),
                          RoomId = 2,
                          NumberOfPeople = 5,
                          Description = "Schůzka s obchodním partnerem",
                          LastName = "Kučera",
                          Name = "Martin",
                          IsActive = false
                      },
                      new Reservation
                      {
                          ReservationId = 26,
                          Email = "eva.vesela@gmail.com",
                          DateReservation = new DateOnly(2026, 8, 17),
                          TimeFrom = new TimeOnly(9, 30),
                          TimeTo = new TimeOnly(11, 30),
                          RoomId = 3,
                          NumberOfPeople = 10,
                          Description = "Workshop produktového týmu",
                          LastName = "Veselá",
                          Name = "Eva",
                          IsActive = true
                      },
                      new Reservation
                      {
                          ReservationId = 27,
                          Email = "jakub.horak@seznam.cz",
                          DateReservation = new DateOnly(2026, 8, 18),
                          TimeFrom = new TimeOnly(15, 0),
                          TimeTo = new TimeOnly(17, 30),
                          RoomId = 1,
                          NumberOfPeople = 2,
                          Description = "Pracovní pohovor",
                          LastName = "Horák",
                          Name = "Jakub",
                          IsActive = true
                      },
                      new Reservation
                      {
                          ReservationId = 28,
                          Email = "katerina.kralova@gmail.com",
                          DateReservation = new DateOnly(2026, 8, 19),
                          TimeFrom = new TimeOnly(8, 30),
                          TimeTo = new TimeOnly(10, 0),
                          RoomId = 2,
                          NumberOfPeople = 7,
                          Description = "Plánování firemní konference",
                          LastName = "Králová",
                          Name = "Kateřina",
                          IsActive = true
                      },
                      new Reservation
                      {
                          ReservationId = 29,
                          Email = "michal.benes@email.cz",
                          DateReservation = new DateOnly(2026, 8, 20),
                          TimeFrom = new TimeOnly(12, 0),
                          TimeTo = new TimeOnly(14, 0),
                          RoomId = 3,
                          NumberOfPeople = 4,
                          Description = "Kontrola průběhu vývoje aplikace",
                          LastName = "Beneš",
                          Name = "Michal",
                          IsActive = false
                      },
                      new Reservation
                      {
                          ReservationId = 30,
                          Email = "tereza.fialova@seznam.cz",
                          DateReservation = new DateOnly(2026, 8, 21),
                          TimeFrom = new TimeOnly(16, 0),
                          TimeTo = new TimeOnly(18, 0),
                          RoomId = 1,
                          NumberOfPeople = 6,
                          Description = "Setkání organizačního týmu",
                          LastName = "Fialová",
                          Name = "Tereza",
                          IsActive = true
                      }
                  );
            });

            modelBuilder.Entity<Room>(e =>
            {
                e.HasKey(e => e.RoomId);

                e.Property(e => e.RoomId)
                    .ValueGeneratedOnAdd();

                e.HasIndex(e => e.Name)
                    .IsUnique();

                e.HasOne(e => e.Location)
                    .WithMany()
                    .HasForeignKey(e => e.LocationId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasData(
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
                    });
            });

            modelBuilder.Entity<Location>(e =>
            {
                e.HasKey(e => e.LocationId);
                e.Property(e => e.LocationId)
                    .ValueGeneratedOnAdd();

                e.HasData(
                    new Location
                    {
                        LocationId = 1,
                        Name = "1st floor",
                        IsActive = true
                    },
                    new Location
                    {
                        LocationId = 2,
                        Name = "2nd floor",
                        IsActive = true
                    },
                    new Location
                    {
                        LocationId = 3,
                        Name = "3rd floor",
                        IsActive = true
                    },
                    new Location
                    {
                        LocationId = 4,
                        Name = "4th floor",
                        IsActive = true
                    },
                    new Location
                    {
                        LocationId = 5,
                        Name = "5th floor",
                        IsActive = true
                    },
                    new Location
                    {
                        LocationId = 6,
                        Name = "6th floor",
                        IsActive = true
                    });
            });

            modelBuilder.Entity<Equipment>(e =>
            {
                e.HasKey(e => e.EquipmentId);

                e.Property(e => e.EquipmentId)
                    .ValueGeneratedOnAdd();

                e.HasMany(e => e.RoomEquipment)
                    .WithOne(re => re.Equipment)
                    .HasForeignKey(re => re.EquipmentId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasData(
                    new Equipment
                    {
                        EquipmentId = 1,
                        Name = "Table",
                        IsActive = true
                    },
                    new Equipment
                    {
                        EquipmentId = 2,
                        Name = "Chair",
                        IsActive = true
                    },
                    new Equipment
                    {
                        EquipmentId = 3,
                        Name = "Projector",
                        IsActive = true
                    },
                    new Equipment
                    {
                        EquipmentId = 4,
                        Name = "Projection screen",
                        IsActive = true
                    },
                    new Equipment
                    {
                        EquipmentId = 5,
                        Name = "TV or monitor",
                        IsActive = true
                    },
                    new Equipment
                    {
                        EquipmentId = 6,
                        Name = "Air conditioning",
                        IsActive = true
                    },
                    new Equipment
                    {
                        EquipmentId = 7,
                        Name = "Microphone",
                        IsActive = true
                    },
                    new Equipment
                    {
                        EquipmentId = 8,
                        Name = "Computer",
                        IsActive = true
                    },
                    new Equipment
                    {
                        EquipmentId = 9,
                        Name = "Bed",
                        IsActive = true
                    });
            });

            modelBuilder.Entity<RoomEquipment>(e =>
            {
                e.HasKey(e => new { e.RoomId, e.EquipmentId });

                e.HasOne(e => e.Equipment)
                    .WithMany(e => e.RoomEquipment)
                    .HasForeignKey(e => e.EquipmentId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(e => e.Room)
                    .WithMany(e => e.RoomEquipment)
                    .HasForeignKey(e => e.RoomId) 
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasData(
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
            });
        }
    }
}
