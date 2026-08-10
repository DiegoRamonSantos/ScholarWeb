using ScholarWeb.Models;

namespace ScholarWeb.ViewModels;

public class DashboardViewModel
{
    public int TotalAlunos { get; set; }
    public int TotalProfessores { get; set; }
    public int TotalCursos { get; set; }
    public int TotalTurmas { get; set; }
    public int FinanceirosAtrasados { get; set; }
    public int PagamentosRecebidos { get; set; }
    public int PagamentosPendentes { get; set; }
    public decimal ReceitaRecebida { get; set; }
    public decimal ReceitaPendente { get; set; }
    public IReadOnlyList<DashboardAlunoItemViewModel> UltimosAlunosCadastrados { get; set; } = [];
}

public class DashboardAlunoItemViewModel
{
    public string NomeCompleto { get; set; } = string.Empty;
    public string CPF { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    public StatusRegistro Status { get; set; }
    public DateTime DataCadastro { get; set; }
}
