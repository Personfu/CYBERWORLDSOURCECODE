# KeyChainWindows.dll and MemoryMonitorWindows.dll for Club Penguin Island Offline Mode

This project is a **fan-made recreation** of the `KeyChainWindows.dll` and `MemoryMonitorWindows.dll`, which is used for encrypting and managing player data in offline mode for **Club Penguin Island**. This DLL enables secure data encryption and decryption, making it essential for players using the custom built offline version of the game.

## Features

- **Data Encryption and Decryption**: Provides encryption for player data to ensure security while playing in offline mode.
- **Custom Build for Offline Mode**: Specifically designed to work with offline **Club Penguin Island** client setups.
- **Open Source**: The source code is fully open for modifications, improvements, and integration into other custom clients.

## Project Requirements

To build the `KeyChainWindows.dll` and `MemoryMonitorWindows.dll`, you will need:

- **Visual Studio 2026** with the **Desktop development with C++** workload installed, including **v145 build tools**.
  - Ensure that the **MSVC v145 - VS 2026 C++ x64/x86 build tools (Latest)** are selected during installation.
  [Download Visual Studio 2022](https://visualstudio.microsoft.com/downloads/)
  
- The latest version of the **Windows SDK**.  
  [Download Windows SDK](https://developer.microsoft.com/en-us/windows/downloads/windows-sdk/)

Ensure that Visual Studio is configured to use the correct Windows SDK version after installation.

## Build Instructions

For detailed setup instructions and steps to build the `KeyChainWindows.dll` and `MemoryMonitorWindows.dll`, including how to ensure the **v143 tools** are selected, refer to the [instructions.md](https://github.com/OpenCPIsland/KeyChainWindows/blob/main/instructions.md) file.

## Target Architecture

Ensure you are building the DLL for the appropriate architecture:

- **x64**: 64-bit systems.
- **x86**: 32-bit systems.

The architecture can be set in Visual Studio using the **Solution Platforms** dropdown.

## Troubleshooting

If you encounter any issues during the build process, refer to the [instructions.md](https://github.com/OpenCPIsland/KeyChainWindows/blob/main/instructions.md) for troubleshooting tips and more detailed guidance.

## Join the Discussion on Discord

You can join the community and get further support or discuss the project on the **Discord server**:

<a href="https://discord.gg/Qpt9Cbukhx">
    <img src="https://logos-world.net/wp-content/uploads/2020/12/Discord-Logo.png" alt="Discord" width="100">
</a>

- **Join here**: [https://discord.gg/Qpt9Cbukhx](https://discord.gg/Qpt9Cbukhx)

## Credits

This project is a **fan-made recreation** of encryption components used for **Club Penguin Island** offline mode. Special thanks to **AllinolCP**, who rewrote the code to make it work and generously shared it.

- **AllinolCP**: [https://github.com/AllinolCP](https://github.com/AllinolCP)

---

If you encounter any issues or have questions, feel free to open an issue in the repository. Contributions and improvements to the code are always welcome!
