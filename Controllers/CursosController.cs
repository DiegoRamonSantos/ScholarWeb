using Microsoft.EntityFrameworkCore;
using ScholarWeb.Controllers.Admin;
using ScholarWeb.Data;
using ScholarWeb.Models;
using ScholarWeb.ViewModels.Admin;

namespace ScholarWeb.Controllers;

public class CursosController : AdminCrudController<Curso>
{
    public CursosController(AppDbContext context) : base(context)
    {
    }

    protected override DbSet<Curso> Entities => Context.Cursos;
    protected override string EntityName => "Curso";
    protected override string EntityPluralName => "Cursos";
    protected override string SearchPlaceholder => "Buscar por nome ou codigo";

    protected override IReadOnlyList<AdminColumnViewModel> Columns =>
    [
        new("Nome", nameof(Curso.Nome)),
        new("Codigo", nameof(Curso.Codigo)),
        new("Carga horaria", nameof(Curso.CargaHoraria)),
        new("Semestres", nameof(Curso.DuracaoSemestres)),
        new("Status", nameof(Curso.Status), AdminDisplayFormat.Status)
    ];

    protected override IReadOnlyList<AdminDisplayFieldViewModel> DisplayFields =>
    [
        new("Nome", nameof(Curso.Nome)),
        new("Codigo", nameof(Curso.Codigo)),
        new("Descricao", nameof(Curso.Descricao)),
        new("Carga horaria", nameof(Curso.CargaHoraria)),
        new("Duracao em semestres", nameof(Curso.DuracaoSemestres)),
        new("Status", nameof(Curso.Status), AdminDisplayFormat.Status)
    ];

    protected override IQueryable<Curso> ApplySearch(IQueryable<Curso> query, string search)
    {
        return query.Where(curso => curso.Nome.Contains(search) || curso.Codigo.Contains(search));
    }

    protected override Task<IReadOnlyList<AdminFormFieldViewModel>> BuildFormFieldsAsync(Curso entity)
    {
        IReadOnlyList<AdminFormFieldViewModel> fields =
        [
            Text(nameof(Curso.Nome), "Nome", true),
            Text(nameof(Curso.Codigo), "Codigo", true),
            TextArea(nameof(Curso.Descricao), "Descricao"),
            Number(nameof(Curso.CargaHoraria), "Carga horaria", true, "1", "10000"),
            Number(nameof(Curso.DuracaoSemestres), "Duracao em semestres", true, "1", "20"),
            Select(nameof(Curso.Status), "Status", EnumOptions<StatusRegistro>())
        ];

        return Task.FromResult(fields);
    }

    protected override Task PrepareForSaveAsync(Curso entity, bool isNew)
    {
        entity.Nome = NormalizeText(entity.Nome);
        entity.Codigo = NormalizeCode(entity.Codigo);
        entity.Descricao = entity.Descricao?.Trim();
        return Task.CompletedTask;
    }

    protected override async Task ValidateBusinessRulesAsync(Curso entity, bool isNew)
    {
        if (await Context.Cursos.AnyAsync(curso => curso.Codigo == entity.Codigo && curso.Id != entity.Id))
        {
            ModelState.AddModelError(nameof(Curso.Codigo), "Ja existe curso cadastrado com este codigo.");
        }
    }

    protected override async Task<bool> CanDeleteAsync(Curso entity)
    {
        var hasTurmas = await Context.Turmas.AnyAsync(turma => turma.CursoId == entity.Id);
        var hasDisciplinas = await Context.Disciplinas.AnyAsync(disciplina => disciplina.CursoId == entity.Id);
        var hasMatriculas = await Context.Matriculas.AnyAsync(matricula => matricula.CursoId == entity.Id);
        return !hasTurmas && !hasDisciplinas && !hasMatriculas;
    }
}
