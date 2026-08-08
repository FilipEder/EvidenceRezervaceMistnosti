using EvidenceRezervaceMistnosti.DTO.PartialView;
using EvidenceRezervaceMistnosti.DTO.Select;
namespace EvidenceRezervaceMistnosti.Models.ViewModel
{
    public class ReservationDashboardViewModel
    {
        public required ReservationDashboardDTO ReservationDashboard { get; set; }
        public required List<RoomSelectDTO> Rooms { get; set; }
        public string? Search { get; set; }
        public int? RoomId { get; set; }
        public int? NumberOfPeople { get; set; }
        public DateOnly? Day { get; set; }
        public TimeOnly? TimeFrom { get; set; }
        public TimeOnly? TimeTo { get; set; }
    }
}
