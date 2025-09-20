using System.ComponentModel.DataAnnotations;

namespace PracticoII_Web.Data.Models
{
    public class Login
    {
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato inválido")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        public string? Contraseña { get; set; }

        public bool Recordarme { get; set; }
    }
}
