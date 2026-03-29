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
    private ProcessModule? _currentModule;
    private LowLevelKeyboardProc? _hookCallback;

    private bool _isDisposed;

    public void Install()
    {
        using var currentProcess = Process.GetCurrentProcess();
        _currentModule = currentProcess.MainModule!;

        _hookCallback = KeyboardHookCallback;
        _hookHandle = NativeMethods.SetWindowsHookEx(
            NativeConstants.HookTypeKeyboardLowLevel,
            _hookCallback,
            NativeMethods.GetModuleHandle(_currentModule.ModuleName),
            0);

        Console.WriteLine("CapsLock Switcher is running...");
        Console.WriteLine("Press Ctrl+C to exit.");
    }

    public void Uninstall()
    {
        if (_hookHandle != IntPtr.Zero)
        {
            NativeMethods.UnhookWindowsHookEx(_hookHandle);
            _hookHandle = IntPtr.Zero;
        }
    }

    private void SetHookEnabled(bool enabled)
    {
        if (enabled)
        {
            _hookHandle = NativeMethods.SetWindowsHookEx(
                NativeConstants.HookTypeKeyboardLowLevel,
                _hookCallback!,
                NativeMethods.GetModuleHandle(_currentModule!.ModuleName),
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

                        var durationInMilliseconds = (DateTime.UtcNow.Ticks - _capsLockPressStartTimeTicks) / TimeSpan.TicksPerMillisecond;

                        if (durationInMilliseconds < NativeConstants.ShortPressThresholdMs)
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
        {
            return;
        }

        Uninstall();
        _currentModule?.Dispose();

        _isDisposed = true;
    }
}
