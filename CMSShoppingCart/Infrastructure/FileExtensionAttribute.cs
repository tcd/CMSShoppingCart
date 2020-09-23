using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace CMSShoppingCart.Infrastructure 
{
    public class FileExtensionAttribute : ValidationAttribute 
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext) 
        {
            // var context = (CMSShoppingCartContext)validationContext.GetService(typeof(CMSShoppingCartContext));
            var file = value as IFormFile;
            if (file != null) {
                var extension = Path.GetExtension(file.FileName);
                bool result = AllowedExtensions().Any(x => extension.EndsWith(x));
                if (!result) {
                    return new ValidationResult(InvalidErrorMessage());
                }
            }
            return ValidationResult.Success;
        }

        private string[] AllowedExtensions() 
        {
            return new string[]{ "jpg", "jpeg", "png" };
        }

        private string InvalidErrorMessage() 
        {
            var allowed = string.Join(", ", AllowedExtensions().ToArray());
            return ("Allowed file extensions are: " + allowed);
        }

    }
}
