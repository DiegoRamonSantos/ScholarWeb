using Microsoft.EntityFrameworkCore;
using ScholarWeb.Controllers.Admin;
using ScholarWeb.Data;
using ScholarWeb.Models;
using ScholarWeb.ViewModels.Admin;

namespace ScholarWeb.Controllers;

public class DisciplinasController : AdminCrudController<Disciplina>
{
    public DisciplinasController(AppDbContext context) : base(context)
    {
    }

    protected override DbSet<Disciplina> Entities => Context.Disciplinas;
    protected override string EntityName => "Disciplina";
    protected override string EntityPluralName => "Disciplinas";
    protected override string SearchPlaceholder => "Buscar por nome, codigo, curso ou professor";

    protected override IReadOnlyList<AdminColumnViewModel> Columns =>
    [
        new("Nome", nameof(Disciplina.Nome)),
        new("Codigo", nameof(Disciplina.Codigo)),
        new("Curso", "Curso.Nome"),
        new("Professor", "Professor.NomeCompleto"),
        new("Carga horaria", nameof(Disciplina.CargaHoraria)),
        new("Status", nameof(Disciplina.Status), AdminDisplayFormat.Status)
    ];

    protected override IReadOnlyList<AdminDisplayFieldViewModel> DisplayFields =>
    [
        new("Nome", nameof(Disciplina.Nome)),
        new("Codigo", nameof(Disciplina.Codigo)),
        new("Curso", "Curso.Nome"),
        new("Professor", "Professor.NomeCompleto"),
        new("Carga horaria", nameof(Disciplina.CargaHoraria)),
        new("Descricao", nameof(Disciplina.Descricao)),
        new("Status", nameof(Disciplina.Status), AdminDisplayFormat.Status)
    ];

    protected override IQueryable<Disciplina> IncludeRelations(IQueryable<Disciplina> query)
    {
        return query
            .Include(disciplina => disciplina.Curso)
            .Include(disciplina => disciplina.Professor);
    }

    protected override IQueryable<Disciplina> ApplySearch(IQueryable<Disciplina> query, string search)
    {
        return query.Where(disciplina =>
            disciplina.Nome.Contains(search) ||
            disciplina.Codigo.Contains(search) ||
            disciplina.Curso.Nome.Contains(search) ||
            disciplina.Professor.NomeCompleto.Contains(search));
    }

    protected override async Task<IReadOnlyList<AdminFormFieldViewModel>> BuildFormFieldsAsync(Disciplina entity)
    {
        var cursos = await Context.Cursos.AsNoTracking()
            .Where(curso => curso.Status == StatusRegistro.Ativo || curso.Id == entity.CursoId)
            .OrderBy(curso => curso.Nome)
            .Select(curso => new { curso.Id, curso.Nome, curso.Codigo })
            .ToListAsync();

        var professores = await Context.Professores.AsNoTracking()
            .Where(professor => professor.Status == StatusRegistro.Ativo || professor.Id == entity.ProfessorId)
            .OrderBy(professor => professor.NomeCompleto)
            .Select(professor => new { professor.Id, professor.NomeCompleto })
            .ToListAsync();

        return
        [
            Text(nameof(Disciplina.Nome), "Nome", true),
            Text(nameof(Disciplina.Codigo), "Codigo", true),
            Select(nameof(Disciplina.CursoId), "Curso", cursos.Select(curso => new AdminSelectOptionViewModel
            {
                Value = curso.Id.ToString(),
                Text = $"{curso.Nome} ({curso.Codigo})"
            }).ToList()),
            Select(nameof(Disciplina.ProfessorId), "Professor", professores.Select(professor => new AdminSelectOptionViewModel
            {
                Value = professor.Id.ToString(),
                Text = professor.NomeCompleto
            }).ToList()),
            Number(nameof(Disciplina.CargaHoraria), "Carga horaria", true, "1", "1000"),
            TextArea(nameof(Disciplina.Descricao), "Descricao"),
            Select(nameof(Disciplina.Status), "Status", EnumOptions<StatusRegistro>())
        ];
    }

    protected override Task PrepareForSaveAsync(Disciplina entity, bool isNew)
    {
        entity.Nome = NormalizeText(entity.Nome);
        entity.Codigo = NormalizeCode(entity.Codigo);
        entity.Descricao = entity.Descricao?.Trim();
        return Task.CompletedTask;
    }

    protected override async Task ValidateBusinessRulesAsync(Disciplina entity, bool isNew)
    {
        if (await Context.Disciplinas.AnyAsync(disciplina => disciplina.Codigo == entity.Codigo && disciplina.Id != entity.Id))
        {
            ModelState.AddModelError(nameof(Disciplina.Codigo), "Ja existe disciplina cadastrada com este codigo.");
        }

        if (!await Context.Cursos.AnyAsync(curso => curso.Id == entity.CursoId && curso.Status == StatusRegistro.Ativo))
        {
            ModelState.AddModelError(nameof(Disciplina.CursoId), "Selecione um curso ativo.");
        }

        if (!await Context.Professores.AnyAsync(professor => professor.Id == entity.ProfessorId && professor.Status == StatusRegistro.Ativo))
        {
            ModelState.AddModelError(nameof(Disciplina.ProfessorId), "Selecione um professor ativo.");
        }
    }

    protected override async Task<bool> CanDeleteAsync(Disciplina entity)
    {
        return !await Context.Notas.AnyAsync(nota => nota.DisciplinaId == entity.Id);
    }
}
