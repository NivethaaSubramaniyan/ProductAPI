using System.ComponentModel.DataAnnotations;

namespace ProductAPI.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; } // 6-digit unique ID

        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        
    }
}
