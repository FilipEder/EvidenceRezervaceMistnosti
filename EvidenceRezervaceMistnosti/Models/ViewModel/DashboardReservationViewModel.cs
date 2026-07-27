using EvidenceRezervaceMistnosti.DTO.PartialView;

namespace EvidenceRezervaceMistnosti.Models.ViewModel
{
    public class DashboardReservationViewModel
    {
        public required ReservationDashboardDTO ReservationDashboard { get; set; }
        public string? Search { get; set; }
        public int? NumberOfPeople { get; set; }
        public DateOnly? Day { get; set; }
        public TimeOnly? TimeFrom { get; set; }
        public TimeOnly? TimeTo { get; set; }
    }
}
