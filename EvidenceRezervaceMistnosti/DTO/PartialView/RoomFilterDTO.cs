using EvidenceRezervaceMistnosti.DTO.Select;
using EvidenceRezervaceMistnosti.Models;

namespace EvidenceRezervaceMistnosti.DTO.PartialView
{
    public class RoomFilterDTO
    {
        public List<LocationSelectDTO>? Locations { get; set; }
        public List<EquipmentSelectDTO>? Equipments { get; set; }
    }
}
