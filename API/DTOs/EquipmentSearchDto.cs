namespace API.DTOs
{
    public class EquipmentSearchDto
    {
        public string? EquipmentName { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public int? CategoryId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}