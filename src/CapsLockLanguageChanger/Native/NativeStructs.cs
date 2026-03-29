using System.Runtime.InteropServices;

namespace CapsLockLanguageChanger.Native;

[StructLayout(LayoutKind.Sequential)]
internal struct Message
{
    public IntPtr WindowHandle;
    public uint Value;
    public IntPtr WParam;
    public IntPtr LParam;
    public uint Time;
    public System.Drawing.Point Point;
}
