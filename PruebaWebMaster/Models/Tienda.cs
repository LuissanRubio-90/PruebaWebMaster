using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace PruebaWebMaster.Models
{
    public class Tienda
    {
        public int Id { get; set; } //Opcional

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength:9, ErrorMessage = "La longitud maxima del campo {0} debe de ser de 9 caracteres")]
        public string Telefono { get; set; }
    }
}
