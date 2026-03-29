namespace CapsLockLanguageChanger.Native;

internal static class NativeConstants
{
    public const int HookTypeKeyboardLowLevel = 13;

    public const int WindowsMessageKeyDown = 0x0100;
    public const int WindowsMessageKeyUp = 0x0101;
    public const int WindowsMessageSystemKeyDown = 0x0104;
    public const int WindowsMessageSystemKeyUp = 0x0105;

    public const byte VirtualKeyCapsLock = 0x14;
    public const byte VirtualKeyLeftWin = 0x5B;
    public const byte VirtualKeySpace = 0x20;

    public const uint KeyEventKeyUp = 0x0002;

    public const int ShortPressThresholdMs = 300;
}
