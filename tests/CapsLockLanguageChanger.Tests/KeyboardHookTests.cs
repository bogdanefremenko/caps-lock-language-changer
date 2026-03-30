using CapsLockLanguageChanger.Hooks;

namespace CapsLockLanguageChanger.Tests;

public class KeyboardHookTests
{
    [Theory]
    [InlineData(0, true)]
    [InlineData(1, true)]
    [InlineData(150, true)]
    [InlineData(299, true)]
    [InlineData(300, false)]
    [InlineData(301, false)]
    [InlineData(1000, false)]
    [InlineData(10000, false)]
    public void IsShortPress_ReturnsExpected(long durationMs, bool expected)
    {
        var result = KeyboardHook.IsShortPress(durationMs);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void IsShortPress_NegativeDuration_ReturnsTrue()
    {
        // Negative duration is technically "less than threshold"
        Assert.True(KeyboardHook.IsShortPress(-1));
    }
}
