// DbScriptManager.WebUI/Models/LoginViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace DbScriptManager.WebUI.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Kullanıcı kodu zorunludur")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre zorunludur")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}