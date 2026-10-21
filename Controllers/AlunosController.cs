using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScholarWeb.Controllers.Admin;
using ScholarWeb.Data;
using ScholarWeb.Models;
using ScholarWeb.ViewModels.Admin;

namespace ScholarWeb.Controllers;

public class AlunosController : AdminCrudController<Aluno>
{
    public AlunosController(AppDbContext context) : base(context)
    {
    }

    protected override DbSet<Aluno> Entities => Context.Alunos;
    protected override string EntityName => "Aluno";
    protected override string EntityPluralName => "Alunos";
    protected override string SearchPlaceholder => "Buscar por nome, CPF ou e-mail";

    protected override IReadOnlyList<AdminColumnViewModel> Columns =>
    [
        new("Nome", nameof(Aluno.NomeCompleto)),
        new("CPF", nameof(Aluno.CPF)),
        new("E-mail", nameof(Aluno.Email)),
        new("Cidade", nameof(Aluno.Cidade)),
        new("Status", nameof(Aluno.Status), AdminDisplayFormat.Status)
    ];

    protected override IReadOnlyList<AdminDisplayFieldViewModel> DisplayFields =>
    [
        new("Nome completo", nameof(Aluno.NomeCompleto)),
        new("CPF", nameof(Aluno.CPF)),
        new("Data de nascimento", nameof(Aluno.DataNascimento), AdminDisplayFormat.Date),
        new("E-mail", nameof(Aluno.Email)),
        new("Telefone", nameof(Aluno.Telefone)),
        new("Endereco", nameof(Aluno.Endereco)),
        new("Cidade", nameof(Aluno.Cidade)),
        new("Estado", nameof(Aluno.Estado)),
        new("Status", nameof(Aluno.Status), AdminDisplayFormat.Status),
        new("Data de cadastro", nameof(Aluno.DataCadastro), AdminDisplayFormat.DateTime)
    ];

    protected override IQueryable<Aluno> ApplySearch(IQueryable<Aluno> query, string search)
    {
        return query.Where(aluno =>
            aluno.NomeCompleto.Contains(search) ||
            aluno.CPF.Contains(search) ||
            aluno.Email.Contains(search));
    }

    protected override Task InitializeNewEntityAsync(Aluno entity)
    {
        entity.DataNascimento = DateTime.Today.AddYears(-18);
        return Task.CompletedTask;
    }

    protected override Task<IReadOnlyList<AdminFormFieldViewModel>> BuildFormFieldsAsync(Aluno entity)
    {
        IReadOnlyList<AdminFormFieldViewModel> fields =
        [
            Text(nameof(Aluno.NomeCompleto), "Nome completo", true),
            Text(nameof(Aluno.CPF), "CPF", true, 14),
            Date(nameof(Aluno.DataNascimento), "Data de nascimento", true),
            Email(nameof(Aluno.Email), "E-mail", true),
            Phone(nameof(Aluno.Telefone), "Telefone"),
            Text(nameof(Aluno.Endereco), "Endereco"),
            Text(nameof(Aluno.Cidade), "Cidade"),
            Text(nameof(Aluno.Estado), "Estado", maxLength: 2),
            Select(nameof(Aluno.Status), "Status", EnumOptions<StatusRegistro>())
        ];

        return Task.FromResult(fields);
    }

    protected override Task PrepareForSaveAsync(Aluno entity, bool isNew)
    {
        entity.NomeCompleto = NormalizeText(entity.NomeCompleto);
        entity.CPF = NormalizeText(entity.CPF);
        entity.Email = NormalizeEmail(entity.Email);
        entity.Estado = entity.Estado?.Trim().ToUpperInvariant();

        if (isNew && entity.DataCadastro == default)
        {
            entity.DataCadastro = DateTime.UtcNow;
        }

        return Task.CompletedTask;
    }

    protected override async Task ValidateBusinessRulesAsync(Aluno entity, bool isNew)
    {
        if (await Context.Alunos.AnyAsync(aluno => aluno.CPF == entity.CPF && aluno.Id != entity.Id))
        {
            ModelState.AddModelError(nameof(Aluno.CPF), "Ja existe aluno cadastrado com este CPF.");
        }

        if (await Context.Alunos.AnyAsync(aluno => aluno.Email == entity.Email && aluno.Id != entity.Id))
        {
            ModelState.AddModelError(nameof(Aluno.Email), "Ja existe aluno cadastrado com este e-mail.");
        }
    }

    protected override async Task<bool> CanDeleteAsync(Aluno entity)
    {
        var hasMatriculas = await Context.Matriculas.AnyAsync(matricula => matricula.AlunoId == entity.Id);
        var hasFinanceiro = await Context.LancamentosFinanceiros.AnyAsync(lancamento => lancamento.AlunoId == entity.Id);
        return !hasMatriculas && !hasFinanceiro;
    }
}
