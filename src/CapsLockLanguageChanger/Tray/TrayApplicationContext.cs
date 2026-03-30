using System.Reflection;
using CapsLockLanguageChanger.Hooks;
using CapsLockLanguageChanger.Services;

namespace CapsLockLanguageChanger.Tray;

internal sealed class TrayApplicationContext : ApplicationContext
{
    private readonly KeyboardHook _keyboardHook;
    private readonly NotifyIcon _notifyIcon;

    public TrayApplicationContext()
    {
        _keyboardHook = new KeyboardHook();

        var autoStartItem = new ToolStripMenuItem("Start with Windows")
        {
            CheckOnClick = true,
            Checked = AutoStartManager.IsEnabled
        };
        autoStartItem.Click += (_, _) =>
        {
            if (autoStartItem.Checked)
                AutoStartManager.Enable();
            else
                AutoStartManager.Disable();
        };

        var exitItem = new ToolStripMenuItem("Exit");
        exitItem.Click += (_, _) => ExitApplication();

        var contextMenu = new ContextMenuStrip();
        contextMenu.Items.Add(autoStartItem);
        contextMenu.Items.Add(new ToolStripSeparator());
        contextMenu.Items.Add(exitItem);

        _notifyIcon = new NotifyIcon
        {
            Icon = LoadEmbeddedIcon(),
            Text = "CapsLock Language Changer",
            Visible = true,
            ContextMenuStrip = contextMenu
        };
    }

    private static Icon LoadEmbeddedIcon()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var stream = assembly.GetManifestResourceStream("CapsLockLanguageChanger.Resources.icon.ico");
        return stream != null ? new Icon(stream) : SystemIcons.Application;
    }

    private void ExitApplication()
    {
        ExitThread();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();
            _keyboardHook.Dispose();
        }

        base.Dispose(disposing);
    }
}
