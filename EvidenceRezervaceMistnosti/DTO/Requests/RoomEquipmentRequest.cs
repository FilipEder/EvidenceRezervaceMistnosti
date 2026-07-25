using System.ComponentModel.DataAnnotations;

namespace EvidenceRezervaceMistnosti.DTO.Requests
{
    public class RoomEquipmentRequest
    {
        public int GearId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Počet musí být kladný")]
        public int Count { get; set; }
    }
}
