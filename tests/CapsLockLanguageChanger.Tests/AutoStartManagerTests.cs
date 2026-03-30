using CapsLockLanguageChanger.Services;
using Microsoft.Win32;

namespace CapsLockLanguageChanger.Tests;

public class AutoStartManagerTests : IDisposable
{
    private const string RegistryKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";

    public AutoStartManagerTests()
    {
        // Clean state before each test
        RemoveTestRegistryValue();
    }

    [Fact]
    public void IsEnabled_WhenNoRegistryKey_ReturnsFalse()
    {
        Assert.False(AutoStartManager.IsEnabled);
    }

    [Fact]
    public void Enable_SetsRegistryValue()
    {
        AutoStartManager.Enable();

        using var key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath, writable: false);
        var value = key?.GetValue(AutoStartManager.ValueName) as string;
        Assert.NotNull(value);
        Assert.Equal(Environment.ProcessPath, value, ignoreCase: true);
    }

    [Fact]
    public void IsEnabled_AfterEnable_ReturnsTrue()
    {
        AutoStartManager.Enable();
        Assert.True(AutoStartManager.IsEnabled);
    }

    [Fact]
    public void Disable_RemovesRegistryValue()
    {
        AutoStartManager.Enable();
        AutoStartManager.Disable();

        using var key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath, writable: false);
        var value = key?.GetValue(AutoStartManager.ValueName);
        Assert.Null(value);
    }

    [Fact]
    public void IsEnabled_AfterDisable_ReturnsFalse()
    {
        AutoStartManager.Enable();
        AutoStartManager.Disable();
        Assert.False(AutoStartManager.IsEnabled);
    }

    [Fact]
    public void Disable_WhenNotEnabled_DoesNotThrow()
    {
        var exception = Record.Exception(() => AutoStartManager.Disable());
        Assert.Null(exception);
    }

    public void Dispose()
    {
        RemoveTestRegistryValue();
    }

    private static void RemoveTestRegistryValue()
    {
        using var key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath, writable: true);
        key?.DeleteValue(AutoStartManager.ValueName, throwOnMissingValue: false);
    }
}
