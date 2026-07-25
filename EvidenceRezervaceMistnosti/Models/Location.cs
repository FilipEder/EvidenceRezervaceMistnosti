namespace EvidenceRezervaceMistnosti.Models
{
    public class Location
    {
        public int LocationId { get; set; }
        public required string Name { get; set; }
        public bool IsActive { get; set; }
    }
}
