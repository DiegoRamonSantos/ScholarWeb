using System.ComponentModel.DataAnnotations;

namespace ScholarWeb.Models;

public enum RecordStatus
{
    [Display(Name = "Ativo")]
    Ativo = 1,

    [Display(Name = "Inativo")]
    Inativo = 2
}

public enum ClassGroupStatus
{
    [Display(Name = "Ativa")]
    Ativa = 1,

    [Display(Name = "Encerrada")]
    Encerrada = 2,

    [Display(Name = "Cancelada")]
    Cancelada = 3
}

public enum AcademicTermStatus
{
    [Display(Name = "Aberto")]
    Aberto = 1,

    [Display(Name = "Encerrado")]
    Encerrado = 2,

    [Display(Name = "Cancelado")]
    Cancelado = 3
}

public enum EnrollmentStatus
{
    [Display(Name = "Ativa")]
    Ativa = 1,

    [Display(Name = "Trancada")]
    Trancada = 2,

    [Display(Name = "Cancelada")]
    Cancelada = 3,

    [Display(Name = "Concluida")]
    Concluida = 4
}

public enum GradeStatus
{
    [Display(Name = "Aprovado")]
    Aprovado = 1,

    [Display(Name = "Recuperacao")]
    Recuperacao = 2,

    [Display(Name = "Reprovado")]
    Reprovado = 3
}

public enum FinancialEntryType
{
    [Display(Name = "Mensalidade")]
    Mensalidade = 1,

    [Display(Name = "Matricula")]
    Matricula = 2,

    [Display(Name = "Taxa")]
    Taxa = 3,

    [Display(Name = "Material")]
    Material = 4,

    [Display(Name = "Outro")]
    Outro = 5
}

public enum FinancialStatus
{
    [Display(Name = "Pendente")]
    Pendente = 1,

    [Display(Name = "Pago")]
    Pago = 2,

    [Display(Name = "Atrasado")]
    Atrasado = 3,

    [Display(Name = "Cancelado")]
    Cancelado = 4
}

public enum Shift
{
    [Display(Name = "Matutino")]
    Matutino = 1,

    [Display(Name = "Vespertino")]
    Vespertino = 2,

    [Display(Name = "Noturno")]
    Noturno = 3,

    [Display(Name = "Integral")]
    Integral = 4,

    [Display(Name = "EAD")]
    Ead = 5
}
