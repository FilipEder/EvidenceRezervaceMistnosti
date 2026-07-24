using Microsoft.EntityFrameworkCore;

namespace EvidenceRezervaceMistnosti.Models
{
    public class ReservationContext : DbContext
    {
        public DbSet<Reservation> Reservation { get; set; }
        public DbSet<Room> Room { get; set; }

        public ReservationContext(DbContextOptions<ReservationContext> options)
            : base(options)
        {
        }
    }
}
