using System.ComponentModel.DataAnnotations;

namespace ScholarWeb.Models;

public class Professor
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Informe o nome completo.")]
    [StringLength(80)]
    [Display(Name = "Nome completo")]
    public string NomeCompleto { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o CPF.")]
    [StringLength(14)]
    [Display(Name = "CPF")]
    public string CPF { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o e-mail.")]
    [RegularExpression(ValidationPatterns.Email, ErrorMessage = "Informe um e-mail valido.")]
    [StringLength(80)]
    [Display(Name = "E-mail")]
    public string Email { get; set; } = string.Empty;

    [RegularExpression(ValidationPatterns.Phone, ErrorMessage = "Informe um telefone valido. Use (XX) XXXXX-XXXX ou 11 digitos.")]
    [StringLength(15)]
    public string? Telefone { get; set; }

    [Required(ErrorMessage = "Informe a formacao.")]
    [StringLength(80)]
    [Display(Name = "Formacao")]
    public string Formacao { get; set; } = string.Empty;

    [StringLength(80)]
    public string? Especialidade { get; set; }

    public StatusRegistro Status { get; set; } = StatusRegistro.Ativo;

    [Display(Name = "Data de cadastro")]
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

    public ICollection<Disciplina> Disciplinas { get; set; } = new List<Disciplina>();
}
