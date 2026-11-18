using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScholarWeb.Data;
using ScholarWeb.Models;
using ScholarWeb.ViewModels;

namespace ScholarWeb.Controllers;

[Authorize(Roles = AppRoles.Admin)]
public class DashboardController : Controller
{
    private readonly AppDbContext _context;

    public DashboardController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var today = DateTime.Today;
        var atrasados = _context.LancamentosFinanceiros
            .Where(lancamento =>
                lancamento.Status == StatusFinanceiro.Atrasado ||
                (lancamento.Status == StatusFinanceiro.Pendente &&
                 lancamento.DataPagamento == null &&
                 lancamento.DataVencimento < today));

        var pendentes = _context.LancamentosFinanceiros
            .Where(lancamento =>
                (lancamento.Status == StatusFinanceiro.Pendente || lancamento.Status == StatusFinanceiro.Atrasado) &&
                lancamento.DataPagamento == null);

        var recebidos = _context.LancamentosFinanceiros
            .Where(lancamento => lancamento.Status == StatusFinanceiro.Pago);

        var viewModel = new DashboardViewModel
        {
            TotalAlunos = await _context.Alunos.CountAsync(aluno => aluno.Status == StatusRegistro.Ativo),
            TotalProfessores = await _context.Professores.CountAsync(professor => professor.Status == StatusRegistro.Ativo),
            TotalCursos = await _context.Cursos.CountAsync(curso => curso.Status == StatusRegistro.Ativo),
            TotalTurmas = await _context.Turmas.CountAsync(turma => turma.Status == StatusTurma.Ativa),
            FinanceirosAtrasados = await atrasados.CountAsync(),
            PagamentosRecebidos = await recebidos.CountAsync(),
            PagamentosPendentes = await pendentes.CountAsync(),
            ReceitaRecebida = await recebidos.SumAsync(lancamento => (decimal?)lancamento.Valor) ?? 0m,
            ReceitaPendente = await pendentes.SumAsync(lancamento => (decimal?)lancamento.Valor) ?? 0m,
            UltimosAlunosCadastrados = await _context.Alunos.AsNoTracking()
                .OrderByDescending(aluno => aluno.DataCadastro)
                .ThenByDescending(aluno => aluno.Id)
                .Take(6)
                .Select(aluno => new DashboardAlunoItemViewModel
                {
                    NomeCompleto = aluno.NomeCompleto,
                    CPF = aluno.CPF,
                    Email = aluno.Email,
                    Cidade = aluno.Cidade,
                    Estado = aluno.Estado,
                    Status = aluno.Status,
                    DataCadastro = aluno.DataCadastro
                })
                .ToListAsync()
        };

        return View(viewModel);
    }
}
