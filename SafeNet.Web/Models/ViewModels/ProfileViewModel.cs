// SafeNet.Web/Models/ViewModels/ProfileViewModel.cs
// AGREGAR este archivo nuevo
using System.ComponentModel.DataAnnotations;

namespace SafeNet.Web.Models.ViewModels
{
    public class ProfileViewModel
    {
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Nombre de usuario")]
        public string UserName { get; set; } = string.Empty;

        // ── Cambio de contraseña (opcional) ──────────────────────────────────

        [DataType(DataType.Password)]
        [Display(Name = "Contraseña actual")]
        public string? CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Nueva contraseña")]
        [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar nueva contraseña")]
        [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden.")]
        public string? ConfirmPassword { get; set; }
    }
}
