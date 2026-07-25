namespace EvidenceRezervaceMistnosti.Models
{
    public class Room
    {
        public int RoomId { get; set; }
        public required string Name { get; set; }
        public int Capacity { get; set; }
        public required string Gear { get; set; }
        public bool IsActive { get; set; }
        public int LocationId { get; set; }
        public virtual ICollection<Reservation>? Reservations { get; set; }
        public virtual Location? Location { get; set; }
    }
}
