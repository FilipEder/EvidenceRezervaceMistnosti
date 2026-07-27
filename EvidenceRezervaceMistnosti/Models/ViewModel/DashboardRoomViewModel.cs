using EvidenceRezervaceMistnosti.DTO.PartialView;
using EvidenceRezervaceMistnosti.DTO.Select;

namespace EvidenceRezervaceMistnosti.Models.ViewModel
{
    public class DashboardRoomViewModel
    {
        public required RoomFilterDTO RoomSelectValues { get; set; }
        public required RoomDashboardDTO RoomDashboard { get; set; }
    }
}
