namespace EvidenceRezervaceMistnosti.Models
{
    public class Equipment
    {
        public int EquipmentId { get; set; }
        public required string Name { get; set; }
        public bool IsActive { get; set; }
    }
}
