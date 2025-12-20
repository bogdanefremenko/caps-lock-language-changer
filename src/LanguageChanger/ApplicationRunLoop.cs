using System.Runtime.InteropServices;

namespace LanguageChanger;

public static class ApplicationRunLoop 
{ 
    public static void Run() 
    {
        while (GetMessage(out var message, IntPtr.Zero, 0, 0)) 
        { 
            TranslateMessage(ref message); 
            DispatchMessage(ref message); 
        } 
    } 

    [DllImport("user32.dll")]
    private static extern bool GetMessage(out Message lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);
    [DllImport("user32.dll")]
    private static extern bool TranslateMessage([In] ref Message lpMsg);
    [DllImport("user32.dll")]
    private static extern IntPtr DispatchMessage([In] ref Message lpMsg);
    
    [StructLayout(LayoutKind.Sequential)] 
    public struct Message 
    { 
        public IntPtr WindowHandle; 
        public uint Value; 
        public IntPtr WParam; 
        public IntPtr LParam; 
        public uint Time; 
        public System.Drawing.Point Point; 
    }
}
