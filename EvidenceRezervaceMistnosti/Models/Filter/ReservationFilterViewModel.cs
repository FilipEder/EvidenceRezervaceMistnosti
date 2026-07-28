using System.ComponentModel.DataAnnotations;

namespace EvidenceRezervaceMistnosti.Models.Filter
{
    public class ReservationFilterViewModel
    {
        public string? Search { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Počet osob musí být kladný")]
        public int? NumberOfPeople { get; set; }
        public DateOnly? Day { get; set; }
        public TimeOnly? TimeFrom { get; set; }
        public TimeOnly? TimeTo { get; set; }
    }
}
