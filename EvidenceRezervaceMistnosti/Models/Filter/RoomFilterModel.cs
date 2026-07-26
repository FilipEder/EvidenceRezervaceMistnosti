namespace EvidenceRezervaceMistnosti.Models.Filter
{
    public class RoomFilterModel
    {
        public int? LocationId { get; set; }
        public int? EquipmentId { get; set; }
        public int? CapacityFrom { get; set; }
        public int? CapacityTo { get; set; }
    }
}
