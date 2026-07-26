using EvidenceRezervaceMistnosti.DTO.PartialView;

namespace EvidenceRezervaceMistnosti.Models.Shared
{
    public class HomepageViewModel
    {
        public bool ReservationOn { get; set; }
        public ReservationDashboardDTO? ReservationDashboard { get; set; }
        public RoomDashboardDTO? RoomDashboard { get; set; }
    }
}
