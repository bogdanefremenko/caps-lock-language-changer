using CapsLockLanguageChanger.Hooks;
using CapsLockLanguageChanger.Services;

using var hook = new KeyboardHook();
hook.Install();
MessageLoop.Run();
