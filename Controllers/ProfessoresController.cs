using Microsoft.EntityFrameworkCore;
using ScholarWeb.Controllers.Admin;
using ScholarWeb.Data;
using ScholarWeb.Models;
using ScholarWeb.ViewModels.Admin;

namespace ScholarWeb.Controllers;

public class ProfessoresController : AdminCrudController<Professor>
{
    public ProfessoresController(AppDbContext context) : base(context)
    {
    }

    protected override DbSet<Professor> Entities => Context.Professores;
    protected override string EntityName => "Professor";
    protected override string EntityPluralName => "Professores";
    protected override string SearchPlaceholder => "Buscar por nome, CPF, e-mail ou especialidade";

    protected override IReadOnlyList<AdminColumnViewModel> Columns =>
    [
        new("Nome", nameof(Professor.NomeCompleto)),
        new("CPF", nameof(Professor.CPF)),
        new("E-mail", nameof(Professor.Email)),
        new("Especialidade", nameof(Professor.Especialidade)),
        new("Status", nameof(Professor.Status), AdminDisplayFormat.Status)
    ];

    protected override IReadOnlyList<AdminDisplayFieldViewModel> DisplayFields =>
    [
        new("Nome completo", nameof(Professor.NomeCompleto)),
        new("CPF", nameof(Professor.CPF)),
        new("E-mail", nameof(Professor.Email)),
        new("Telefone", nameof(Professor.Telefone)),
        new("Formacao", nameof(Professor.Formacao)),
        new("Especialidade", nameof(Professor.Especialidade)),
        new("Status", nameof(Professor.Status), AdminDisplayFormat.Status),
        new("Data de cadastro", nameof(Professor.DataCadastro), AdminDisplayFormat.DateTime)
    ];

    protected override IQueryable<Professor> ApplySearch(IQueryable<Professor> query, string search)
    {
        return query.Where(professor =>
            professor.NomeCompleto.Contains(search) ||
            professor.CPF.Contains(search) ||
            professor.Email.Contains(search) ||
            (professor.Especialidade != null && professor.Especialidade.Contains(search)));
    }

    protected override Task<IReadOnlyList<AdminFormFieldViewModel>> BuildFormFieldsAsync(Professor entity)
    {
        IReadOnlyList<AdminFormFieldViewModel> fields =
        [
            Text(nameof(Professor.NomeCompleto), "Nome completo", true),
            Text(nameof(Professor.CPF), "CPF", true),
            Email(nameof(Professor.Email), "E-mail", true),
            Phone(nameof(Professor.Telefone), "Telefone"),
            Text(nameof(Professor.Formacao), "Formacao", true),
            Text(nameof(Professor.Especialidade), "Especialidade"),
            Select(nameof(Professor.Status), "Status", EnumOptions<StatusRegistro>())
        ];

        return Task.FromResult(fields);
    }

    protected override Task PrepareForSaveAsync(Professor entity, bool isNew)
    {
        entity.NomeCompleto = NormalizeText(entity.NomeCompleto);
        entity.CPF = NormalizeText(entity.CPF);
        entity.Email = NormalizeEmail(entity.Email);
        entity.Formacao = NormalizeText(entity.Formacao);
        entity.Especialidade = entity.Especialidade?.Trim();

        if (isNew && entity.DataCadastro == default)
        {
            entity.DataCadastro = DateTime.UtcNow;
        }

        return Task.CompletedTask;
    }

    protected override async Task ValidateBusinessRulesAsync(Professor entity, bool isNew)
    {
        if (await Context.Professores.AnyAsync(professor => professor.CPF == entity.CPF && professor.Id != entity.Id))
        {
            ModelState.AddModelError(nameof(Professor.CPF), "Ja existe professor cadastrado com este CPF.");
        }

        if (await Context.Professores.AnyAsync(professor => professor.Email == entity.Email && professor.Id != entity.Id))
        {
            ModelState.AddModelError(nameof(Professor.Email), "Ja existe professor cadastrado com este e-mail.");
        }
    }

    protected override async Task<bool> CanDeleteAsync(Professor entity)
    {
        return !await Context.Disciplinas.AnyAsync(disciplina => disciplina.ProfessorId == entity.Id);
    }
}
