using System.ComponentModel.DataAnnotations;

namespace ScholarWeb.Models;

public class Matricula
{
    public int Id { get; set; }

    [Display(Name = "Aluno")]
    [Range(1, int.MaxValue, ErrorMessage = "Selecione um aluno.")]
    public int AlunoId { get; set; }

    public Aluno Aluno { get; set; } = null!;

    [Display(Name = "Curso")]
    [Range(1, int.MaxValue, ErrorMessage = "Selecione um curso.")]
    public int CursoId { get; set; }

    public Curso Curso { get; set; } = null!;

    [Display(Name = "Turma")]
    [Range(1, int.MaxValue, ErrorMessage = "Selecione uma turma.")]
    public int TurmaId { get; set; }

    public Turma Turma { get; set; } = null!;

    [Display(Name = "Periodo letivo")]
    [Range(1, int.MaxValue, ErrorMessage = "Selecione um periodo letivo.")]
    public int PeriodoLetivoId { get; set; }

    public PeriodoLetivo PeriodoLetivo { get; set; } = null!;

    [Required(ErrorMessage = "Informe a data de matricula.")]
    [DataType(DataType.Date)]
    [Display(Name = "Data de matricula")]
    public DateTime DataMatricula { get; set; } = DateTime.Today;

    public StatusMatricula Status { get; set; } = StatusMatricula.Ativa;

    [StringLength(80)]
    public string? Observacoes { get; set; }

    public ICollection<Nota> Notas { get; set; } = new List<Nota>();

    public ICollection<LancamentoFinanceiro> LancamentosFinanceiros { get; set; } = new List<LancamentoFinanceiro>();
}
