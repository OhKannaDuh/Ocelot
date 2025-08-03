#!/bin/bash

set -e

# Usage check
if [ $# -ne 2 ]; then
  echo "Usage: ./release.sh <api_key> <version>"
  exit 1
fi

API_KEY="$1"
VERSION="$2"
PROJECT_NAME="FFXIVOcelot"
CSPROJ_PATH="Ocelot/Ocelot.csproj"
PLUGIN_CLASS_PATH="Ocelot/OcelotPlugin.cs"
OUTPUT_DIR="Ocelot/bin/x64/Release"
NUGET_SOURCE="https://api.nuget.org/v3/index.json"
NUPKG_PATH="$OUTPUT_DIR/$PROJECT_NAME.$VERSION.nupkg"

# Ensure version in csproj matches the input version
echo "🔍 Ensuring version $VERSION is set in $CSPROJ_PATH..."
if grep -q "<Version>$VERSION</Version>" "$CSPROJ_PATH"; then
  echo "✅ Version already correct in .csproj"
else
  echo "✏️ Updating version in .csproj..."
  # Replace existing <Version>...</Version> or add if missing
  if grep -q "<Version>.*</Version>" "$CSPROJ_PATH"; then
    sed -i "s|<Version>.*</Version>|<Version>$VERSION</Version>|" "$CSPROJ_PATH"
    git add $CSPROJ_PATH
    git commit -m"Version: $VERSION"
  fi
fi

# Ensure OcelotVersion matches the input version
echo "🔍 Ensuring OcelotVersion is \"$VERSION\" in $PLUGIN_CLASS_PATH..."
if grep -q "public const string OcelotVersion = \"$VERSION\";" "$PLUGIN_CLASS_PATH"; then
  echo "✅ OcelotVersion already correct"
else
  echo "✏️ Updating OcelotVersion in source..."
  sed -i "s|public const string OcelotVersion = \".*\";|public const string OcelotVersion = \"$VERSION\";|" "$PLUGIN_CLASS_PATH"
  git add $PLUGIN_CLASS_PATH
  git commit -m"OcelotVersion: $VERSION"
fi

echo "🔧 Building project..."
dotnet build -c Release

echo "📦 Packing version $VERSION..."
dotnet pack -c Release

echo "🚀 Pushing $NUPKG_PATH to NuGet..."
dotnet nuget push "$NUPKG_PATH" -k "$API_KEY" -s "$NUGET_SOURCE"

echo "✅ Publish complete!"

git tag "$VERSION"
git push origin master "$VERSION"
  
echo "✅ Git tag complete!"
