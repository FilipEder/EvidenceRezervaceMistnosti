using EvidenceRezervaceMistnosti.Models;

namespace EvidenceRezervaceMistnosti.DTO.Response
{
    public class EquipmentResponse
    {
        public int EquipmentId { get; set; }
        public required string Name { get; set; }
        public required bool IsActive { get; set; }
    }
}
