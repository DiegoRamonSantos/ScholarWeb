using System.ComponentModel.DataAnnotations;

namespace ScholarWeb.Models;

public class Disciplina
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

    [Display(Name = "Professor")]
    [Range(1, int.MaxValue, ErrorMessage = "Selecione um professor.")]
    public int ProfessorId { get; set; }

    public Professor Professor { get; set; } = null!;

    [Range(1, 1000, ErrorMessage = "Informe uma carga horaria valida.")]
    [Display(Name = "Carga horaria")]
    public int CargaHoraria { get; set; }

    [StringLength(80)]
    public string? Descricao { get; set; }

    public StatusRegistro Status { get; set; } = StatusRegistro.Ativo;

    public ICollection<Nota> Notas { get; set; } = new List<Nota>();
}
