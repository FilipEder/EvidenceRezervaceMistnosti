namespace EvidenceRezervaceMistnosti.Models
{
    public class Room
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int Capacity { get; set; }
        public required string Location { get; set; }
        public required string Gear { get; set; }
    }
}
