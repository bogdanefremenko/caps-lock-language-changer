using LanguageChanger.Native;

namespace LanguageChanger.Services;

internal static class LanguageSwitcher
{
    public static void Switch()
    {
        SimulateKeyPress(NativeConstants.VirtualKeyLeftWin, isKeyDown: true);
        SimulateKeyPress(NativeConstants.VirtualKeySpace, isKeyDown: true);
        SimulateKeyPress(NativeConstants.VirtualKeySpace, isKeyDown: false);
        SimulateKeyPress(NativeConstants.VirtualKeyLeftWin, isKeyDown: false);
    }

    private static void SimulateKeyPress(byte virtualKey, bool isKeyDown)
    {
        var flags = isKeyDown ? 0u : NativeConstants.KeyEventKeyUp;
        NativeMethods.keybd_event(virtualKey, 0, flags, UIntPtr.Zero);
    }
}
