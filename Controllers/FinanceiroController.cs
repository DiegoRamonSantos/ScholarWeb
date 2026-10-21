using Microsoft.EntityFrameworkCore;
using ScholarWeb.Controllers.Admin;
using ScholarWeb.Data;
using ScholarWeb.Models;
using ScholarWeb.ViewModels.Admin;

namespace ScholarWeb.Controllers;

public class FinanceiroController : AdminCrudController<LancamentoFinanceiro>
{
    public FinanceiroController(AppDbContext context) : base(context)
    {
    }

    protected override DbSet<LancamentoFinanceiro> Entities => Context.LancamentosFinanceiros;
    protected override string EntityName => "Lancamento financeiro";
    protected override string EntityPluralName => "Financeiro";
    protected override string SearchPlaceholder => "Buscar por aluno, descricao ou status";

    protected override IReadOnlyList<AdminColumnViewModel> Columns =>
    [
        new("Aluno", "Aluno.NomeCompleto"),
        new("Descricao", nameof(LancamentoFinanceiro.Descricao)),
        new("Tipo", nameof(LancamentoFinanceiro.Tipo)),
        new("Valor", nameof(LancamentoFinanceiro.Valor), AdminDisplayFormat.Currency),
        new("Vencimento", nameof(LancamentoFinanceiro.DataVencimento), AdminDisplayFormat.Date),
        new("Status", nameof(LancamentoFinanceiro.Status), AdminDisplayFormat.Status)
    ];

    protected override IReadOnlyList<AdminDisplayFieldViewModel> DisplayFields =>
    [
        new("Aluno", "Aluno.NomeCompleto"),
        new("Matricula", "Matricula.Turma.Nome"),
        new("Descricao", nameof(LancamentoFinanceiro.Descricao)),
        new("Tipo", nameof(LancamentoFinanceiro.Tipo)),
        new("Valor", nameof(LancamentoFinanceiro.Valor), AdminDisplayFormat.Currency),
        new("Data de vencimento", nameof(LancamentoFinanceiro.DataVencimento), AdminDisplayFormat.Date),
        new("Data de pagamento", nameof(LancamentoFinanceiro.DataPagamento), AdminDisplayFormat.Date),
        new("Status", nameof(LancamentoFinanceiro.Status), AdminDisplayFormat.Status),
        new("Forma de pagamento", nameof(LancamentoFinanceiro.FormaPagamento)),
        new("Observacoes", nameof(LancamentoFinanceiro.Observacoes))
    ];

    protected override async Task BeforeIndexAsync()
    {
        var today = DateTime.Today;
        var atrasados = await Context.LancamentosFinanceiros
            .Where(lancamento =>
                lancamento.Status == StatusFinanceiro.Pendente &&
                lancamento.DataPagamento == null &&
                lancamento.DataVencimento < today)
            .ToListAsync();

        if (atrasados.Count == 0)
        {
            return;
        }

        foreach (var lancamento in atrasados)
        {
            lancamento.Status = StatusFinanceiro.Atrasado;
        }

        await Context.SaveChangesAsync();
    }

    protected override IQueryable<LancamentoFinanceiro> IncludeRelations(IQueryable<LancamentoFinanceiro> query)
    {
        return query
            .Include(lancamento => lancamento.Aluno)
            .Include(lancamento => lancamento.Matricula)
                .ThenInclude(matricula => matricula.Turma);
    }

    protected override IQueryable<LancamentoFinanceiro> ApplySearch(IQueryable<LancamentoFinanceiro> query, string search)
    {
        var hasStatus = Enum.TryParse<StatusFinanceiro>(search, true, out var status);

        return query.Where(lancamento =>
            lancamento.Aluno.NomeCompleto.Contains(search) ||
            lancamento.Descricao.Contains(search) ||
            (hasStatus && lancamento.Status == status));
    }

    protected override async Task<IReadOnlyList<AdminFormFieldViewModel>> BuildFormFieldsAsync(LancamentoFinanceiro entity)
    {
        var alunos = await Context.Alunos.AsNoTracking()
            .Where(aluno => aluno.Status == StatusRegistro.Ativo || aluno.Id == entity.AlunoId)
            .OrderBy(aluno => aluno.NomeCompleto)
            .Select(aluno => new { aluno.Id, aluno.NomeCompleto, aluno.CPF })
            .ToListAsync();

        var matriculas = await Context.Matriculas.AsNoTracking()
            .Include(matricula => matricula.Aluno)
            .Include(matricula => matricula.Turma)
            .Where(matricula => matricula.Status == StatusMatricula.Ativa || matricula.Id == entity.MatriculaId)
            .OrderBy(matricula => matricula.Aluno.NomeCompleto)
            .Select(matricula => new
            {
                matricula.Id,
                matricula.AlunoId,
                AlunoNome = matricula.Aluno.NomeCompleto,
                TurmaNome = matricula.Turma.Nome
            })
            .ToListAsync();

        return
        [
            Select(nameof(LancamentoFinanceiro.AlunoId), "Aluno", alunos.Select(aluno => new AdminSelectOptionViewModel
            {
                Value = aluno.Id.ToString(),
                Text = $"{aluno.NomeCompleto} - {aluno.CPF}"
            }).ToList()),
            Select(nameof(LancamentoFinanceiro.MatriculaId), "Matricula", matriculas.Select(matricula => new AdminSelectOptionViewModel
            {
                Value = matricula.Id.ToString(),
                Text = $"{matricula.AlunoNome} - {matricula.TurmaNome}"
            }).ToList()),
            Text(nameof(LancamentoFinanceiro.Descricao), "Descricao", true),
            Select(nameof(LancamentoFinanceiro.Tipo), "Tipo", EnumOptions<TipoLancamento>()),
            Money(nameof(LancamentoFinanceiro.Valor), "Valor", true),
            Date(nameof(LancamentoFinanceiro.DataVencimento), "Data de vencimento", true),
            Date(nameof(LancamentoFinanceiro.DataPagamento), "Data de pagamento"),
            Select(nameof(LancamentoFinanceiro.Status), "Status", EnumOptions<StatusFinanceiro>()),
            Text(nameof(LancamentoFinanceiro.FormaPagamento), "Forma de pagamento"),
            TextArea(nameof(LancamentoFinanceiro.Observacoes), "Observacoes")
        ];
    }

    protected override Task PrepareForSaveAsync(LancamentoFinanceiro entity, bool isNew)
    {
        entity.Descricao = NormalizeText(entity.Descricao);
        entity.FormaPagamento = entity.FormaPagamento?.Trim();
        entity.Observacoes = entity.Observacoes?.Trim();
        return Task.CompletedTask;
    }

    protected override async Task ValidateBusinessRulesAsync(LancamentoFinanceiro entity, bool isNew)
    {
        var alunoAtivo = await Context.Alunos.AnyAsync(aluno => aluno.Id == entity.AlunoId && aluno.Status == StatusRegistro.Ativo);
        if (!alunoAtivo)
        {
            ModelState.AddModelError(nameof(LancamentoFinanceiro.AlunoId), "Selecione um aluno ativo.");
        }

        var matricula = await Context.Matriculas.AsNoTracking().FirstOrDefaultAsync(item => item.Id == entity.MatriculaId);
        if (matricula is null)
        {
            ModelState.AddModelError(nameof(LancamentoFinanceiro.MatriculaId), "Selecione uma matricula valida.");
            return;
        }

        if (matricula.AlunoId != entity.AlunoId)
        {
            ModelState.AddModelError(nameof(LancamentoFinanceiro.MatriculaId), "A matricula deve pertencer ao aluno selecionado.");
        }
    }

    protected override void ApplyInactivation(LancamentoFinanceiro entity)
    {
        entity.Status = StatusFinanceiro.Cancelado;
    }
}
