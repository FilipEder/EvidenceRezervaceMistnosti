using System.ComponentModel.DataAnnotations;

namespace EvidenceRezervaceMistnosti.DTO.Requests
{
    public class ReservationRequest
    {
        [Required(ErrorMessage = "Jméno je povinný")]
        [MaxLength(30, ErrorMessage = "Jméno nesmí být delší než 30 znaků")]
        [MinLength(2, ErrorMessage = "Jméno musí být alespoň 2 znaky dlouhý")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Příjmení je povinné")]
        [MaxLength(30, ErrorMessage = "Příjmení nesmí být delší než 30 znaků")]
        [MinLength(2, ErrorMessage = "Příjmení musí být alespoň 2 znaky dlouhý")]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "E-mail je povinný")]
        [EmailAddress(ErrorMessage = "Neplatný formát e-mailu")]
        public required string Email { get; set; }

        // Informace o rezervaci
        [Required(ErrorMessage = "Datum rezervace je povinný")]
        public DateOnly DateReservation { get; set; }

        [Required(ErrorMessage = "Čas od je povinný")]
        public TimeOnly TimeFrom { get; set; }

        [Required(ErrorMessage = "Čas do je povinný")]
        public TimeOnly TimeTo { get; set; }

        [Required(ErrorMessage = "Počet osob je povinný")]
        [Range(1, 1000, ErrorMessage = "Počet osob musí být mezi 1 a 1000")]
        public int NumberOfPeople { get; set; }

        [MaxLength(500, ErrorMessage = "Popis nesmí být delší než 500 znaků")]
        [MinLength(4, ErrorMessage = "Popis musí být alespoň 4 znaků dlouhý")]
        public string? Description { get; set; }
        // Mistnost
        [Required(ErrorMessage = "Mistnost je povinná")]
        public int RoomId { get; set; }
    }
}
