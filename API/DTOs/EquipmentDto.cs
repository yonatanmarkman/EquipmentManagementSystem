namespace API.DTOs
{
    public class EquipmentDto : BaseEquipmentDto
    {
        public int Id { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string LocationName { get; set; } = string.Empty;
    }
}
