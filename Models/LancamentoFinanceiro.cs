using System.ComponentModel.DataAnnotations;

namespace ScholarWeb.Models;

public class LancamentoFinanceiro
{
    public int Id { get; set; }

    [Display(Name = "Aluno")]
    [Range(1, int.MaxValue, ErrorMessage = "Selecione um aluno.")]
    public int AlunoId { get; set; }

    public Aluno Aluno { get; set; } = null!;

    [Display(Name = "Matricula")]
    [Range(1, int.MaxValue, ErrorMessage = "Selecione uma matricula.")]
    public int MatriculaId { get; set; }

    public Matricula Matricula { get; set; } = null!;

    [Required(ErrorMessage = "Informe a descricao.")]
    [StringLength(80)]
    public string Descricao { get; set; } = string.Empty;

    public TipoLancamento Tipo { get; set; } = TipoLancamento.Mensalidade;

    [Range(0.01, 9999999, ErrorMessage = "O valor deve ser maior que zero.")]
    public decimal Valor { get; set; }

    [Required(ErrorMessage = "Informe a data de vencimento.")]
    [DataType(DataType.Date)]
    [Display(Name = "Data de vencimento")]
    public DateTime DataVencimento { get; set; } = DateTime.Today;

    [DataType(DataType.Date)]
    [Display(Name = "Data de pagamento")]
    public DateTime? DataPagamento { get; set; }

    public StatusFinanceiro Status { get; set; } = StatusFinanceiro.Pendente;

    [StringLength(60)]
    [Display(Name = "Forma de pagamento")]
    public string? FormaPagamento { get; set; }

    [StringLength(80)]
    public string? Observacoes { get; set; }
}
