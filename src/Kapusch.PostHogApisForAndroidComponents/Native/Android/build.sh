#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO="$(cd "$ROOT/../../../.." && pwd)"
mkdir -p "$REPO/artifacts/nuget"
SDK="${ANDROID_SDK_ROOT:-${ANDROID_HOME:-$HOME/Library/Android/sdk}}"
[[ -d "$SDK" ]] || { echo 'Android SDK is required' >&2; exit 1; }
export GRADLE_USER_HOME="${GRADLE_USER_HOME:-$ROOT/.gradle/user-home}"
mkdir -p "$ROOT/.gradle" "$ROOT/build/aar" "$ROOT/build/deps"
printf 'sdk.dir=%s\n' "$SDK" > "$ROOT/local.properties"
GRADLE="$ROOT/.gradle/gradle-8.7/bin/gradle"
if [[ ! -x "$GRADLE" ]]; then
 curl -fsSL https://services.gradle.org/distributions/gradle-8.7-bin.zip -o "$ROOT/.gradle/gradle.zip"
 echo '544c35d6bd849ae8a5ed0bcea39ba677dc40f49df7d1835561582da2009b961d  '"$ROOT/.gradle/gradle.zip" | shasum -a 256 -c -
 unzip -q "$ROOT/.gradle/gradle.zip" -d "$ROOT/.gradle"
fi
"$GRADLE" --no-daemon -p "$ROOT" :posthoginterop:assembleRelease :posthoginterop:copyNativeDependencies
cp "$ROOT/posthoginterop/build/outputs/aar/posthoginterop-release.aar" "$ROOT/build/aar/kposthog-release.aar"
python3 "$REPO/scripts/native-integrity.py" "$ROOT/build/deps" "$REPO/DependencyLocks/native-artifacts.json"
