using CapsLockLanguageChanger.Native;

namespace CapsLockLanguageChanger.Services;

internal static class CapsLockToggler
{
    public static void Toggle(Action<bool> setHookEnabled)
    {
        setHookEnabled(false);

        SimulateKeyPress(NativeConstants.VirtualKeyCapsLock, isKeyDown: true);
        SimulateKeyPress(NativeConstants.VirtualKeyCapsLock, isKeyDown: false);

        setHookEnabled(true);
    }

    private static void SimulateKeyPress(byte virtualKey, bool isKeyDown)
    {
        var flags = isKeyDown ? 0u : NativeConstants.KeyEventKeyUp;
        NativeMethods.keybd_event(virtualKey, 0, flags, UIntPtr.Zero);
    }
}
