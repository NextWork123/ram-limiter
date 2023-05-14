# Discord and Browsers RAM Limiter

This project aims to reduce memory usage in Discord and web browsers by utilizing the `SetProcessWorkingSetSize()` function, which helps prevent unnecessary caching in the programs.

## Supported Browsers
Currently, the following browsers are supported:
- Thorium
- Firefox
- Edge
- Chrome

If you would like support for another browser, please let me know, and i will consider adding it.

## Developers
- [miaf#2458](https://discord.com/users/308986559292768258)
- [faraj#2607](https://discord.com/users/635406751495356436)

## Instructions

To use the Discord and Browsers RAM Limiter:

1. Download the `DiscordRamLimit.exe` file from the [release page](https://github.com/NextWork123/ram-limiter/releases/download/Ram-Limiter/DiscordRamLimiter.exe).
2. Press Win + R to open the Run dialog.
3. Type `shell:startup` and press Enter. This will open the `Startup` folder.
4. Copy the `DiscordRamLimit.exe` file.
5. Paste the `DiscordRamLimit.exe` file into the `Startup` folder.
6. The `DiscordRamLimit` program will now start automatically with Windows boot.

## Comments
When running the program in DEBUG mode within Visual Studio, the CPU usage may be higher. We recommend performing your program tests after compiling as Release AnyCPU.

## Credits
This project was made possible with contributions from [Lufzys](https://github.com/Lufzys).

This project is licensed under the terms of the [MIT License](https://github.com/faraaj/discord-ram-limiter/blob/main/LICENSE).
