using Microsoft.EntityFrameworkCore;
using ScholarWeb.Controllers.Admin;
using ScholarWeb.Data;
using ScholarWeb.Models;
using ScholarWeb.ViewModels.Admin;

namespace ScholarWeb.Controllers;

public class NotasController : AdminCrudController<Nota>
{
    public NotasController(AppDbContext context) : base(context)
    {
    }

    protected override DbSet<Nota> Entities => Context.Notas;
    protected override string EntityName => "Nota";
    protected override string EntityPluralName => "Notas";
    protected override string SearchPlaceholder => "Buscar por aluno, turma ou disciplina";

    protected override IReadOnlyList<AdminColumnViewModel> Columns =>
    [
        new("Aluno", "Matricula.Aluno.NomeCompleto"),
        new("Turma", "Matricula.Turma.Nome"),
        new("Disciplina", "Disciplina.Nome"),
        new("Media", nameof(Nota.MediaFinal), AdminDisplayFormat.Decimal),
        new("Situacao", nameof(Nota.Situacao), AdminDisplayFormat.Status)
    ];

    protected override IReadOnlyList<AdminDisplayFieldViewModel> DisplayFields =>
    [
        new("Aluno", "Matricula.Aluno.NomeCompleto"),
        new("Turma", "Matricula.Turma.Nome"),
        new("Disciplina", "Disciplina.Nome"),
        new("Nota 1", nameof(Nota.Nota1), AdminDisplayFormat.Decimal),
        new("Nota 2", nameof(Nota.Nota2), AdminDisplayFormat.Decimal),
        new("Nota 3", nameof(Nota.Nota3), AdminDisplayFormat.Decimal),
        new("Media final", nameof(Nota.MediaFinal), AdminDisplayFormat.Decimal),
        new("Situacao", nameof(Nota.Situacao), AdminDisplayFormat.Status),
        new("Observacoes", nameof(Nota.Observacoes))
    ];

    protected override IQueryable<Nota> IncludeRelations(IQueryable<Nota> query)
    {
        return query
            .Include(nota => nota.Matricula)
                .ThenInclude(matricula => matricula.Aluno)
            .Include(nota => nota.Matricula)
                .ThenInclude(matricula => matricula.Turma)
            .Include(nota => nota.Disciplina);
    }

    protected override IQueryable<Nota> ApplySearch(IQueryable<Nota> query, string search)
    {
        return query.Where(nota =>
            nota.Matricula.Aluno.NomeCompleto.Contains(search) ||
            nota.Matricula.Turma.Nome.Contains(search) ||
            nota.Disciplina.Nome.Contains(search));
    }

    protected override async Task<IReadOnlyList<AdminFormFieldViewModel>> BuildFormFieldsAsync(Nota entity)
    {
        var matriculas = await Context.Matriculas.AsNoTracking()
            .Include(matricula => matricula.Aluno)
            .Include(matricula => matricula.Turma)
            .Where(matricula => matricula.Status == StatusMatricula.Ativa || matricula.Id == entity.MatriculaId)
            .OrderBy(matricula => matricula.Aluno.NomeCompleto)
            .Select(matricula => new
            {
                matricula.Id,
                AlunoNome = matricula.Aluno.NomeCompleto,
                TurmaNome = matricula.Turma.Nome
            })
            .ToListAsync();

        var disciplinas = await Context.Disciplinas.AsNoTracking()
            .Where(disciplina => disciplina.Status == StatusRegistro.Ativo || disciplina.Id == entity.DisciplinaId)
            .OrderBy(disciplina => disciplina.Nome)
            .Select(disciplina => new { disciplina.Id, disciplina.Nome, disciplina.Codigo })
            .ToListAsync();

        return
        [
            Select(nameof(Nota.MatriculaId), "Matricula", matriculas.Select(matricula => new AdminSelectOptionViewModel
            {
                Value = matricula.Id.ToString(),
                Text = $"{matricula.AlunoNome} - {matricula.TurmaNome}"
            }).ToList()),
            Select(nameof(Nota.DisciplinaId), "Disciplina", disciplinas.Select(disciplina => new AdminSelectOptionViewModel
            {
                Value = disciplina.Id.ToString(),
                Text = $"{disciplina.Nome} ({disciplina.Codigo})"
            }).ToList()),
            Grade(nameof(Nota.Nota1), "Nota 1"),
            Grade(nameof(Nota.Nota2), "Nota 2"),
            Grade(nameof(Nota.Nota3), "Nota 3"),
            TextArea(nameof(Nota.Observacoes), "Observacoes")
        ];
    }

    protected override Task PrepareForSaveAsync(Nota entity, bool isNew)
    {
        entity.Observacoes = entity.Observacoes?.Trim();
        return Task.CompletedTask;
    }

    protected override async Task ValidateBusinessRulesAsync(Nota entity, bool isNew)
    {
        var duplicate = await Context.Notas.AnyAsync(nota =>
            nota.MatriculaId == entity.MatriculaId &&
            nota.DisciplinaId == entity.DisciplinaId &&
            nota.Id != entity.Id);

        if (duplicate)
        {
            ModelState.AddModelError(string.Empty, "Ja existe nota para esta matricula e disciplina.");
        }

        var matricula = await Context.Matriculas.AsNoTracking().FirstOrDefaultAsync(item => item.Id == entity.MatriculaId);
        var disciplina = await Context.Disciplinas.AsNoTracking().FirstOrDefaultAsync(item => item.Id == entity.DisciplinaId);

        if (matricula is null)
        {
            ModelState.AddModelError(nameof(Nota.MatriculaId), "Selecione uma matricula valida.");
            return;
        }

        if (matricula.Status != StatusMatricula.Ativa)
        {
            ModelState.AddModelError(nameof(Nota.MatriculaId), "A matricula deve estar ativa para lancar notas.");
        }

        if (disciplina is null || disciplina.Status != StatusRegistro.Ativo)
        {
            ModelState.AddModelError(nameof(Nota.DisciplinaId), "Selecione uma disciplina ativa.");
            return;
        }

        if (disciplina.CursoId != matricula.CursoId)
        {
            ModelState.AddModelError(nameof(Nota.DisciplinaId), "A disciplina deve pertencer ao curso da matricula.");
        }
    }
}