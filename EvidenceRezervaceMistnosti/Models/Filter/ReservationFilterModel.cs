namespace EvidenceRezervaceMistnosti.Models.Filter
{
    public class ReservationFilterModel
    {
        public int? NumberOfPeople { get; set; }
        public DateOnly? Day { get; set; }
        public TimeOnly? TimeFrom { get; set; }
        public TimeOnly? TimeTo { get; set; }
    }
}
