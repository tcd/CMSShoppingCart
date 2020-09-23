using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using CMSShoppingCart.Infrastructure;

namespace CMSShoppingCart.Models
{
    public class Product
    {

        /// <summary>
        /// Primary Key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// A URL-friendly representation of <see cref="Name"/>.
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// Name of the Product.
        /// </summary>
        [
            Required(ErrorMessage = "The Name field is required"),
            MinLength(2, ErrorMessage = "Minimum length is 2"),
        ]
        public string Name { get; set; }

        /// <summary>
        /// A <a href="https://en.wikipedia.org/wiki/Rich_Text_Format">Rich Text</a> description of the product.
        /// </summary>
        [
            Required(ErrorMessage = "The Description field is required"),
            MinLength(4, ErrorMessage = "Minimum length is 4"),
        ]
        public string Description { get; set; }

         /// <summary>
         /// Price of the Product.
         /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public string Image { get; set; }

        [
            NotMapped,
            FileExtension,
        ]
        public IFormFile ImageUpload { get; set; }

        [
            Display(Name = "Category"),
            // Range(1, int.MaxValue, ErrorMessage = "You must choose a category"),
        ]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

    }
}
