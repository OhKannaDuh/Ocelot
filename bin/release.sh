#!/usr/bin/env bash
set -euo pipefail

if [[ -f .env ]]; then
  export $(grep -v '^#' .env | xargs)
fi
: "${NUGET_API_KEY:?NUGET_API_KEY must be set in .env or environment}"

NUGET_SOURCE="https://api.nuget.org/v3/index.json"
NUGET_FLAT="https://api.nuget.org/v3-flatcontainer"
CONFIGURATION="Release"
ARTIFACTS_DIR="artifacts"
DRY_RUN=false
BUMP_TYPE=""   # major / minor / patch

usage() {
  cat <<EOF
Usage: $0 [--major|--minor|--patch] [--dry-run] [--ui [VERSION]] [--pathfinding [VERSION]] ...

Examples:
  # Explicit version (old behaviour)
  $0 --ui 0.5.0

  # Calculate from latest NuGet version (e.g. 0.4.5 -> 0.5.0)
  $0 --minor --ui

  # Bump several in one go
  $0 --patch --ui --pathfinding --rotation

Notes:
  - If you omit VERSION for a module, you MUST specify one of --major/--minor/--patch.
  - If a package has never been published and you use a bump flag, the script will error and ask for an explicit version.
EOF
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

latest_nuget_version() {
  local pkgid="$1"
  local id_lower
  id_lower=$(echo "$pkgid" | tr '[:upper:]' '[:lower:]')

  local url="$NUGET_FLAT/$id_lower/index.json"
  local json

  if ! json=$(curl -fsS "$url" 2>/dev/null); then
    echo ""
    return
  fi

  # use last element of versions array (supports older jq via "last")
  local ver
  ver=$(
    printf '%s\n' "$json" \
      | jq -r '.versions | last' 2>/dev/null \
      || echo ""
  )
  echo "$ver"
}

bump_version() {
  local current="$1"
  local kind="$2"

  if [[ ! "$current" =~ ^([0-9]+)\.([0-9]+)\.([0-9]+)$ ]]; then
    echo "❌ Cannot bump non-semver version: $current" >&2
    exit 1
  fi

  local major="${BASH_REMATCH[1]}"
  local minor="${BASH_REMATCH[2]}"
  local patch="${BASH_REMATCH[3]}"

  case "$kind" in
    major)
      major=$((major + 1))
      minor=0
      patch=0
      ;;
    minor)
      minor=$((minor + 1))
      patch=0
      ;;
    patch)
      patch=$((patch + 1))
      ;;
    *)
      echo "❌ Unknown bump type: $kind" >&2
      exit 1
      ;;
  esac

  echo "$major.$minor.$patch"
}

# Parse args
declare -A TARGETS   # module -> version (may be empty, meaning "compute from NuGet")

while [[ $# -gt 0 ]]; do
  case "$1" in
    --dry-run)
      DRY_RUN=true
      shift
      ;;
    --major|--minor|--patch)
      if [[ -n "$BUMP_TYPE" ]]; then
        echo "❌ Only one of --major/--minor/--patch may be specified."
        usage
      fi
      BUMP_TYPE="${1#--}"
      shift
      ;;
    --*)
      mod="${1#--}"
      if [[ -z "${PROJECTS[$mod]:-}" ]]; then
        echo "❌ Unknown module: $mod"
        usage
      fi

      # Look ahead: if next arg exists and is not another flag, treat it as explicit version
      if [[ $# -ge 2 && ! "$2" =~ ^-- ]]; then
        TARGETS["$mod"]="$2"
        shift 2
      else
        TARGETS["$mod"]=""   # mark for computed version
        shift
      fi
      ;;
    *)
      usage
      ;;
  esac
done

[[ ${#TARGETS[@]} -eq 0 ]] && usage

# If any module needs computed version, require a bump type
for mod in "${!TARGETS[@]}"; do
  if [[ -z "${TARGETS[$mod]}" && -z "$BUMP_TYPE" ]]; then
    echo "❌ No version provided for '$mod' and no --major/--minor/--patch specified."
    usage
  fi
done

rm -rf "$ARTIFACTS_DIR"
mkdir -p "$ARTIFACTS_DIR"

for mod in "${!TARGETS[@]}"; do
  csproj="${PROJECTS[$mod]}"
  version="${TARGETS[$mod]}"

  if [[ ! -f "$csproj" ]]; then
    echo "❌ Missing project file: $csproj"
    exit 1
  fi

  #   pkgid=$(xmllint --xpath "string(//PackageId)" "$csproj" 2>/dev/null || true)
  pkgid=$(dotnet msbuild "$csproj" -nologo -getProperty:PackageId 2>/dev/null | tail -n 1 || true)
  if [[ -z "$pkgid" ]]; then
    pkgid=$(basename "$csproj" .csproj)
  fi

  # Compute version from NuGet + bump if needed
  if [[ -z "$version" ]]; then
    current=$(latest_nuget_version "$pkgid")
    if [[ -z "$current" ]]; then
      echo "❌ Package $pkgid has no published version; please specify an explicit version for $mod."
      exit 1
    fi
    version=$(bump_version "$current" "$BUMP_TYPE")
    echo "ℹ️  $pkgid latest on NuGet: $current; bumping $BUMP_TYPE -> $version"
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
