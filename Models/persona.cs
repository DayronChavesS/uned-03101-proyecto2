using System.ComponentModel.DataAnnotations;

namespace chaves_dayron_proyecto2_3101.Models
{
    public class persona
    {
        public int persona_id { get; set; }
        [Required(ErrorMessage = "Complete este campo.")]
        [StringLength(12, ErrorMessage = "La cedula es demasiada extensa. (12 Letras Max.)")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Ingrese solamente numeros.")]
        [Display(Name = "Cedula")]
        public string cedula { get; set; }

        [Required(ErrorMessage = "Complete este campo.")]
        [StringLength(255, ErrorMessage = "El nombre es demasiado extenso. (255 Letras Max.)")]
        [RegularExpression(@"^[a-zA-Z\sÑñÀ-ÿ]*$", ErrorMessage = "Se introdujo un caracter no permitido.")]
        [Display(Name = "Nombre")]
        public string nombre { get; set; }

        [Required(ErrorMessage = "Complete este campo.")]
        [StringLength(255, ErrorMessage = "El apellido es demasiado extenso. (255 Letras Max.)")]
        [RegularExpression(@"^[a-zA-Z\sÑñÀ-ÿ]*$", ErrorMessage = "Se introdujo un caracter no permitido.")]
        [Display(Name = "Apellido")]
        public string apellido { get; set; }

        [Required(ErrorMessage = "Complete este campo.")]
        [StringLength(255, ErrorMessage = "La direccion es demasiada extensa. (255 Letras Max.)")]
        [Display(Name = "Direccion")]
        public string direccion { get; set; }

        [Required(ErrorMessage = "Complete este campo.")]
        [StringLength(50, ErrorMessage = "El correo es demasiado extenso. (50 Letras Max.)")]
        [EmailAddress(ErrorMessage = "El formato ingresado no es correcto, intentelo de nuevo.")]
        [Display(Name = "Correo electronico")]
        public string correo { get; set; }

        [Required(ErrorMessage = "Complete este campo.")]
        [StringLength(50, ErrorMessage = "El telefono es demasiado extenso. (50 Letras Max.)")]
        [Phone(ErrorMessage = "El formato ingresado no es correcto, intentelo de nuevo.")]
        [Display(Name = "Telefono")]
        public string telefono { get; set; }
    }
}