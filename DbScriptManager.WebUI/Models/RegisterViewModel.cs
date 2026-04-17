using System.ComponentModel.DataAnnotations;

namespace DbScriptManager.WebUI.Models
{
    public class RegisterViewModel
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        public string UserName{ get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password",ErrorMessage = "Şifreler Eşleşmiyor")]
        public string ConfirmPassword { get; set; }





    }
}
