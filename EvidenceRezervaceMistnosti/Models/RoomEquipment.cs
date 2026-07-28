namespace EvidenceRezervaceMistnosti.Models
{
    public class RoomEquipment
    {
        public int RoomId { get; set; }
        public int EquipmentId { get; set; }
        public virtual Room Room { get; set; } = null!;
        public virtual Equipment Equipment { get; set; } = null!;
    }
}
