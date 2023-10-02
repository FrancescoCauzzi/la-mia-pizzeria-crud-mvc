﻿using la_mia_pizzeria_crud_mvc.ValidationAttributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace la_mia_pizzeria_crud_mvc.Models
{
    public class Pizza
    {
        
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage ="This field is mandatory")]
        [MaxLength(100, ErrorMessage = $"The name must not exceed 100 characters")]
        public string Name { get; set; }


        [Required(ErrorMessage = "This field is mandatory")]
        [MaxLength(1000, ErrorMessage = $"The name must not exceed 1000 characters")]
        [Column(TypeName = "text")]
        [MoreThanFiveWords]
        public string Description { get; set; }

        [Required(ErrorMessage = "This field is mandatory")]
        [Column(TypeName = "decimal(18, 2)")]
        [Range(0.1,100)]
        //[NoCommaAllowed]        
        public decimal Price { get; set; }

        // Custom Validation
        [UrlOrFilePath]
        [MaxLength(500)]        
        public string ImageUrl { get; set; } = "/images/default_pizza.png";

        // Empty constructor
        public Pizza() {
            
        }

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
            // ImageUrl will use the default value 
        }
    }
}
