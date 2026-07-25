using EvidenceRezervaceMistnosti.Models;

namespace EvidenceRezervaceMistnosti.DTO.Response
{
    public class RoomResponse
    {
        public int RoomId { get; set; }
        public required string Name { get; set; }
        public int Capacity { get; set; }
        public int LocationId { get; set; }
        public required string LocationName { get; set; }
        public required bool RoomIsActive { get; set; }
        public bool LocationIsActive { get; set; }

    }
}
