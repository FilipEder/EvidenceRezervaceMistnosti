namespace EvidenceRezervaceMistnosti.DTO.Response
{
    public class LocationResponse
    {
        public int LocationId { get; set; }
        public required string Name { get; set; }
        public required bool IsActive { get; set; }
    }
}
