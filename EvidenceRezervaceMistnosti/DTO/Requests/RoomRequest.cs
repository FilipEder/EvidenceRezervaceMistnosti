using System.ComponentModel.DataAnnotations;

namespace EvidenceRezervaceMistnosti.DTO.Requests
{
    public class RoomRequest
    {
        [Required(ErrorMessage = "Název místnosti je povinný")]
        [MaxLength(120, ErrorMessage = "Název místnosti nesmí být delší než 120 znaků")]
        [MinLength(5, ErrorMessage = "Název místnosti musí být alespoň 5 znaků dlouhý")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Kapacita místnosti je povinná")]
        [Range(1, 1000, ErrorMessage = "Kapacita musí být od 1 do 1000")]
        public required int Capacity { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Umístění místnosti je povinné")]
        public required int LocationId { get; set; }
        public List<int>? GearIds { get; set; }
    }
}
