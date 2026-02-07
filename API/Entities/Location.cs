using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
    public class Location
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(25)]
        public string LocationName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Building { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Floor { get; set; } = string.Empty;

        // Navigation property
        public ICollection<Equipment> EquipmentItems { get; set; } = new List<Equipment>();
    }
}