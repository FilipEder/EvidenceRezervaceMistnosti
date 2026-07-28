using EvidenceRezervaceMistnosti.DTO.PartialView;
using EvidenceRezervaceMistnosti.DTO.Select;

namespace EvidenceRezervaceMistnosti.Models.ViewModel
{
    public class RoomDashboardViewModel
    {
        public required RoomDashboardDTO RoomDashboard { get; set; }
        public List<LocationSelectDTO>? Locations { get; set; }
        public List<EquipmentSelectDTO>? Equipments { get; set; }
        public string? Search { get; set; }
        public int? LocationId { get; set; }
        public int? EquipmentId { get; set; }
        public int? CapacityFrom { get; set; }
        public int? CapacityTo { get; set; }
    }
}
