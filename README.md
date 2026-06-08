# RisContentPipeline
The pipeline for packing different asset into game compatible assets.

## Projects
- `RisContentPipeline` - core logic for reading image formats and converting them to KTX2.
- `RisContentPipeline.GUI` - Eto.Forms desktop UI for converting files.
- `RisContentPipeline.GUI.Windows` - Windows-specific entry point (WPF/Eto).
- `RisContentPipeline.GUI.Linux` - Linux-specific entry point (Gtk/Eto).
- `RisContentPipeline.Windows.Installer` - WiX 3.x MSI installer for Windows.
- `RisContentPipeline.Tests` - tests for the core logic.

---

## Building the Windows Installer

### Prerequisites

| Tool | Version | Download |
|------|---------|----------|
| .NET SDK | 10.0+ | https://dotnet.microsoft.com/download |
| WiX Toolset | 3.11 | https://wixtoolset.org/releases/v3.11/stable |

### Quick build (recommended)

Open a **Developer Command Prompt** or any terminal with `dotnet` in PATH, then run:

```cmd
cd RisContentPipeline.Windows.Installer
build-installer.cmd
```

The script will:
1. `dotnet publish` the Windows GUI app as a self-contained win-x64 build into `publish\`
2. Run `heat.exe` to harvest all published files into `HarvestedComponents.wxs`
3. Run `candle.exe` to compile `Product.wxs` + `HarvestedComponents.wxs` into `.wixobj` files
4. Run `light.exe` to link everything into `bin\Release\RisContentPipeline-1.0.0-windows-x64.msi`

A `build.log` file is written alongside the script for diagnostics.

### Building via MSBuild / Rider

The `.wixproj` is configured to run the same steps automatically when you build the
`RisContentPipeline.Windows.Installer` project from Rider or MSBuild:

```cmd
msbuild RisContentPipeline.Windows.Installer\RisContentPipeline.Windows.Installer.wixproj /p:Configuration=Release /p:Platform=x86
```

> **Note:** WiX Toolset v3.11 must be installed for the MSBuild integration to work.

### Output

```
RisContentPipeline.Windows.Installer\bin\Release\RisContentPipeline-1.0.0-windows-x64.msi
```

The installer:
- Installs to `%ProgramFiles%\RisContentPipeline\` by default (user-selectable)
- Creates a **Desktop shortcut** and a **Start Menu** folder with launch + uninstall shortcuts
- Bundles the self-contained .NET 10 runtime (no separate .NET install required)
- Supports clean uninstall via Add/Remove Programs
