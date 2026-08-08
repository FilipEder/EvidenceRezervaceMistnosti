using EvidenceRezervaceMistnosti.DTO.Select;

namespace EvidenceRezervaceMistnosti.Models.ViewModel
{
    public class ReservationDetailViewModel
    {
        public int ReservationId { get; set; }
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public int SelectedRoomId { get; set; }
        public int NumberOfPeople { get; set; }
        public DateOnly Day { get; set; }
        public TimeOnly TimeFrom { get; set; }
        public TimeOnly TimeTo { get; set; }
        public string? Description { get; set; }
        public List<RoomSelectDTO>? Rooms { get; set; }
    }
}
