namespace API.DTOs
{
    public class EquipmentSeedDto
    {
        public string EquipmentName { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string LocationName { get; set; } = string.Empty;
        public DateTime PurchaseDate { get; set; }
        public string Status { get; set; } = "Active";
    }
}

