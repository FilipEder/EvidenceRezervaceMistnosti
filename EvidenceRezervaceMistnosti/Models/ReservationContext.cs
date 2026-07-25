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
            });

            modelBuilder.Entity<Location>(e =>
            {
                e.HasKey(e => e.LocationId);
                e.Property(e => e.LocationId)
                    .ValueGeneratedOnAdd();
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
            });
        }
    }
}
