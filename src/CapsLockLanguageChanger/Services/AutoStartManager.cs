using Microsoft.Win32;

namespace CapsLockLanguageChanger.Services;

internal static class AutoStartManager
{
    private const string RegistryKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
    internal const string ValueName = "CapsLockLanguageChanger";

    public static bool IsEnabled
    {
        get
        {
            using var key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath, writable: false);
            var value = key?.GetValue(ValueName) as string;
            return value != null && value.Equals(Environment.ProcessPath, StringComparison.OrdinalIgnoreCase);
        }
    }

    public static void Enable()
    {
        using var key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath, writable: true);
        key?.SetValue(ValueName, Environment.ProcessPath!);
    }

    public static void Disable()
    {
        using var key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath, writable: true);
        key?.DeleteValue(ValueName, throwOnMissingValue: false);
    }
}
