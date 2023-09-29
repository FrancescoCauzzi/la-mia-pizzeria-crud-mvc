using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace la_mia_pizzeria_crud_mvc.Models
{
    public class Pizza
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [Column(TypeName = "text")]
        public string Description { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        [MaxLength(500)]        

        public string ImageUrl { get; set; } = "/images/default_pizza.png";



        public Pizza(string name, string description,decimal price, string imageUrl)
        {
            Name = name;
            Description = description;
            Price = price;
            ImageUrl = imageUrl;
        }
        // constructor overload
        public Pizza(string name, string description, decimal price)
        {
            Name = name;
            Description = description;
            Price = price;
            // ImageUrl will use the default value string.Empty
        }


    }
}
