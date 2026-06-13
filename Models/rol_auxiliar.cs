using System.ComponentModel.DataAnnotations;

namespace chaves_dayron_proyecto2_3101.Models
{
    public class rol_auxiliar
    {
        [Display(Name = "Proprietario")]
        public bool isProprietario { get; set; }
        [Display(Name = "Administrador")]
        public bool isAdministrador { get; set; }
        [Display(Name = "Contribuidor")]
        public bool isContribuidor { get; set; }
        [Display(Name = "Lector")]
        public bool isLector { get; set; }
        [Display(Name = "Invitado")]
        public bool isInvitado { get; set; }
    }
}