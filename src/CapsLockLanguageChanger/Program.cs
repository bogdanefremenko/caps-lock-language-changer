using CapsLockLanguageChanger.Tray;

using var mutex = new Mutex(true, "CapsLockLanguageChanger", out var isNew);
if (!isNew)
    return;

Application.EnableVisualStyles();
Application.SetCompatibleTextRenderingDefault(false);
Application.Run(new TrayApplicationContext());
