using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public abstract class BaseEquipmentDto
    {
        [Required(ErrorMessage = "Equipment name is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Equipment name must be between 3 and 50 characters")]
        public string EquipmentName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Serial number is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Serial number must be between 3 and 50 characters")]
        public string SerialNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Category ID must be greater than 0")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Location ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Location ID must be greater than 0")]
        public int LocationId { get; set; }

        [Required(ErrorMessage = "Purchase date is required")]
        public DateTime PurchaseDate { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public string Status { get; set; } = "Active";
    }
}


