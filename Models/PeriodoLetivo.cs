using System.ComponentModel.DataAnnotations;

namespace ScholarWeb.Models;

public class PeriodoLetivo
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Informe o nome.")]
    [StringLength(80)]
    public string Nome { get; set; } = string.Empty;

    [Range(2000, 2100, ErrorMessage = "Informe um ano valido.")]
    public int Ano { get; set; }

    [Range(1, 2, ErrorMessage = "Informe 1 ou 2.")]
    public int Semestre { get; set; }

    [Required(ErrorMessage = "Informe a data de inicio.")]
    [DataType(DataType.Date)]
    [Display(Name = "Data de inicio")]
    public DateTime DataInicio { get; set; }

    [Required(ErrorMessage = "Informe a data de fim.")]
    [DataType(DataType.Date)]
    [Display(Name = "Data de fim")]
    public DateTime DataFim { get; set; }

    public StatusPeriodoLetivo Status { get; set; } = StatusPeriodoLetivo.Aberto;

    public ICollection<Turma> Turmas { get; set; } = new List<Turma>();

    public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();
}
