namespace EvidenceRezervaceMistnosti.Models
{
    public class Reservation
    {
        public int ReservationId { get; set; }
        // Rezervace pro
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        // Informace o rezervaci
        public DateOnly DateReservation { get; set; }
        public TimeOnly TimeFrom { get; set; }
        public TimeOnly TimeTo { get; set; }
        public int NumberOfPeople { get; set; }
        public string? Description { get; set; }
        // Mistnost
        public int RoomId { get; set; }
        public virtual Room? Room { get; set; }
    }
}
