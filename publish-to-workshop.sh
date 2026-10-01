#!/usr/bin/env bash
set -euo pipefail

project_dir="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
release_dir="$project_dir/BepinexRelease"
modinfo="$release_dir/modinfo.json"
published_file_id="$(jq -er '.PublishedFileId | select(type == "number")' "$modinfo")"
workshop_dir="/run/media/system/990-EP1-LS/SteamLibrary/steamapps/workshop/content/1140150/$published_file_id"
scriptengine_scripts_dir="/run/media/system/990-EP1-LS/SteamLibrary/steamapps/workshop/content/1140150/3483706823/BepInEx/scripts"

if [[ ! -d "$workshop_dir" ]]; then
    printf 'Workshop destination does not exist: %s\n' "$workshop_dir" >&2
    exit 1
fi

dotnet publish "$project_dir/DreamStartOfTurnMod_TopDeck.csproj" \
    /property:GenerateFullPaths=true \
    '/consoleloggerparameters:NoSummary;ForceNoAlign'

timestamp="$(date +%s)"
jq --argjson timestamp "$timestamp" '.LastUpdateTime = $timestamp' "$modinfo" > "$modinfo.tmp"
mv "$modinfo.tmp" "$modinfo"
rsync -a "$release_dir/" "$workshop_dir/"
find "$scriptengine_scripts_dir" -mindepth 1 -maxdepth 1 -exec rm -rf -- {} +

printf 'Synced release files to %s (LastUpdateTime: %s) and cleared %s\n' \
    "$workshop_dir" "$timestamp" "$scriptengine_scripts_dir"
