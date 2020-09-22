using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CMSShoppingCart.Models
{
    public class Page
    {
        public int Id      { get; set; }
        public int Sorting { get; set; }
        public string Slug { get; set; }
        [
            Required(ErrorMessage = "The Title field is required"),
            MinLength(2, ErrorMessage = "Minimum length is 2"),
        ]
        public string Title { get; set; }
        [
            Required(ErrorMessage = "The Content field is required"),
            MinLength(4, ErrorMessage = "Minimum length is 4"),
            RegularExpression(@"^[a-zA-Z-]*$", ErrorMessage = "Only letters and dashes are allowed"),
        ]
        public string Content { get; set; }
    }
}
