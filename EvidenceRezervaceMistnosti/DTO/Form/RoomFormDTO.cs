using EvidenceRezervaceMistnosti.DTO.Select;

namespace EvidenceRezervaceMistnosti.DTO.Form
{
    public class RoomFormDTO
    {
        public required List<EquipmentSelectDTO> EquipmentSelect { get; set; }
        public required List<LocationSelectDTO> LocationSelect { get; set; }
    }
}
