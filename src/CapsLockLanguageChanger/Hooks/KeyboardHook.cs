using System.Diagnostics;
using System.Runtime.InteropServices;
using CapsLockLanguageChanger.Native;
using CapsLockLanguageChanger.Services;

namespace CapsLockLanguageChanger.Hooks;

internal sealed class KeyboardHook : IDisposable
{
    private bool _isCapsLockKeyHeldDown;
    private long _capsLockPressStartTimeTicks;

    private IntPtr _hookHandle;
    private readonly ProcessModule _currentModule;
    private readonly LowLevelKeyboardProc _hookCallback;

    private bool _isDisposed;

    public KeyboardHook()
    {
        using var currentProcess = Process.GetCurrentProcess();
        _currentModule = currentProcess.MainModule!;

        _hookCallback = KeyboardHookCallback;
        _hookHandle = NativeMethods.SetWindowsHookEx(
            NativeConstants.HookTypeKeyboardLowLevel,
            _hookCallback,
            NativeMethods.GetModuleHandle(_currentModule.ModuleName),
            0);
    }

    internal static bool IsShortPress(long durationMs)
    {
        return durationMs < NativeConstants.ShortPressThresholdMs;
    }

    private void SetHookEnabled(bool enabled)
    {
        if (enabled)
        {
            _hookHandle = NativeMethods.SetWindowsHookEx(
                NativeConstants.HookTypeKeyboardLowLevel,
                _hookCallback,
                NativeMethods.GetModuleHandle(_currentModule.ModuleName),
                0);
        }
        else
        {
            NativeMethods.UnhookWindowsHookEx(_hookHandle);
        }
    }

    private IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0)
        {
            var virtualKeyCode = Marshal.ReadInt32(lParam);

            var isKeyDownEvent = wParam == NativeConstants.WindowsMessageKeyDown ||
                                 wParam == NativeConstants.WindowsMessageSystemKeyDown;
            var isKeyUpEvent = wParam == NativeConstants.WindowsMessageKeyUp ||
                               wParam == NativeConstants.WindowsMessageSystemKeyUp;

            if (virtualKeyCode == NativeConstants.VirtualKeyCapsLock)
            {
                if (isKeyDownEvent)
                {
                    if (!_isCapsLockKeyHeldDown)
                    {
                        _isCapsLockKeyHeldDown = true;
                        _capsLockPressStartTimeTicks = DateTime.UtcNow.Ticks;
                    }

                    return 1;
                }

                if (isKeyUpEvent)
                {
                    if (_isCapsLockKeyHeldDown)
                    {
                        _isCapsLockKeyHeldDown = false;

                        var durationMs = (DateTime.UtcNow.Ticks - _capsLockPressStartTimeTicks) / TimeSpan.TicksPerMillisecond;

                        if (IsShortPress(durationMs))
                        {
                            LanguageSwitcher.Switch();
                        }
                        else
                        {
                            CapsLockToggler.Toggle(SetHookEnabled);
                        }
                    }

                    return 1;
                }
            }
        }

        return NativeMethods.CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
    }

    public void Dispose()
    {
        if (_isDisposed)
            return;

        if (_hookHandle != IntPtr.Zero)
        {
            NativeMethods.UnhookWindowsHookEx(_hookHandle);
            _hookHandle = IntPtr.Zero;
        }

        _currentModule.Dispose();
        _isDisposed = true;
    }
}
