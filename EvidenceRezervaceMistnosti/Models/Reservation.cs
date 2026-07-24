namespace EvidenceRezervaceMistnosti.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        // Rezervace pro
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        // Informace o rezervaci
        public DateTime DateReservation { get; set; }
        public TimeSpan TimeFrom { get; set; }
        public TimeSpan TimeTo { get; set; }
        public int NumberOfPeople { get; set; }
        public string? Description { get; set; }
        // Mistnost
        public int RoomId { get; set; }
        public Room Room { get; set; } = null!;
    }
}
