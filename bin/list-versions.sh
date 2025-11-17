#!/usr/bin/env bash
set -euo pipefail

NUGET_INDEX="https://api.nuget.org/v3-flatcontainer"

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

echo "📦 NuGet Versions"
echo "-----------------"

for mod in "${!PROJECTS[@]}"; do
  csproj="${PROJECTS[$mod]}"

  if [[ ! -f "$csproj" ]]; then
    echo "❌ $mod: missing project file"
    continue
  fi

  pkgid=$(xmllint --xpath "string(//PackageId)" "$csproj" 2>/dev/null || true)
  [[ -z "$pkgid" ]] && pkgid=$(basename "$csproj" .csproj)

  # Lowercase safely
  pkgid_lower=$(echo "$pkgid" | tr '[:upper:]' '[:lower:]')

  # Build URL
  url="$NUGET_INDEX/$pkgid_lower/index.json"

  # Query NuGet
  json=$(curl -fsS "$url" 2>/dev/null || true)

  if [[ -z "$json" ]]; then
    version="(not published)"
  else
    if command -v jq >/dev/null 2>&1; then
        version=$(printf '%s' "$json" | jq -r '.versions[-1] // empty')
    else
        if command -v python3 >/dev/null 2>&1; then
            version=$(python3 -c 'import sys,json; j=json.load(sys.stdin); v=j.get("versions",[]); print(v[-1] if v else "")' <<<"$json" 2>/dev/null || true)
        elif command -v python >/dev/null 2>&1; then
            version=$(python -c 'import sys,json; j=json.load(sys.stdin); v=j.get("versions",[]); print(v[-1] if v else "")' <<<"$json" 2>/dev/null || true)
        else
            version=""
        fi
    fi

    [[ -z "${version:-}" ]] && version="(no versions)"
  fi

  printf "%-12s %-30s %s\n" "$mod" "$pkgid" "$version"
done
