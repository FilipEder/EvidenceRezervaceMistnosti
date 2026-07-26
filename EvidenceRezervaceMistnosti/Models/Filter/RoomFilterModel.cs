namespace EvidenceRezervaceMistnosti.Models.Filter
{
    public class RoomFilterModel
    {
        public int LocationId { get; set; }
        public List<int>? EquipmentId { get; set; }
        public int Capacity { get; set; }
    }
}
