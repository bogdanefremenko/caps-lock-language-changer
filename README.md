# CapsLock Language Changer

Remap CapsLock to switch keyboard languages on Windows.

## Features

- **Short press** CapsLock (< 300ms) — switches input language (simulates Win+Space)
- **Long press** CapsLock (>= 300ms) — toggles CapsLock on/off as usual
- **System tray** icon with context menu
- **Start with Windows** option via system tray

## Installation

Download the latest `CapsLockLanguageChanger-win-x64.exe` from [Releases](../../releases). Move it to a permanent location (e.g. `C:\Program Files\CapsLockLanguageChanger\`) and run it from there. This is important if you plan to use the "Start with Windows" option, as it registers the exe path for auto-start.

The app runs in the system tray. Right-click the tray icon for options:

- **Start with Windows** — toggle auto-start on login
- **Exit** — close the application

## Build from Source

Requires [.NET 10 SDK](https://dotnet.microsoft.com/download).

```bash
dotnet publish src/CapsLockLanguageChanger/CapsLockLanguageChanger.csproj -c Release -o ./publish
```

The output is a self-contained single-file executable in `./publish/`.

## How It Works

The application installs a [low-level keyboard hook](https://learn.microsoft.com/en-us/windows/win32/winmsg/about-hooks#wh_keyboard_ll) to intercept CapsLock key events. It measures press duration to distinguish between a quick tap (language switch) and a held press (CapsLock toggle). Language switching is done by simulating the Win+Space key combination.

## Contributing

Contributions are welcome. Please open an issue to discuss proposed changes before submitting a pull request.

## License

[MIT](LICENSE)
