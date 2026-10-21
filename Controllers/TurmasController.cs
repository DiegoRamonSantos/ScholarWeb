using Microsoft.EntityFrameworkCore;
using ScholarWeb.Controllers.Admin;
using ScholarWeb.Data;
using ScholarWeb.Models;
using ScholarWeb.ViewModels.Admin;

namespace ScholarWeb.Controllers;

public class TurmasController : AdminCrudController<Turma>
{
    public TurmasController(AppDbContext context) : base(context)
    {
    }

    protected override DbSet<Turma> Entities => Context.Turmas;
    protected override string EntityName => "Turma";
    protected override string EntityPluralName => "Turmas";
    protected override string SearchPlaceholder => "Buscar por nome, codigo, curso ou periodo";

    protected override IReadOnlyList<AdminColumnViewModel> Columns =>
    [
        new("Nome", nameof(Turma.Nome)),
        new("Codigo", nameof(Turma.Codigo)),
        new("Curso", "Curso.Nome"),
        new("Periodo", "PeriodoLetivo.Nome"),
        new("Turno", nameof(Turma.Turno)),
        new("Capacidade", nameof(Turma.CapacidadeMaxima)),
        new("Status", nameof(Turma.Status), AdminDisplayFormat.Status)
    ];

    protected override IReadOnlyList<AdminDisplayFieldViewModel> DisplayFields =>
    [
        new("Nome", nameof(Turma.Nome)),
        new("Codigo", nameof(Turma.Codigo)),
        new("Curso", "Curso.Nome"),
        new("Periodo letivo", "PeriodoLetivo.Nome"),
        new("Turno", nameof(Turma.Turno)),
        new("Capacidade maxima", nameof(Turma.CapacidadeMaxima)),
        new("Status", nameof(Turma.Status), AdminDisplayFormat.Status)
    ];

    protected override IQueryable<Turma> IncludeRelations(IQueryable<Turma> query)
    {
        return query
            .Include(turma => turma.Curso)
            .Include(turma => turma.PeriodoLetivo);
    }

    protected override IQueryable<Turma> ApplySearch(IQueryable<Turma> query, string search)
    {
        return query.Where(turma =>
            turma.Nome.Contains(search) ||
            turma.Codigo.Contains(search) ||
            turma.Curso.Nome.Contains(search) ||
            turma.PeriodoLetivo.Nome.Contains(search));
    }

    protected override async Task<IReadOnlyList<AdminFormFieldViewModel>> BuildFormFieldsAsync(Turma entity)
    {
        var cursos = await Context.Cursos.AsNoTracking()
            .Where(curso => curso.Status == StatusRegistro.Ativo || curso.Id == entity.CursoId)
            .OrderBy(curso => curso.Nome)
            .Select(curso => new { curso.Id, curso.Nome, curso.Codigo })
            .ToListAsync();

        var periodos = await Context.PeriodosLetivos.AsNoTracking()
            .Where(periodo => periodo.Status == StatusPeriodoLetivo.Aberto || periodo.Id == entity.PeriodoLetivoId)
            .OrderByDescending(periodo => periodo.Ano)
            .ThenByDescending(periodo => periodo.Semestre)
            .Select(periodo => new { periodo.Id, periodo.Nome, periodo.Ano, periodo.Semestre })
            .ToListAsync();

        return
        [
            Text(nameof(Turma.Nome), "Nome", true),
            Text(nameof(Turma.Codigo), "Codigo", true),
            Select(nameof(Turma.CursoId), "Curso", cursos.Select(curso => new AdminSelectOptionViewModel
            {
                Value = curso.Id.ToString(),
                Text = $"{curso.Nome} ({curso.Codigo})"
            }).ToList()),
            Select(nameof(Turma.PeriodoLetivoId), "Periodo letivo", periodos.Select(periodo => new AdminSelectOptionViewModel
            {
                Value = periodo.Id.ToString(),
                Text = $"{periodo.Nome} - {periodo.Ano}/{periodo.Semestre}"
            }).ToList()),
            Select(nameof(Turma.Turno), "Turno", EnumOptions<Turno>()),
            Number(nameof(Turma.CapacidadeMaxima), "Capacidade maxima", true, "1", "500"),
            Select(nameof(Turma.Status), "Status", EnumOptions<StatusTurma>())
        ];
    }

    protected override Task PrepareForSaveAsync(Turma entity, bool isNew)
    {
        entity.Nome = NormalizeText(entity.Nome);
        entity.Codigo = NormalizeCode(entity.Codigo);
        return Task.CompletedTask;
    }

    protected override async Task ValidateBusinessRulesAsync(Turma entity, bool isNew)
    {
        if (await Context.Turmas.AnyAsync(turma => turma.Codigo == entity.Codigo && turma.Id != entity.Id))
        {
            ModelState.AddModelError(nameof(Turma.Codigo), "Ja existe turma cadastrada com este codigo.");
        }

        var cursoAtivo = await Context.Cursos.AnyAsync(curso => curso.Id == entity.CursoId && curso.Status == StatusRegistro.Ativo);
        if (!cursoAtivo)
        {
            ModelState.AddModelError(nameof(Turma.CursoId), "Selecione um curso ativo.");
        }

        var periodoAberto = await Context.PeriodosLetivos.AnyAsync(periodo => periodo.Id == entity.PeriodoLetivoId && periodo.Status == StatusPeriodoLetivo.Aberto);
        if (!periodoAberto)
        {
            ModelState.AddModelError(nameof(Turma.PeriodoLetivoId), "Selecione um periodo letivo aberto.");
        }
    }

    protected override async Task<bool> CanDeleteAsync(Turma entity)
    {
        return !await Context.Matriculas.AnyAsync(matricula => matricula.TurmaId == entity.Id);
    }

    protected override void ApplyInactivation(Turma entity)
    {
        entity.Status = StatusTurma.Cancelada;
    }
}
