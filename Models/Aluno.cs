using System.ComponentModel.DataAnnotations;

namespace ScholarWeb.Models;

public class Aluno
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

    [Required(ErrorMessage = "Informe a data de nascimento.")]
    [DataType(DataType.Date)]
    [Display(Name = "Data de nascimento")]
    public DateTime DataNascimento { get; set; }

    [Required(ErrorMessage = "Informe o e-mail.")]
    [RegularExpression(ValidationPatterns.Email, ErrorMessage = "Informe um e-mail valido.")]
    [StringLength(80)]
    [Display(Name = "E-mail")]
    public string Email { get; set; } = string.Empty;

    [RegularExpression(ValidationPatterns.Phone, ErrorMessage = "Informe um telefone valido. Use (XX) XXXXX-XXXX ou 11 digitos.")]
    [StringLength(15)]
    public string? Telefone { get; set; }

    [StringLength(80)]
    [Display(Name = "Endereco")]
    public string? Endereco { get; set; }

    [StringLength(80)]
    public string? Cidade { get; set; }

    [StringLength(2)]
    public string? Estado { get; set; }

    public StatusRegistro Status { get; set; } = StatusRegistro.Ativo;

    [Display(Name = "Data de cadastro")]
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

    public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();

    public ICollection<LancamentoFinanceiro> LancamentosFinanceiros { get; set; } = new List<LancamentoFinanceiro>();
}
