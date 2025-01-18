using System.ComponentModel.DataAnnotations;

namespace EcomApplication.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public byte[] Image { get; set; }

        public int CategoryId { get; set; } // Non nullable
        public Category Category { get; set; } // Relation obligatoire
    }
}
