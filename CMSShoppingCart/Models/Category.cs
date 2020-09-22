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
            Required(ErrorMessage = "The Slug field is required"),
        ]
        public string Slug { get; set; }

        [
            Required(ErrorMessage = "The Name field is required"),
            MinLength(2, ErrorMessage = "Minimum length is 2"),
            RegularExpression(@"^[a-zA-Z-]*$", ErrorMessage = "Only letters and dashes are allowed"),
        ]
        public string Name { get; set; }

    }
}
