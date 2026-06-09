#!/bin/bash

# Clean script for RisContentPipeline AppImage build artifacts

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${YELLOW}Cleaning AppImage build artifacts...${NC}"

# Remove build directories
if [ -d "build" ]; then
    echo "Removing build directory..."
    rm -rf build/
fi

if [ -d "output" ]; then
    echo "Removing output directory..."
    rm -rf output/
fi

if [ -d "releases" ]; then
    echo "Removing releases directory..."
    rm -rf releases/
fi

# Remove any extracted AppImage directories
if [ -d "squashfs-root" ]; then
    echo "Removing squashfs-root directory..."
    rm -rf squashfs-root/
fi

# Remove any AppImage files in the current directory
if ls *.AppImage 1> /dev/null 2>&1; then
    echo "Removing AppImage files..."
    rm -f *.AppImage
fi

# Remove log files
if ls *.log 1> /dev/null 2>&1; then
    echo "Removing log files..."
    rm -f *.log
fi

echo -e "${GREEN}Clean complete!${NC}"
echo "The following files remain:"
ls -la