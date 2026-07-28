namespace EvidenceRezervaceMistnosti.DTO.PartialView
{
    public class RoomRowDTO
    {
        public int RoomId { get; set; }
        public required string RoomName { get; set; }
        public int Capacity { get; set; }
        public required string LocationName { get; set; }
        public required string EquipmentText { get; set; }
    }
}
