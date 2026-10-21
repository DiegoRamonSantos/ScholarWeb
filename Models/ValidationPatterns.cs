namespace ScholarWeb.Models;

public static class ValidationPatterns
{
    public const string Email = @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$";
    public const string EmailInput = @"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}";

    public const string Phone = @"^(\(\d{2}\) ?\d{5}-\d{4}|\(\d{2}\) ?\d{9}|\d{11})$";
    public const string PhoneInput = @"(\(\d{2}\) ?\d{5}-\d{4}|\(\d{2}\) ?\d{9}|\d{11})";
}
