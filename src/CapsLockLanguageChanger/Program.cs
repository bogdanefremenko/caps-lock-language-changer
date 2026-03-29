using CapsLockLanguageChanger.Hooks;

using var hook = new KeyboardHook();
hook.Install();
Application.Run();
