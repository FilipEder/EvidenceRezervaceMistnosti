using EvidenceRezervaceMistnosti.DTO.Select;

namespace EvidenceRezervaceMistnosti.Models.ViewModel
{
    public class RoomDetailViewModel
    {
        public required string Name { get; set; }
        public int Capacity { get; set; }
        public List<int>? SelectedGearId { get; set; }
        public int SelectedLocationId { get; set; }
        public List<LocationSelectDTO>? LocationSelect { get; set; }
        public List<EquipmentSelectDTO>? EquipmentSelect { get; set; }
    }
}
