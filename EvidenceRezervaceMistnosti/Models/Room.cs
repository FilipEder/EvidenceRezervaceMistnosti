namespace EvidenceRezervaceMistnosti.Models
{
    public class Room
    {
        public int RoomId { get; set; }
        public required string Name { get; set; }
        public int Capacity { get; set; }
        public int LocationId { get; set; }
        public required bool IsActive { get; set; }
        public virtual Location Location { get; set; } = null!;
        public virtual ICollection<Reservation> Reservations { get; set; } = null!;
        public virtual ICollection<RoomEquipment> RoomEquipment { get; set; } = null!;
    }
}
