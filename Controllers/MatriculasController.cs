using Microsoft.EntityFrameworkCore;
using ScholarWeb.Controllers.Admin;
using ScholarWeb.Data;
using ScholarWeb.Models;
using ScholarWeb.ViewModels.Admin;

namespace ScholarWeb.Controllers;

public class MatriculasController : AdminCrudController<Matricula>
{
    public MatriculasController(AppDbContext context) : base(context)
    {
    }

    protected override DbSet<Matricula> Entities => Context.Matriculas;
    protected override string EntityName => "Matricula";
    protected override string EntityPluralName => "Matriculas";
    protected override string SearchPlaceholder => "Buscar por aluno, curso, turma ou periodo";

    protected override IReadOnlyList<AdminColumnViewModel> Columns =>
    [
        new("Aluno", "Aluno.NomeCompleto"),
        new("Curso", "Curso.Nome"),
        new("Turma", "Turma.Nome"),
        new("Periodo", "PeriodoLetivo.Nome"),
        new("Data", nameof(Matricula.DataMatricula), AdminDisplayFormat.Date),
        new("Status", nameof(Matricula.Status), AdminDisplayFormat.Status)
    ];

    protected override IReadOnlyList<AdminDisplayFieldViewModel> DisplayFields =>
    [
        new("Aluno", "Aluno.NomeCompleto"),
        new("Curso", "Curso.Nome"),
        new("Turma", "Turma.Nome"),
        new("Periodo letivo", "PeriodoLetivo.Nome"),
        new("Data de matricula", nameof(Matricula.DataMatricula), AdminDisplayFormat.Date),
        new("Status", nameof(Matricula.Status), AdminDisplayFormat.Status),
        new("Observacoes", nameof(Matricula.Observacoes))
    ];

    protected override IQueryable<Matricula> IncludeRelations(IQueryable<Matricula> query)
    {
        return query
            .Include(matricula => matricula.Aluno)
            .Include(matricula => matricula.Curso)
            .Include(matricula => matricula.Turma)
            .Include(matricula => matricula.PeriodoLetivo);
    }

    protected override IQueryable<Matricula> ApplySearch(IQueryable<Matricula> query, string search)
    {
        return query.Where(matricula =>
            matricula.Aluno.NomeCompleto.Contains(search) ||
            matricula.Curso.Nome.Contains(search) ||
            matricula.Turma.Nome.Contains(search) ||
            matricula.PeriodoLetivo.Nome.Contains(search));
    }

    protected override Task InitializeNewEntityAsync(Matricula entity)
    {
        entity.DataMatricula = DateTime.Today;
        return Task.CompletedTask;
    }

    protected override async Task<IReadOnlyList<AdminFormFieldViewModel>> BuildFormFieldsAsync(Matricula entity)
    {
        var alunos = await Context.Alunos.AsNoTracking()
            .Where(aluno => aluno.Status == StatusRegistro.Ativo || aluno.Id == entity.AlunoId)
            .OrderBy(aluno => aluno.NomeCompleto)
            .Select(aluno => new { aluno.Id, aluno.NomeCompleto, aluno.CPF })
            .ToListAsync();

        var cursos = await Context.Cursos.AsNoTracking()
            .Where(curso => curso.Status == StatusRegistro.Ativo || curso.Id == entity.CursoId)
            .OrderBy(curso => curso.Nome)
            .Select(curso => new { curso.Id, curso.Nome, curso.Codigo })
            .ToListAsync();

        var turmas = await Context.Turmas.AsNoTracking()
            .Include(turma => turma.Curso)
            .Include(turma => turma.PeriodoLetivo)
            .Where(turma => turma.Status == StatusTurma.Ativa || turma.Id == entity.TurmaId)
            .OrderBy(turma => turma.Nome)
            .Select(turma => new
            {
                turma.Id,
                turma.Nome,
                turma.Codigo,
                CursoNome = turma.Curso.Nome,
                PeriodoNome = turma.PeriodoLetivo.Nome
            })
            .ToListAsync();

        var periodos = await Context.PeriodosLetivos.AsNoTracking()
            .Where(periodo => periodo.Status == StatusPeriodoLetivo.Aberto || periodo.Id == entity.PeriodoLetivoId)
            .OrderByDescending(periodo => periodo.Ano)
            .ThenByDescending(periodo => periodo.Semestre)
            .Select(periodo => new { periodo.Id, periodo.Nome, periodo.Ano, periodo.Semestre })
            .ToListAsync();

        return
        [
            Select(nameof(Matricula.AlunoId), "Aluno", alunos.Select(aluno => new AdminSelectOptionViewModel
            {
                Value = aluno.Id.ToString(),
                Text = $"{aluno.NomeCompleto} - {aluno.CPF}"
            }).ToList()),
            Select(nameof(Matricula.CursoId), "Curso", cursos.Select(curso => new AdminSelectOptionViewModel
            {
                Value = curso.Id.ToString(),
                Text = $"{curso.Nome} ({curso.Codigo})"
            }).ToList()),
            Select(nameof(Matricula.TurmaId), "Turma", turmas.Select(turma => new AdminSelectOptionViewModel
            {
                Value = turma.Id.ToString(),
                Text = $"{turma.Nome} ({turma.Codigo}) - {turma.CursoNome} / {turma.PeriodoNome}"
            }).ToList()),
            Select(nameof(Matricula.PeriodoLetivoId), "Periodo letivo", periodos.Select(periodo => new AdminSelectOptionViewModel
            {
                Value = periodo.Id.ToString(),
                Text = $"{periodo.Nome} - {periodo.Ano}/{periodo.Semestre}"
            }).ToList()),
            Date(nameof(Matricula.DataMatricula), "Data de matricula", true),
            Select(nameof(Matricula.Status), "Status", EnumOptions<StatusMatricula>()),
            TextArea(nameof(Matricula.Observacoes), "Observacoes")
        ];
    }

    protected override Task PrepareForSaveAsync(Matricula entity, bool isNew)
    {
        entity.Observacoes = entity.Observacoes?.Trim();
        if (entity.DataMatricula == default)
        {
            entity.DataMatricula = DateTime.Today;
        }

        return Task.CompletedTask;
    }

    protected override async Task ValidateBusinessRulesAsync(Matricula entity, bool isNew)
    {
        var duplicate = await Context.Matriculas.AnyAsync(matricula =>
            matricula.AlunoId == entity.AlunoId &&
            matricula.TurmaId == entity.TurmaId &&
            matricula.PeriodoLetivoId == entity.PeriodoLetivoId &&
            matricula.Id != entity.Id);

        if (duplicate)
        {
            ModelState.AddModelError(string.Empty, "Ja existe matricula deste aluno nesta turma e periodo letivo.");
        }

        var alunoAtivo = await Context.Alunos.AnyAsync(aluno => aluno.Id == entity.AlunoId && aluno.Status == StatusRegistro.Ativo);
        if (!alunoAtivo)
        {
            ModelState.AddModelError(nameof(Matricula.AlunoId), "Selecione um aluno ativo.");
        }

        var cursoAtivo = await Context.Cursos.AnyAsync(curso => curso.Id == entity.CursoId && curso.Status == StatusRegistro.Ativo);
        if (!cursoAtivo)
        {
            ModelState.AddModelError(nameof(Matricula.CursoId), "Selecione um curso ativo.");
        }

        var turma = await Context.Turmas.AsNoTracking().FirstOrDefaultAsync(item => item.Id == entity.TurmaId);
        if (turma is null)
        {
            ModelState.AddModelError(nameof(Matricula.TurmaId), "Selecione uma turma valida.");
            return;
        }

        if (turma.Status != StatusTurma.Ativa)
        {
            ModelState.AddModelError(nameof(Matricula.TurmaId), "Nao e permitido matricular em turma encerrada ou cancelada.");
        }

        if (turma.CursoId != entity.CursoId)
        {
            ModelState.AddModelError(nameof(Matricula.CursoId), "O curso selecionado deve ser o mesmo da turma.");
        }

        if (turma.PeriodoLetivoId != entity.PeriodoLetivoId)
        {
            ModelState.AddModelError(nameof(Matricula.PeriodoLetivoId), "O periodo letivo deve ser o mesmo da turma.");
        }

        var periodo = await Context.PeriodosLetivos.AsNoTracking().FirstOrDefaultAsync(item => item.Id == entity.PeriodoLetivoId);
        if (periodo is null || periodo.Status != StatusPeriodoLetivo.Aberto)
        {
            ModelState.AddModelError(nameof(Matricula.PeriodoLetivoId), "Nao e permitido matricular em periodo letivo encerrado ou cancelado.");
        }

        if (entity.Status == StatusMatricula.Ativa)
        {
            var ocupacao = await Context.Matriculas.CountAsync(matricula =>
                matricula.TurmaId == entity.TurmaId &&
                matricula.PeriodoLetivoId == entity.PeriodoLetivoId &&
                matricula.Status == StatusMatricula.Ativa &&
                matricula.Id != entity.Id);

            if (ocupacao >= turma.CapacidadeMaxima)
            {
                ModelState.AddModelError(nameof(Matricula.TurmaId), "A turma esta lotada.");
            }
        }
    }

    protected override async Task<bool> CanDeleteAsync(Matricula entity)
    {
        var hasNotas = await Context.Notas.AnyAsync(nota => nota.MatriculaId == entity.Id);
        var hasFinanceiro = await Context.LancamentosFinanceiros.AnyAsync(lancamento => lancamento.MatriculaId == entity.Id);
        return !hasNotas && !hasFinanceiro;
    }

    protected override void ApplyInactivation(Matricula entity)
    {
        entity.Status = StatusMatricula.Cancelada;
    }
}
