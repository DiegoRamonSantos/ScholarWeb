using System.ComponentModel.DataAnnotations;

namespace ScholarWeb.Models;

public class Curso
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Informe o nome.")]
    [StringLength(80)]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o codigo.")]
    [StringLength(80)]
    public string Codigo { get; set; } = string.Empty;

    [StringLength(80)]
    public string? Descricao { get; set; }

    [Range(1, 10000, ErrorMessage = "Informe uma carga horaria valida.")]
    [Display(Name = "Carga horaria")]
    public int CargaHoraria { get; set; }

    [Range(1, 20, ErrorMessage = "Informe a duracao em semestres.")]
    [Display(Name = "Duracao em semestres")]
    public int DuracaoSemestres { get; set; }

    public StatusRegistro Status { get; set; } = StatusRegistro.Ativo;

    public ICollection<Turma> Turmas { get; set; } = new List<Turma>();

    public ICollection<Disciplina> Disciplinas { get; set; } = new List<Disciplina>();

    public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();
}
