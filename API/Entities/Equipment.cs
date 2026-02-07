using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    public class Equipment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string EquipmentName { get; set; } = string.Empty;

        [Required]
        public string SerialNumber { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int LocationId { get; set; }

        [Required]
        public DateTime PurchaseDate { get; set; }

        [Required]
        public EquipmentStatus Status { get; set; } = EquipmentStatus.Active;

        // Navigation properties
        [ForeignKey("CategoryId")]
        public Category Category { get; set; } = null!;

        [ForeignKey("LocationId")]
        public Location Location { get; set; } = null!;
    }
}


