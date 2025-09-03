#!/bin/bash
set -e

echo "📥 Fetching the latest release from MollyIO/PlayerMetrics..."
LATEST_RELEASE=$(curl -s https://api.github.com/repos/MollyIO/PlayerMetrics/releases/latest)

TAG=$(echo $LATEST_RELEASE | grep -oP '"tag_name": "\K(.*)(?=")')
echo "🔖 Latest version: $TAG"

PLUGIN_URL=$(echo $LATEST_RELEASE | grep -oP '"browser_download_url": "\K(.*PlayerMetrics\.dll)(?=")')
DEPS_URL=$(echo $LATEST_RELEASE | grep -oP '"browser_download_url": "\K(.*dependencies\.zip)(?=")')

read -p "Enter the port to install the plugin (leave empty for global): " USER_PORT
if [[ -z "$USER_PORT" || ! "$USER_PORT" =~ ^[0-9]+$ ]]; then
    USER_PORT="global"
fi

LAB_API_DIR="$HOME/.config/SCP Secret Laboratory/LabAPI"
INSTALL_DIR="$LAB_API_DIR/plugins/$USER_PORT"
DEPS_DIR="$LAB_API_DIR/dependencies/$USER_PORT"

mkdir -p "$INSTALL_DIR" "$DEPS_DIR"

echo "📦 Downloading plugin..."
curl -L "$PLUGIN_URL" -o "$INSTALL_DIR/PlayerMetrics.dll"

if [ -n "$DEPS_URL" ]; then
  echo "📦 Downloading dependencies..."
  curl -L "$DEPS_URL" -o "$DEPS_DIR/dependencies.zip"
  unzip -o "$DEPS_DIR/dependencies.zip" -d "$DEPS_DIR"
  rm "$DEPS_DIR/dependencies.zip"
fi

echo "✅ Installation complete!"
