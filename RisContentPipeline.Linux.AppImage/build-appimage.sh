#!/bin/bash

# Build script for creating RisContentPipeline AppImage
set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${GREEN}Building RisContentPipeline AppImage...${NC}"

# Configuration
APP_NAME="RisContentPipeline"
APP_VERSION="0.1.0"
ARCH=$(uname -m)
BUILD_DIR="$(pwd)/build"
APPDIR="$BUILD_DIR/$APP_NAME.AppDir"
OUTPUT_DIR="$(pwd)/output"

# Clean previous builds
echo -e "${YELLOW}Cleaning previous builds...${NC}"
rm -rf "$BUILD_DIR"
rm -rf "$OUTPUT_DIR"
mkdir -p "$BUILD_DIR"
mkdir -p "$OUTPUT_DIR"

# Build the .NET application
echo -e "${YELLOW}Building .NET application...${NC}"
cd ../RisContentPipeline.GUI.Linux
dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=false -o "$BUILD_DIR/publish"

# Create AppDir structure
echo -e "${YELLOW}Creating AppDir structure...${NC}"
mkdir -p "$APPDIR"
mkdir -p "$APPDIR/usr/bin"
mkdir -p "$APPDIR/usr/lib"
mkdir -p "$APPDIR/usr/share/applications"
mkdir -p "$APPDIR/usr/share/icons/hicolor/256x256/apps"
mkdir -p "$APPDIR/usr/share/icons/hicolor/128x128/apps"
mkdir -p "$APPDIR/usr/share/icons/hicolor/64x64/apps"
mkdir -p "$APPDIR/usr/share/icons/hicolor/32x32/apps"
mkdir -p "$APPDIR/usr/share/icons/hicolor/16x16/apps"

# Copy application files
echo -e "${YELLOW}Copying application files...${NC}"
cp -r "$BUILD_DIR/publish/"* "$APPDIR/usr/lib/"

# Copy Python runtime if it exists
if [ -d "python-3.11.8" ]; then
    echo -e "${YELLOW}Copying Python runtime...${NC}"
    cp -r python-3.11.8 "$APPDIR/usr/lib/"
fi

# Create the main executable wrapper
echo -e "${YELLOW}Creating executable wrapper...${NC}"
cat > "$APPDIR/usr/bin/riscontentpipeline" << 'EOF'
#!/bin/bash
APPDIR="$(dirname "$(dirname "$(readlink -f "$0")")")"
export LD_LIBRARY_PATH="$APPDIR/usr/lib:$APPDIR/usr/lib/python-3.11.8/lib:$LD_LIBRARY_PATH"
export PYTHONHOME="$APPDIR/usr/lib/python-3.11.8"
export PYTHONPATH="$APPDIR/usr/lib/python-3.11.8/lib/python3.11:$PYTHONPATH"
cd "$APPDIR/usr/lib"
exec "$APPDIR/usr/lib/RisContentPipeline.GUI.Linux" "$@"
EOF
chmod +x "$APPDIR/usr/bin/riscontentpipeline"

# Copy and convert icon
echo -e "${YELLOW}Setting up icons...${NC}"
if [ -f "Resources/Icon.png" ]; then
    cp "Resources/Icon.png" "$APPDIR/usr/share/icons/hicolor/256x256/apps/riscontentpipeline.png"
    
    # Create different icon sizes using ImageMagick if available
    if command -v convert &> /dev/null; then
        convert "Resources/Icon.png" -resize 128x128 "$APPDIR/usr/share/icons/hicolor/128x128/apps/riscontentpipeline.png"
        convert "Resources/Icon.png" -resize 64x64 "$APPDIR/usr/share/icons/hicolor/64x64/apps/riscontentpipeline.png"
        convert "Resources/Icon.png" -resize 32x32 "$APPDIR/usr/share/icons/hicolor/32x32/apps/riscontentpipeline.png"
        convert "Resources/Icon.png" -resize 16x16 "$APPDIR/usr/share/icons/hicolor/16x16/apps/riscontentpipeline.png"
    else
        # Copy the same icon to all sizes if ImageMagick is not available
        cp "Resources/Icon.png" "$APPDIR/usr/share/icons/hicolor/128x128/apps/riscontentpipeline.png"
        cp "Resources/Icon.png" "$APPDIR/usr/share/icons/hicolor/64x64/apps/riscontentpipeline.png"
        cp "Resources/Icon.png" "$APPDIR/usr/share/icons/hicolor/32x32/apps/riscontentpipeline.png"
        cp "Resources/Icon.png" "$APPDIR/usr/share/icons/hicolor/16x16/apps/riscontentpipeline.png"
    fi
    
    # Copy icon to root for AppImage
    cp "Resources/Icon.png" "$APPDIR/riscontentpipeline.png"
else
    echo -e "${RED}Warning: Icon.png not found${NC}"
fi

# Create desktop entry
echo -e "${YELLOW}Creating desktop entry...${NC}"
cat > "$APPDIR/usr/share/applications/riscontentpipeline.desktop" << EOF
[Desktop Entry]
Type=Application
Name=Ris Content Pipeline
Comment=Content pipeline tool for processing and converting assets
Exec=riscontentpipeline %F
Icon=riscontentpipeline
Categories=Development;Graphics;
Terminal=false
StartupNotify=true
MimeType=image/png;image/jpeg;image/bmp;image/gif;
EOF

# Copy desktop entry to root for AppImage
cp "$APPDIR/usr/share/applications/riscontentpipeline.desktop" "$APPDIR/riscontentpipeline.desktop"

# Create AppRun script
echo -e "${YELLOW}Creating AppRun script...${NC}"
cat > "$APPDIR/AppRun" << 'EOF'
#!/bin/bash
HERE="$(dirname "$(readlink -f "${0}")")"
export LD_LIBRARY_PATH="${HERE}/usr/lib:${HERE}/usr/lib/python-3.11.8/lib:${LD_LIBRARY_PATH}"
export PYTHONHOME="${HERE}/usr/lib/python-3.11.8"
export PYTHONPATH="${HERE}/usr/lib/python-3.11.8/lib/python3.11:${PYTHONPATH}"
cd "${HERE}/usr/lib"
exec "${HERE}/usr/lib/RisContentPipeline.GUI.Linux" "$@"
EOF
chmod +x "$APPDIR/AppRun"

# Download appimagetool if not present
echo -e "${YELLOW}Downloading appimagetool...${NC}"
if [ ! -f "$BUILD_DIR/appimagetool-x86_64.AppImage" ]; then
    wget -q --show-progress "https://github.com/AppImage/AppImageKit/releases/download/continuous/appimagetool-x86_64.AppImage" -O "$BUILD_DIR/appimagetool-x86_64.AppImage"
    chmod +x "$BUILD_DIR/appimagetool-x86_64.AppImage"
fi

# Create the AppImage
echo -e "${YELLOW}Creating AppImage...${NC}"
cd "$BUILD_DIR"
ARCH=$ARCH ./appimagetool-x86_64.AppImage "$APPDIR" "$OUTPUT_DIR/$APP_NAME-$APP_VERSION-$ARCH.AppImage"

# Make the AppImage executable
chmod +x "$OUTPUT_DIR/$APP_NAME-$APP_VERSION-$ARCH.AppImage"

echo -e "${GREEN}AppImage created successfully!${NC}"
echo -e "${GREEN}Output: $OUTPUT_DIR/$APP_NAME-$APP_VERSION-$ARCH.AppImage${NC}"

# Optional: Test the AppImage
echo -e "${YELLOW}You can test the AppImage by running:${NC}"
echo "$OUTPUT_DIR/$APP_NAME-$APP_VERSION-$ARCH.AppImage"
