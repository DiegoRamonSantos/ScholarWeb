using System.ComponentModel.DataAnnotations;

namespace ScholarWeb.Models;

public class Turma
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Informe o nome.")]
    [StringLength(80)]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o codigo.")]
    [StringLength(80)]
    public string Codigo { get; set; } = string.Empty;

    [Display(Name = "Curso")]
    [Range(1, int.MaxValue, ErrorMessage = "Selecione um curso.")]
    public int CursoId { get; set; }

    public Curso Curso { get; set; } = null!;

    [Display(Name = "Periodo letivo")]
    [Range(1, int.MaxValue, ErrorMessage = "Selecione um periodo letivo.")]
    public int PeriodoLetivoId { get; set; }

    public PeriodoLetivo PeriodoLetivo { get; set; } = null!;

    public Turno Turno { get; set; } = Turno.Matutino;

    [Range(1, 500, ErrorMessage = "Informe a capacidade maxima.")]
    [Display(Name = "Capacidade maxima")]
    public int CapacidadeMaxima { get; set; }

    public StatusTurma Status { get; set; } = StatusTurma.Ativa;

    public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();
}
