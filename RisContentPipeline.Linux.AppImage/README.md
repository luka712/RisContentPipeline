# RisContentPipeline AppImage Builder

This directory contains scripts and resources for building an AppImage package of RisContentPipeline for Linux.

## Prerequisites

Before building the AppImage, ensure you have the following installed:

1. **.NET SDK 8.0 or later**
   ```bash
   # Install .NET SDK if not already installed
   wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
   chmod +x dotnet-install.sh
   ./dotnet-install.sh --channel 8.0
   ```

2. **wget** (for downloading AppImage tools)
   ```bash
   sudo apt-get install wget
   ```

3. **ImageMagick** (optional, for icon resizing)
   ```bash
   sudo apt-get install imagemagick
   ```

## Building the AppImage

1. Navigate to this directory:
   ```bash
   cd RisContentPipeline.Linux.AppImage
   ```

2. Make the build script executable:
   ```bash
   chmod +x build-appimage.sh
   ```

3. Run the build script:
   ```bash
   ./build-appimage.sh
   ```

The script will:
- Build the .NET application in Release mode
- Create the AppDir structure
- Copy all necessary files including the Python runtime
- Generate desktop entries and icons
- Download appimagetool if needed
- Create the final AppImage

## Output

The AppImage will be created in the `output` directory with the name:
```
RisContentPipeline-1.0.0-x86_64.AppImage
```

## Running the AppImage

After building, you can run the AppImage directly:
```bash
./output/RisContentPipeline-1.0.0-x86_64.AppImage
```

## Distribution

The AppImage is a portable application that can run on most Linux distributions without installation. Users just need to:

1. Download the AppImage file
2. Make it executable: `chmod +x RisContentPipeline-*.AppImage`
3. Run it: `./RisContentPipeline-*.AppImage`

## Troubleshooting

### Missing Dependencies
If the AppImage fails to run due to missing libraries, you may need to install:
```bash
# GTK dependencies for Eto.Forms
sudo apt-get install libgtk-3-0 libgdk-pixbuf2.0-0

# Additional dependencies
sudo apt-get install libssl1.1 libicu66
```

### FUSE Issues
On newer systems, you might need to extract and run the AppImage:
```bash
./RisContentPipeline-*.AppImage --appimage-extract
./squashfs-root/AppRun
```

Or install FUSE:
```bash
sudo apt-get install libfuse2
```

### Python Runtime
The build script automatically includes the Python 3.11.8 runtime from the Linux project directory. Ensure the `python-3.11.8` folder exists in `RisContentPipeline.GUI.Linux/` before building.

## Customization

You can modify the following in `build-appimage.sh`:
- `APP_VERSION`: Change the version number
- Desktop entry metadata in the `.desktop` file
- Icon sizes and locations
- Additional libraries or resources

## Clean Build

To perform a clean build, the script automatically removes previous build artifacts. You can also manually clean using the provided script:
```bash
./clean.sh
```

Or manually:
```bash
rm -rf build/ output/ releases/
```

## License

This build script is part of the RisContentPipeline project and follows the same license terms.