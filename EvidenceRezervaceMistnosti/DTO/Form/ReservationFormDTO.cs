using EvidenceRezervaceMistnosti.DTO.Select;

namespace EvidenceRezervaceMistnosti.DTO.Form
{
    public class ReservationFormDTO
    {
        public required List<RoomSelectDTO> Rooms { get; set; }
    }
}
