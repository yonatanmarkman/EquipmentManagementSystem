using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string CategoryName { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        // Navigation property
        public ICollection<Equipment> EquipmentItems { get; set; } = new List<Equipment>();
    }
}