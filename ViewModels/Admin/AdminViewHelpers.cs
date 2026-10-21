using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

namespace ScholarWeb.ViewModels.Admin;

public static class AdminViewHelpers
{
    private static readonly CultureInfo PtBr = CultureInfo.GetCultureInfo("pt-BR");

    public static object? GetRawValue(object item, string propertyPath)
    {
        object? current = item;

        foreach (var part in propertyPath.Split('.'))
        {
            if (current is null)
            {
                return null;
            }

            current = current.GetType().GetProperty(part)?.GetValue(current);
        }

        return current;
    }

    public static string GetDisplayValue(object item, string propertyPath, AdminDisplayFormat format = AdminDisplayFormat.Text)
    {
        var value = GetRawValue(item, propertyPath);

        if (value is null)
        {
            return "-";
        }

        if (value is Enum enumValue)
        {
            return GetEnumDisplayName(enumValue);
        }

        return format switch
        {
            AdminDisplayFormat.Date when value is DateTime date => date.ToString("dd/MM/yyyy", PtBr),
            AdminDisplayFormat.DateTime when value is DateTime dateTime => dateTime.ToString("dd/MM/yyyy HH:mm", PtBr),
            AdminDisplayFormat.Currency when value is decimal money => money.ToString("C", PtBr),
            AdminDisplayFormat.Decimal when value is decimal number => number.ToString("N2", PtBr),
            _ => Convert.ToString(value, PtBr) ?? "-"
        };
    }

    public static string GetInputValue(object item, AdminFormFieldViewModel field)
    {
        var value = GetRawValue(item, field.Name);

        if (value is null)
        {
            return string.Empty;
        }

        if (value is DateTime dateTime)
        {
            return dateTime == default ? string.Empty : dateTime.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        if (value is decimal decimalValue)
        {
            return decimalValue.ToString("0.##", CultureInfo.InvariantCulture);
        }

        if (value is Enum enumValue)
        {
            return Convert.ToInt32(enumValue, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
        }

        return Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
    }

    public static string GetEnumDisplayName(Enum value)
    {
        var member = value.GetType().GetMember(value.ToString()).FirstOrDefault();
        var display = member?.GetCustomAttribute<DisplayAttribute>();
        return display?.GetName() ?? value.ToString();
    }

    public static string GetBadgeClass(string text)
    {
        var normalized = text.ToLowerInvariant();

        if (normalized.Contains("ativo") || normalized.Contains("aberto") || normalized.Contains("aprovado") || normalized.Contains("pago"))
        {
            return "badge badge-success";
        }

        if (normalized.Contains("pendente") || normalized.Contains("recuperacao") || normalized.Contains("trancada"))
        {
            return "badge badge-warning";
        }

        if (normalized.Contains("atrasado") || normalized.Contains("reprovado") || normalized.Contains("cancelado") || normalized.Contains("encerrado") || normalized.Contains("inativo"))
        {
            return "badge badge-danger";
        }

        return "badge badge-neutral";
    }
}
