using EvidenceRezervaceMistnosti.Models;

namespace EvidenceRezervaceMistnosti.DTO.Response
{
    public class ReservationResponse
    {
        public int ReservationId { get; set; }
        public required string ReservationName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public DateOnly DateReservation { get; set; }
        public required string TimeFrom { get; set; }
        public required string TimeTo { get; set; }
        public int NumberOfPeople { get; set; }
        public string? Description { get; set; }
        public int RoomId { get; set; }
        public required string RoomName { get; set; }
        public int RoomCapacity { get; set; }
        public int LocationId { get; set; }
        public bool ReservationIsActive { get;  set; }
        public bool RoomIsActive { get; set; }
    }
}
