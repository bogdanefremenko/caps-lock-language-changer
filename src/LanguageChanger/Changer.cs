using System.Diagnostics;
using System.Runtime.InteropServices;

namespace LanguageChanger;

public sealed class Changer : IDisposable
{
    private const int HookTypeKeyboardLowLevel = 13;
    private const int WindowsMessageKeyDown = 0x0100;
    private const int WindowsMessageKeyUp = 0x0101;
    private const int WindowsMessageSystemKeyDown = 0x0104;
    private const int WindowsMessageSystemKeyUp = 0x0105;

    private const int VirtualKeyCapsLock = 0x14;
    private const int VirtualKeyLeftWin = 0x5B;
    private const int VirtualKeySpace = 0x20;
    private const uint KeyEventKeyUp = 0x0002;

    private bool _isCapsLockKeyHeldDown;
    private long _capsLockPressStartTimeTicks;
    
    private IntPtr _hookIdentifier;
    private ProcessModule _currentModule = null!;
    
    private bool _isDisposed;

    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    public void Launch()
    {
        using var currentProcess = Process.GetCurrentProcess();
        _currentModule = currentProcess.MainModule!;
        
        var hookIdentifier = SetWindowsHookEx(HookTypeKeyboardLowLevel, KeyboardHookCallback, NativeHelpers.GetModuleHandle(_currentModule.ModuleName), 0);

        Console.WriteLine("CapsLock Switcher is running)...");
        Console.WriteLine("Press Ctrl+C to exit.");

        ApplicationRunLoop.Run();

        NativeHelpers.UnhookWindowsHookEx(hookIdentifier);
    }

    private IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0)
        {
            var virtualKeyCode = Marshal.ReadInt32(lParam);

            var isKeyDownEvent = wParam is WindowsMessageKeyDown or WindowsMessageSystemKeyDown;
            var isKeyUpEvent = wParam is WindowsMessageKeyUp or WindowsMessageSystemKeyUp;

            if (virtualKeyCode == VirtualKeyCapsLock)
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

                        if (durationInMilliseconds < 300)
                        {
                            PerformLanguageSwitch();
                        }
                        else
                        {
                            ToggleSystemCapsLockState();
                        }
                    }

                    return 1;
                }
            }
        }

        return NativeHelpers.CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
    }

    private void ToggleSystemCapsLockState()
    {
        NativeHelpers.UnhookWindowsHookEx(_hookIdentifier);

        SimulateKeyPress(VirtualKeyCapsLock, true);
        SimulateKeyPress(VirtualKeyCapsLock, false);

        _hookIdentifier = SetWindowsHookEx(
            HookTypeKeyboardLowLevel,
            KeyboardHookCallback,
            NativeHelpers.GetModuleHandle(_currentModule.ModuleName),
            0
        );
    }

    private void PerformLanguageSwitch()
    {
        SimulateKeyPress(VirtualKeyLeftWin, true);
        SimulateKeyPress(VirtualKeySpace, true);
        SimulateKeyPress(VirtualKeySpace, false);
        SimulateKeyPress(VirtualKeyLeftWin, false);
    }

    private void SimulateKeyPress(byte virtualKey, bool isKeyDown)
    {
        var flags = isKeyDown ? 0u : KeyEventKeyUp;
        NativeHelpers.keybd_event(virtualKey, 0, flags, UIntPtr.Zero);
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);
    
    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }
        
        _currentModule.Dispose();
        
        _isDisposed = true;
    }
}
