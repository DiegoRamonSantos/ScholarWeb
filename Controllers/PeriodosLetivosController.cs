using Microsoft.EntityFrameworkCore;
using ScholarWeb.Controllers.Admin;
using ScholarWeb.Data;
using ScholarWeb.Models;
using ScholarWeb.ViewModels.Admin;

namespace ScholarWeb.Controllers;

public class PeriodosLetivosController : AdminCrudController<PeriodoLetivo>
{
    public PeriodosLetivosController(AppDbContext context) : base(context)
    {
    }

    protected override DbSet<PeriodoLetivo> Entities => Context.PeriodosLetivos;
    protected override string EntityName => "Periodo letivo";
    protected override string EntityPluralName => "Periodos letivos";
    protected override string SearchPlaceholder => "Buscar por nome ou ano";

    protected override IReadOnlyList<AdminColumnViewModel> Columns =>
    [
        new("Nome", nameof(PeriodoLetivo.Nome)),
        new("Ano", nameof(PeriodoLetivo.Ano)),
        new("Semestre", nameof(PeriodoLetivo.Semestre)),
        new("Inicio", nameof(PeriodoLetivo.DataInicio), AdminDisplayFormat.Date),
        new("Fim", nameof(PeriodoLetivo.DataFim), AdminDisplayFormat.Date),
        new("Status", nameof(PeriodoLetivo.Status), AdminDisplayFormat.Status)
    ];

    protected override IReadOnlyList<AdminDisplayFieldViewModel> DisplayFields =>
    [
        new("Nome", nameof(PeriodoLetivo.Nome)),
        new("Ano", nameof(PeriodoLetivo.Ano)),
        new("Semestre", nameof(PeriodoLetivo.Semestre)),
        new("Data de inicio", nameof(PeriodoLetivo.DataInicio), AdminDisplayFormat.Date),
        new("Data de fim", nameof(PeriodoLetivo.DataFim), AdminDisplayFormat.Date),
        new("Status", nameof(PeriodoLetivo.Status), AdminDisplayFormat.Status)
    ];

    protected override IQueryable<PeriodoLetivo> ApplySearch(IQueryable<PeriodoLetivo> query, string search)
    {
        var hasYear = int.TryParse(search, out var year);
        return query.Where(periodo => periodo.Nome.Contains(search) || (hasYear && periodo.Ano == year));
    }

    protected override Task<IReadOnlyList<AdminFormFieldViewModel>> BuildFormFieldsAsync(PeriodoLetivo entity)
    {
        IReadOnlyList<AdminFormFieldViewModel> fields =
        [
            Text(nameof(PeriodoLetivo.Nome), "Nome", true),
            Number(nameof(PeriodoLetivo.Ano), "Ano", true, "2000", "2100"),
            Number(nameof(PeriodoLetivo.Semestre), "Semestre", true, "1", "2"),
            Date(nameof(PeriodoLetivo.DataInicio), "Data de inicio", true),
            Date(nameof(PeriodoLetivo.DataFim), "Data de fim", true),
            Select(nameof(PeriodoLetivo.Status), "Status", EnumOptions<StatusPeriodoLetivo>())
        ];

        return Task.FromResult(fields);
    }

    protected override Task PrepareForSaveAsync(PeriodoLetivo entity, bool isNew)
    {
        entity.Nome = NormalizeText(entity.Nome);
        return Task.CompletedTask;
    }

    protected override Task ValidateBusinessRulesAsync(PeriodoLetivo entity, bool isNew)
    {
        if (entity.DataFim.Date < entity.DataInicio.Date)
        {
            ModelState.AddModelError(nameof(PeriodoLetivo.DataFim), "A data de fim deve ser maior ou igual a data de inicio.");
        }

        return Task.CompletedTask;
    }

    protected override async Task<bool> CanDeleteAsync(PeriodoLetivo entity)
    {
        var hasTurmas = await Context.Turmas.AnyAsync(turma => turma.PeriodoLetivoId == entity.Id);
        var hasMatriculas = await Context.Matriculas.AnyAsync(matricula => matricula.PeriodoLetivoId == entity.Id);
        return !hasTurmas && !hasMatriculas;
    }

    protected override void ApplyInactivation(PeriodoLetivo entity)
    {
        entity.Status = StatusPeriodoLetivo.Encerrado;
    }
}