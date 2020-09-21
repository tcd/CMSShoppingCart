using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CMSShoppingCart.Models
{
    public class Category
    {
        public int Id { get; set; }
        public int Sorting { get; set; }
        [
            Required(ErrorMessage = "Slug is required"),
        ]
        public string Slug { get; set; }
        [
            Required,
            MinLength(2, ErrorMessage = "Minimum length is 2"),
            RegularExpression(@"^[a-zA-Z]$", ErrorMessage = "Only letters are allowed")
        ]
        public string Name { get; set; }
    }
}
