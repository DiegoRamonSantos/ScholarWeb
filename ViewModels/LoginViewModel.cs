using System.ComponentModel.DataAnnotations;
using ScholarWeb.Models;

namespace ScholarWeb.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Informe o e-mail.")]
    [RegularExpression(ValidationPatterns.Email, ErrorMessage = "Informe um e-mail valido.")]
    [StringLength(80)]
    [Display(Name = "E-mail")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a senha.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Lembrar de mim")]
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}
