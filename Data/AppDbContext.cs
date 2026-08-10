using Microsoft.EntityFrameworkCore;
using ScholarWeb.Models;

namespace ScholarWeb.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Aluno> Alunos => Set<Aluno>();
    public DbSet<Professor> Professores => Set<Professor>();
    public DbSet<Curso> Cursos => Set<Curso>();
    public DbSet<Turma> Turmas => Set<Turma>();
    public DbSet<Disciplina> Disciplinas => Set<Disciplina>();
    public DbSet<Matricula> Matriculas => Set<Matricula>();
    public DbSet<Nota> Notas => Set<Nota>();
    public DbSet<LancamentoFinanceiro> LancamentosFinanceiros => Set<LancamentoFinanceiro>();
    public DbSet<PeriodoLetivo> PeriodosLetivos => Set<PeriodoLetivo>();

    public override int SaveChanges()
    {
        ApplyBusinessRules();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyBusinessRules();
        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Aluno>(entity =>
        {
            entity.HasIndex(e => e.CPF).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.DataCadastro).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Professor>(entity =>
        {
            entity.HasIndex(e => e.CPF).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.DataCadastro).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Curso>(entity =>
        {
            entity.HasIndex(e => e.Codigo).IsUnique();
        });

        modelBuilder.Entity<Turma>(entity =>
        {
            entity.HasIndex(e => e.Codigo).IsUnique();
            entity.HasOne(e => e.Curso)
                .WithMany(e => e.Turmas)
                .HasForeignKey(e => e.CursoId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.PeriodoLetivo)
                .WithMany(e => e.Turmas)
                .HasForeignKey(e => e.PeriodoLetivoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Disciplina>(entity =>
        {
            entity.HasIndex(e => e.Codigo).IsUnique();
            entity.HasOne(e => e.Curso)
                .WithMany(e => e.Disciplinas)
                .HasForeignKey(e => e.CursoId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Professor)
                .WithMany(e => e.Disciplinas)
                .HasForeignKey(e => e.ProfessorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Matricula>(entity =>
        {
            entity.HasIndex(e => new { e.AlunoId, e.TurmaId, e.PeriodoLetivoId }).IsUnique();
            entity.HasOne(e => e.Aluno)
                .WithMany(e => e.Matriculas)
                .HasForeignKey(e => e.AlunoId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Curso)
                .WithMany(e => e.Matriculas)
                .HasForeignKey(e => e.CursoId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Turma)
                .WithMany(e => e.Matriculas)
                .HasForeignKey(e => e.TurmaId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.PeriodoLetivo)
                .WithMany(e => e.Matriculas)
                .HasForeignKey(e => e.PeriodoLetivoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Nota>(entity =>
        {
            entity.HasIndex(e => new { e.MatriculaId, e.DisciplinaId }).IsUnique();
            entity.Property(e => e.Nota1).HasPrecision(5, 2);
            entity.Property(e => e.Nota2).HasPrecision(5, 2);
            entity.Property(e => e.Nota3).HasPrecision(5, 2);
            entity.Property(e => e.MediaFinal).HasPrecision(5, 2);
            entity.HasOne(e => e.Matricula)
                .WithMany(e => e.Notas)
                .HasForeignKey(e => e.MatriculaId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Disciplina)
                .WithMany(e => e.Notas)
                .HasForeignKey(e => e.DisciplinaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<LancamentoFinanceiro>(entity =>
        {
            entity.Property(e => e.Valor).HasPrecision(12, 2);
            entity.HasOne(e => e.Aluno)
                .WithMany(e => e.LancamentosFinanceiros)
                .HasForeignKey(e => e.AlunoId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Matricula)
                .WithMany(e => e.LancamentosFinanceiros)
                .HasForeignKey(e => e.MatriculaId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ApplyBusinessRules()
    {
        foreach (var entry in ChangeTracker.Entries<Aluno>())
        {
            if (entry.State == EntityState.Added && entry.Entity.DataCadastro == default)
            {
                entry.Entity.DataCadastro = DateTime.UtcNow;
            }
        }

        foreach (var entry in ChangeTracker.Entries<Professor>())
        {
            if (entry.State == EntityState.Added && entry.Entity.DataCadastro == default)
            {
                entry.Entity.DataCadastro = DateTime.UtcNow;
            }
        }

        foreach (var entry in ChangeTracker.Entries<Nota>())
        {
            if (entry.State is EntityState.Added or EntityState.Modified)
            {
                CalcularResultado(entry.Entity);
            }
        }

        foreach (var entry in ChangeTracker.Entries<LancamentoFinanceiro>())
        {
            if (entry.State is EntityState.Added or EntityState.Modified)
            {
                AtualizarStatusFinanceiro(entry.Entity);
            }
        }
    }

    private static void CalcularResultado(Nota nota)
    {
        nota.MediaFinal = Math.Round((nota.Nota1 + nota.Nota2 + nota.Nota3) / 3, 2);

        nota.Situacao = nota.MediaFinal >= 7
            ? SituacaoNota.Aprovado
            : nota.MediaFinal >= 5
                ? SituacaoNota.Recuperacao
                : SituacaoNota.Reprovado;
    }

    private static void AtualizarStatusFinanceiro(LancamentoFinanceiro lancamento)
    {
        if (lancamento.Status == StatusFinanceiro.Cancelado)
        {
            return;
        }

        if (lancamento.DataPagamento.HasValue)
        {
            lancamento.Status = StatusFinanceiro.Pago;
            return;
        }

        lancamento.Status = lancamento.DataVencimento.Date < DateTime.Today
            ? StatusFinanceiro.Atrasado
            : StatusFinanceiro.Pendente;
    }
}