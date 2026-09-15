#!/usr/bin/env bash
set -euo pipefail
export AVALONIA_TELEMETRY_OPTOUT=1

runtime="${1:-osx-arm64}"
if [[ "$runtime" != "osx-arm64" && "$runtime" != "osx-x64" ]]; then
  echo "사용법: ./Scripts/publish-macos.sh [osx-arm64|osx-x64]" >&2
  exit 1
fi

script_dir="$(cd "$(dirname "$0")" && pwd)"
project_dir="$(cd "$script_dir/.." && pwd)"
publish_dir="$project_dir/artifacts/$runtime/publish"
app_dir="$project_dir/artifacts/$runtime/SKCTPractice.app"

dotnet publish "$project_dir/SKCTPractice.csproj" \
  -c Release \
  -r "$runtime" \
  --self-contained true \
  -p:PublishSingleFile=true \
  -p:IncludeNativeLibrariesForSelfExtract=true \
  -o "$publish_dir"

rm -rf "$app_dir"
mkdir -p "$app_dir/Contents/MacOS" "$app_dir/Contents/Resources"
cp "$publish_dir/SKCTPractice" "$app_dir/Contents/MacOS/SKCTPractice"
chmod +x "$app_dir/Contents/MacOS/SKCTPractice"

cat > "$app_dir/Contents/Info.plist" <<'PLIST'
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
  <key>CFBundleName</key><string>SKCT Practice</string>
  <key>CFBundleDisplayName</key><string>SKCT Practice</string>
  <key>CFBundleIdentifier</key><string>com.local.skctpractice</string>
  <key>CFBundleExecutable</key><string>SKCTPractice</string>
  <key>CFBundlePackageType</key><string>APPL</string>
  <key>CFBundleShortVersionString</key><string>1.0.0</string>
  <key>CFBundleVersion</key><string>1</string>
  <key>NSHighResolutionCapable</key><true/>
</dict>
</plist>
PLIST

codesign --force --deep --sign - "$app_dir" 2>/dev/null || true
echo "완료: $app_dir"
