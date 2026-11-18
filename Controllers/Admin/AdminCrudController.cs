using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScholarWeb.Data;
using ScholarWeb.Models;
using ScholarWeb.ViewModels.Admin;

namespace ScholarWeb.Controllers.Admin;

[Authorize(Roles = AppRoles.Admin)]
public abstract class AdminCrudController<TEntity> : Controller where TEntity : class, new()
{
    private const int DefaultTextMaxLength = 80;
    private const int PhoneMaxLength = 15;
    private const string EmailPatternMessage = "Informe um e-mail valido.";
    private const string PhonePatternMessage = "Informe um telefone valido. Use (XX) XXXXX-XXXX ou 11 digitos.";

    protected AdminCrudController(AppDbContext context)
    {
        Context = context;
    }

    protected AppDbContext Context { get; }
    protected abstract DbSet<TEntity> Entities { get; }
    protected abstract string EntityName { get; }
    protected abstract string EntityPluralName { get; }
    protected virtual string ControllerName => GetType().Name.Replace("Controller", string.Empty);
    protected virtual string SearchPlaceholder => $"Buscar {EntityPluralName.ToLowerInvariant()}...";
    protected abstract IReadOnlyList<AdminColumnViewModel> Columns { get; }
    protected abstract IReadOnlyList<AdminDisplayFieldViewModel> DisplayFields { get; }

    public virtual async Task<IActionResult> Index(string? search)
    {
        await BeforeIndexAsync();

        var query = IncludeRelations(Entities.AsNoTracking());

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = ApplySearch(query, search.Trim());
        }

        var items = await query
            .OrderByDescending(entity => EF.Property<int>(entity, "Id"))
            .ToListAsync();

        return View("~/Views/Shared/AdminCrud/Index.cshtml", new AdminListViewModel
        {
            Title = EntityPluralName,
            EntityName = EntityName,
            ControllerName = ControllerName,
            SearchTerm = search,
            SearchPlaceholder = SearchPlaceholder,
            Columns = Columns,
            Items = items.Cast<object>().ToList()
        });
    }

    public virtual async Task<IActionResult> Details(int id)
    {
        var entity = await FindForDisplayAsync(id);

        if (entity is null)
        {
            return NotFound();
        }

        return View("~/Views/Shared/AdminCrud/Details.cshtml", new AdminDetailsViewModel
        {
            Title = $"Detalhes de {EntityName.ToLowerInvariant()}",
            EntityName = EntityName,
            ControllerName = ControllerName,
            Entity = entity,
            Fields = DisplayFields
        });
    }

    public virtual async Task<IActionResult> Create()
    {
        var entity = new TEntity();
        await InitializeNewEntityAsync(entity);
        return View("~/Views/Shared/AdminCrud/Create.cshtml", await BuildFormPageAsync(entity, "Create", false));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public virtual async Task<IActionResult> Create(IFormCollection form)
    {
        var entity = new TEntity();
        await BindAndValidateAsync(entity);
        await PrepareForSaveAsync(entity, true);
        await ValidateBusinessRulesAsync(entity, true);

        if (ModelState.IsValid)
        {
            Entities.Add(entity);
            if (await SaveWithFeedbackAsync($"{EntityName} cadastrado com sucesso."))
            {
                return RedirectToAction(nameof(Index));
            }
        }

        return View("~/Views/Shared/AdminCrud/Create.cshtml", await BuildFormPageAsync(entity, "Create", false));
    }

    public virtual async Task<IActionResult> Edit(int id)
    {
        var entity = await FindTrackedAsync(id);

        if (entity is null)
        {
            return NotFound();
        }

        return View("~/Views/Shared/AdminCrud/Edit.cshtml", await BuildFormPageAsync(entity, "Edit", true));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public virtual async Task<IActionResult> Edit(int id, IFormCollection form)
    {
        var entity = await FindTrackedAsync(id);

        if (entity is null)
        {
            return NotFound();
        }

        await BindAndValidateAsync(entity);
        await PrepareForSaveAsync(entity, false);
        await ValidateBusinessRulesAsync(entity, false);

        if (ModelState.IsValid && await SaveWithFeedbackAsync($"{EntityName} atualizado com sucesso."))
        {
            return RedirectToAction(nameof(Index));
        }

        return View("~/Views/Shared/AdminCrud/Edit.cshtml", await BuildFormPageAsync(entity, "Edit", true));
    }

    public virtual async Task<IActionResult> Delete(int id)
    {
        var entity = await FindForDisplayAsync(id);

        if (entity is null)
        {
            return NotFound();
        }

        var canDelete = await CanDeleteAsync(entity);

        return View("~/Views/Shared/AdminCrud/Delete.cshtml", new AdminDeleteViewModel
        {
            Title = canDelete ? $"Excluir {EntityName.ToLowerInvariant()}" : $"Inativar {EntityName.ToLowerInvariant()}",
            EntityName = EntityName,
            ControllerName = ControllerName,
            Entity = entity,
            Fields = DisplayFields,
            CanDelete = canDelete,
            CanInactivate = !canDelete && CanInactivate(entity),
            WarningMessage = canDelete
                ? $"Confirme a exclusao deste {EntityName.ToLowerInvariant()}."
                : $"Este {EntityName.ToLowerInvariant()} possui vinculos importantes. A opcao segura e inativar o registro.",
            Id = GetEntityId(entity)
        });
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public virtual async Task<IActionResult> DeleteConfirmed(int id)
    {
        var entity = await FindTrackedAsync(id);

        if (entity is null)
        {
            return NotFound();
        }

        if (!await CanDeleteAsync(entity))
        {
            TempData["Error"] = $"{EntityName} possui vinculos e nao pode ser excluido. Utilize a inativacao.";
            return RedirectToAction(nameof(Delete), new { id });
        }

        Entities.Remove(entity);

        if (await SaveWithFeedbackAsync($"{EntityName} excluido com sucesso."))
        {
            return RedirectToAction(nameof(Index));
        }

        return RedirectToAction(nameof(Delete), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public virtual async Task<IActionResult> Inactivate(int id)
    {
        var entity = await FindTrackedAsync(id);

        if (entity is null)
        {
            return NotFound();
        }

        if (!CanInactivate(entity))
        {
            TempData["Error"] = $"{EntityName} nao possui status para inativacao.";
            return RedirectToAction(nameof(Delete), new { id });
        }

        ApplyInactivation(entity);

        if (await SaveWithFeedbackAsync($"{EntityName} inativado com sucesso."))
        {
            return RedirectToAction(nameof(Index));
        }

        return RedirectToAction(nameof(Delete), new { id });
    }

    protected virtual Task BeforeIndexAsync()
    {
        return Task.CompletedTask;
    }

    protected virtual Task InitializeNewEntityAsync(TEntity entity)
    {
        return Task.CompletedTask;
    }

    protected virtual IQueryable<TEntity> IncludeRelations(IQueryable<TEntity> query)
    {
        return query;
    }

    protected virtual IQueryable<TEntity> ApplySearch(IQueryable<TEntity> query, string search)
    {
        return query;
    }

    protected abstract Task<IReadOnlyList<AdminFormFieldViewModel>> BuildFormFieldsAsync(TEntity entity);

    protected virtual Task PrepareForSaveAsync(TEntity entity, bool isNew)
    {
        return Task.CompletedTask;
    }

    protected virtual Task ValidateBusinessRulesAsync(TEntity entity, bool isNew)
    {
        return Task.CompletedTask;
    }

    protected virtual Task<bool> CanDeleteAsync(TEntity entity)
    {
        return Task.FromResult(true);
    }

    protected virtual bool CanInactivate(TEntity entity)
    {
        return entity.GetType().GetProperty("Status") is not null;
    }

    protected virtual void ApplyInactivation(TEntity entity)
    {
        var property = entity.GetType().GetProperty("Status");

        if (property?.PropertyType == typeof(StatusRegistro))
        {
            property.SetValue(entity, StatusRegistro.Inativo);
        }
    }

    protected virtual Task<TEntity?> FindTrackedAsync(int id)
    {
        return Entities.FindAsync(id).AsTask();
    }

    protected virtual Task<TEntity?> FindForDisplayAsync(int id)
    {
        return IncludeRelations(Entities.AsNoTracking())
            .FirstOrDefaultAsync(entity => EF.Property<int>(entity, "Id") == id);
    }

    protected async Task<AdminFormPageViewModel> BuildFormPageAsync(TEntity entity, string actionName, bool isEdit)
    {
        return new AdminFormPageViewModel
        {
            Title = isEdit ? $"Editar {EntityName.ToLowerInvariant()}" : $"Novo {EntityName.ToLowerInvariant()}",
            EntityName = EntityName,
            ControllerName = ControllerName,
            ActionName = actionName,
            Entity = entity,
            Fields = await BuildFormFieldsAsync(entity),
            IsEdit = isEdit,
            Id = isEdit ? GetEntityId(entity) : null
        };
    }

    protected async Task BindAndValidateAsync(TEntity entity)
    {
        await TryUpdateModelAsync(entity);
        TryValidateModel(entity);
    }

    protected async Task<bool> SaveWithFeedbackAsync(string successMessage)
    {
        try
        {
            await Context.SaveChangesAsync();
            TempData["Success"] = successMessage;
            return true;
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(string.Empty, "Nao foi possivel salvar. Verifique duplicidades e vinculos antes de tentar novamente.");
            return false;
        }
    }

    protected int GetEntityId(TEntity entity)
    {
        return (int)(entity.GetType().GetProperty("Id")?.GetValue(entity) ?? 0);
    }

    protected static AdminFormFieldViewModel Text(string name, string label, bool required = false, int maxLength = DefaultTextMaxLength)
    {
        return new AdminFormFieldViewModel(name, label, AdminFieldType.Text)
        {
            IsRequired = required,
            MaxLength = maxLength
        };
    }

    protected static AdminFormFieldViewModel Email(string name, string label, bool required = false)
    {
        return new AdminFormFieldViewModel(name, label, AdminFieldType.Email)
        {
            IsRequired = required,
            MaxLength = DefaultTextMaxLength,
            Pattern = ValidationPatterns.EmailInput,
            PatternMessage = EmailPatternMessage,
            InputMode = "email"
        };
    }

    protected static AdminFormFieldViewModel Phone(string name, string label)
    {
        return new AdminFormFieldViewModel(name, label, AdminFieldType.Phone)
        {
            MaxLength = PhoneMaxLength,
            Pattern = ValidationPatterns.PhoneInput,
            PatternMessage = PhonePatternMessage,
            InputMode = "tel"
        };
    }

    protected static AdminFormFieldViewModel Date(string name, string label, bool required = false)
    {
        return new AdminFormFieldViewModel(name, label, AdminFieldType.Date)
        {
            IsRequired = required
        };
    }

    protected static AdminFormFieldViewModel Number(string name, string label, bool required = false, string? min = null, string? max = null)
    {
        return new AdminFormFieldViewModel(name, label, AdminFieldType.Number)
        {
            IsRequired = required,
            Min = min,
            Max = max,
            Step = "1"
        };
    }

    protected static AdminFormFieldViewModel Money(string name, string label, bool required = false)
    {
        return new AdminFormFieldViewModel(name, label, AdminFieldType.Decimal)
        {
            IsRequired = required,
            Min = "0.01",
            Step = "0.01"
        };
    }

    protected static AdminFormFieldViewModel Grade(string name, string label)
    {
        return new AdminFormFieldViewModel(name, label, AdminFieldType.Decimal)
        {
            IsRequired = true,
            Min = "0",
            Max = "10",
            Step = "0.01"
        };
    }

    protected static AdminFormFieldViewModel TextArea(string name, string label, int rows = 4, int maxLength = DefaultTextMaxLength)
    {
        return new AdminFormFieldViewModel(name, label, AdminFieldType.TextArea)
        {
            Rows = rows,
            MaxLength = maxLength
        };
    }

    protected static AdminFormFieldViewModel Select(string name, string label, IReadOnlyList<AdminSelectOptionViewModel> options, bool required = true)
    {
        return new AdminFormFieldViewModel(name, label, AdminFieldType.Select)
        {
            Options = options,
            IsRequired = required
        };
    }

    protected static IReadOnlyList<AdminSelectOptionViewModel> EnumOptions<TEnum>() where TEnum : Enum
    {
        return Enum.GetValues(typeof(TEnum))
            .Cast<TEnum>()
            .Select(value => new AdminSelectOptionViewModel
            {
                Value = Convert.ToInt32(value).ToString(),
                Text = AdminViewHelpers.GetEnumDisplayName(value)
            })
            .ToList();
    }

    protected static string NormalizeCode(string value)
    {
        return value.Trim().ToUpperInvariant();
    }

    protected static string NormalizeEmail(string value)
    {
        return value.Trim().ToLowerInvariant();
    }

    protected static string NormalizeText(string value)
    {
        return value.Trim();
    }
}