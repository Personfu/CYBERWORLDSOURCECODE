# Project Requirements and Setup Instructions

This project is aimed at building a Windows Dynamic Link Library (DLL) for **Club Penguin Island Offline Mode**. Please follow the instructions below to ensure your environment is properly set up for building the DLL.

## 1. Visual Studio 2026

You will need **Visual Studio 2026** to build the DLL. You can download it from the official website:

- [Download Visual Studio 2026](https://visualstudio.microsoft.com/downloads/)

### Required Workloads:

During the installation of Visual Studio 2022, make sure to select the following workloads:

- **Desktop development with C++**: This provides the necessary tools for building DLLs and working with Windows desktop applications.
- Ensure that **v145 build tools** are selected during installation:
  - In the Visual Studio Installer, under **Individual Components**, make sure **MSVC v145 - VS 2026 C++ x64/x86 build tools (Latest)** is selected.

## 2. Latest Windows SDK

The latest version of the **Windows SDK** is required to successfully build the project. The SDK provides headers, libraries, and tools necessary for Windows development.

Download and install the latest version of the Windows SDK from the official Microsoft website:

- [Download Windows SDK](https://developer.microsoft.com/en-us/windows/downloads/windows-sdk/)

Once installed, ensure that Visual Studio is configured to use this SDK by checking the project properties:
- **Right-click on the project in Visual Studio** -> **Properties** -> **General** -> **Windows SDK Version** -> Select the latest SDK version.

## 3. Build Instructions

1. **Clone the repository**:
   - If you haven’t already, clone the repository using Git:
     ```bash
     git clone https://github.com/OpenCPIsland/KeyChainWindows.git
     ```

2. **Open the project in Visual Studio**:
   - Navigate to the folder where the repository is located and open the `.sln` file with Visual Studio.

3. **Ensure the v145 Tools are selected**:
   - Check the project’s properties to ensure it is using the **v143 tools** for the build:
     - Right-click the project in the **Solution Explorer**.
     - Go to **Properties** → **Configuration Properties** → **General** → **Platform Toolset**.
     - Ensure that **Visual Studio 2026 (v145)** is selected.

4. **Set the Build Configuration**:
   - Ensure that you have selected the appropriate build configuration:
     - **Debug**: For development and testing.
     - **Release**: For creating the final DLL for distribution.
   - You can change the build configuration from the **Solution Configurations** dropdown in the Visual Studio toolbar.

5. **Build the DLL**:
   - Select **Build** → **Build Solution** (or press `Ctrl+Shift+B`).
   - The output DLL file will be created in the `Debug` or `Release` folder, depending on your selected configuration.

6. **Place the DLL in the Client Directory**:
   - After building the DLL, locate the outputted file in your build directory.
   - Place the DLL in your custom-built Club Penguin Island client directory:
     - Navigate to your **CP Island_Data/Plugins/x86_64/** folder.
     - Copy the outputted DLL into this folder.

## 4. Target Architecture

Ensure that you are building the DLL for the correct target architecture:

- **x64**: 64-bit systems.
- **x86**: 32-bit systems.

You can select the architecture in Visual Studio using the **Solution Platforms** dropdown in the toolbar.

## 5. Troubleshooting

- **Missing Dependencies**: If the build fails due to missing dependencies, ensure that the required libraries are properly referenced in the project settings or installed on your machine.
- **Incorrect SDK Version**: Make sure the correct Windows SDK version is selected in your project settings if the build is failing due to SDK-related issues.
- **v143 Tools**: If you encounter build errors related to platform toolset, ensure that **v143 build tools** are installed and selected in the project properties.

If you encounter any issues not covered by this document, please open an issue in the repository, and the maintainers will assist you.

---

### Summary

In summary, to build this DLL, ensure you have:
1. Installed **Visual Studio 2026** with the required **Desktop development with C++** workload and **v143 build tools**.
2. Installed the **latest Windows SDK**.
3. Configured the build settings in Visual Studio for the correct architecture and SDK.
4. Successfully built the project using the provided instructions.
5. After the DLL is built, place the outputted DLL into your custom Club Penguin Island client directory **CP Island_Data/Plugins/Windows/** folder.

This document serves as the primary reference for setting up and building the DLL.
