namespace SkillNet.Infrastructure.Helpers;

public static class LogMaskHelper
{
    public static string MaskEmail(string email)
    {
        var parts = email.Split('@');
        if (parts.Length != 2) return "***";
        return $"{parts[0][0]}***@{parts[1][0]}***.{parts[1].Split('.').Last()}";
    }

    public static string MaskPhone(string phone)
        => phone.Length >= 10 ? $"{phone[..3]}***{phone[^4..]}" : "***";
}
