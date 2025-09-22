#!/usr/bin/env bash
set -euo pipefail

if [[ -f .env ]]; then
  export $(grep -v '^#' .env | xargs)
fi
: "${NUGET_API_KEY:?NUGET_API_KEY must be set in .env or environment}"

NUGET_SOURCE="https://api.nuget.org/v3/index.json"
CONFIGURATION="Release"
ARTIFACTS_DIR="artifacts"
DRY_RUN=false

usage() {
  echo "Usage: $0 [--ui VERSION] [--pathfinding VERSION] [--chain VERSION] ... [--dry-run]"
  echo "Examples:"
  echo "  $0 --ui 0.1.12 --dry-run"
  echo "  $0 --pathfinding 1.2.3"
  exit 1
}

declare -A PROJECTS=(
  [ui]="Ocelot.UI/Ocelot.UI.csproj"
  [pathfinding]="Ocelot.Pathfinding/Ocelot.Pathfinding.csproj"
  [chain]="Ocelot.Chain/Ocelot.Chain.csproj"
  [ecommons]="Ocelot.ECommons/Ocelot.ECommons.csproj"
  [mechanic]="Ocelot.Mechanic/Ocelot.Mechanic.csproj"
  [pictomancy]="Ocelot.Pictomancy/Ocelot.Pictomancy.csproj"
  [rotation]="Ocelot.Rotation/Ocelot.Rotation.csproj"
  [core]="Ocelot/Ocelot.csproj"
)

# Parse args
declare -A TARGETS
while [[ $# -gt 0 ]]; do
  case "$1" in
    --dry-run) DRY_RUN=true; shift ;;
    --*) 
      mod="${1#--}"
      shift
      [[ $# -eq 0 ]] && usage
      ver="$1"; shift
      if [[ -z "${PROJECTS[$mod]:-}" ]]; then
        echo "❌ Unknown module: $mod"
        usage
      fi
      TARGETS["$mod"]="$ver"
      ;;
    *) usage ;;
  esac
done

[[ ${#TARGETS[@]} -eq 0 ]] && usage

rm -rf "$ARTIFACTS_DIR"
mkdir -p "$ARTIFACTS_DIR"

for mod in "${!TARGETS[@]}"; do
  csproj="${PROJECTS[$mod]}"
  version="${TARGETS[$mod]}"
  if [[ ! -f "$csproj" ]]; then
    echo "❌ Missing project file: $csproj"
    exit 1
  fi

  pkgid=$(xmllint --xpath "string(//PackageId)" "$csproj" 2>/dev/null || true)

  if [[ -z "$pkgid" ]]; then
    pkgid=$(basename "$csproj" .csproj)
  fi

  echo "🔧 Building $pkgid ($version)…"
  dotnet restore "$csproj"
  dotnet build "$csproj" -c "$CONFIGURATION" --no-restore

  echo "📦 Packing $pkgid $version…"
  dotnet pack "$csproj" \
    -c "$CONFIGURATION" \
    -o "$ARTIFACTS_DIR" \
    -p:PackageVersion="$version" \
    -p:Version="$version" \
    --no-build

  nupkg="$ARTIFACTS_DIR/$pkgid.$version.nupkg"

  if $DRY_RUN; then
    echo "🚫 Dry run: built $nupkg but not pushing."
  else
    echo "🚀 Pushing $pkgid $version…"
    dotnet nuget push "$nupkg" -k "$NUGET_API_KEY" -s "$NUGET_SOURCE" --skip-duplicate

    tag="pkg/$pkgid/v$version"
    if git rev-parse -q --verify "refs/tags/$tag" >/dev/null; then
      echo "🏷️  Tag $tag already exists; skipping."
    else
      git tag "$tag"
      git push origin "$tag"
    fi
  fi
done

echo "✅ Done!"
