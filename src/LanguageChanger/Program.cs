using LanguageChanger.Hooks;
using LanguageChanger.Services;

using var hook = new KeyboardHook();
hook.Install();
MessageLoop.Run();
