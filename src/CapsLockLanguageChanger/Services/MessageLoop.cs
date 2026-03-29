using CapsLockLanguageChanger.Native;

namespace CapsLockLanguageChanger.Services;

internal static class MessageLoop
{
    public static void Run()
    {
        while (NativeMethods.GetMessage(out var message, IntPtr.Zero, 0, 0))
        {
            NativeMethods.TranslateMessage(ref message);
            NativeMethods.DispatchMessage(ref message);
        }
    }
}
