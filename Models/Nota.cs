using System.ComponentModel.DataAnnotations;

namespace ScholarWeb.Models;

public class Nota
{
    public int Id { get; set; }

    [Display(Name = "Matricula")]
    [Range(1, int.MaxValue, ErrorMessage = "Selecione uma matricula.")]
    public int MatriculaId { get; set; }

    public Matricula Matricula { get; set; } = null!;

    [Display(Name = "Disciplina")]
    [Range(1, int.MaxValue, ErrorMessage = "Selecione uma disciplina.")]
    public int DisciplinaId { get; set; }

    public Disciplina Disciplina { get; set; } = null!;

    [Range(0, 10, ErrorMessage = "A nota deve estar entre 0 e 10.")]
    public decimal Nota1 { get; set; }

    [Range(0, 10, ErrorMessage = "A nota deve estar entre 0 e 10.")]
    public decimal Nota2 { get; set; }

    [Range(0, 10, ErrorMessage = "A nota deve estar entre 0 e 10.")]
    public decimal Nota3 { get; set; }

    [Display(Name = "Media final")]
    public decimal MediaFinal { get; set; }

    public SituacaoNota Situacao { get; set; } = SituacaoNota.Reprovado;

    [StringLength(80)]
    public string? Observacoes { get; set; }
}
