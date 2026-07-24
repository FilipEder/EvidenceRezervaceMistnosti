using System.ComponentModel.DataAnnotations;

namespace EvidenceRezervaceMistnosti.DTO
{
    public class RoomDTORequest
    {
        [Required(ErrorMessage = "Název místnosti je povinný")]
        [MaxLength(120, ErrorMessage = "Název místnosti nesmí být delší než 120 znaků")]
        [MinLength(5, ErrorMessage = "Název místnosti musí být alespoň 5 znaků dlouhý")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Kapacita místnosti je povinná")]
        [Range(1, 1000, ErrorMessage = "Kapacita musí být od 1 do 1000")]
        public required int Capacity { get; set; }
        [Required(ErrorMessage = "Umístění místnosti je povinné")]
        [MaxLength(120, ErrorMessage = "Umístění místnosti nesmí být delší než 120 znaků")]
        [MinLength(5, ErrorMessage = "Umístění místnosti musí být alespoň 5 znaků dlouhé")]
        public required string Location { get; set; }
        [Required(ErrorMessage = "Vybavení místnosti je povinné")]
        [MaxLength(120, ErrorMessage = "Vybavení místnosti nesmí být delší než 120 znaků")]
        [MinLength(5, ErrorMessage = "Vybavení místnosti musí být alespoň 5 znaků dlouhé")]
        public required string Gear { get; set; }
    }
}
