namespace EvidenceRezervaceMistnosti.DTO.PartialView
{
    public class ReservationRowDTO
    {
        public required string ReservatioName { get; set; }
        public required string RoomName { get; set; }
        public required string UserName { get; set; }
        public int NumberOfPeople { get; set; }
        public required string DayReservation { get; set; }
        public required string TimeReservation { get; set; } // Format - 09:00–10:30
    }
}
