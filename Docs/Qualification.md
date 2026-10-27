# Qualification gates

Build device and simulator/emulator slices. Pack, restore into a clean consumer,
and link an optimized release. Inspect native symbols, bundled licenses and
privacy resources, and scan the public Git tree for private identifiers.

On each platform test anonymous-to-account, logout and A-to-B switches, offline
capture, kill/restart/reconnect, bounds/overflow, opt-out and duplicate business
IDs. Reconcile received events and timestamps on the server. Native disk queues
reduce loss but are not unlimited storage or exactly-once delivery. Cache
purging/uninstall and queue bounds may discard data. No replay claim until tested.

## Local evidence — 2026-10-08

Native compilation and packed NuGet Release sample build passed locally on
.NET 10.0.203. iOS native slices: device arm64 and simulator arm64/x64. Android:
SDK 35, Java 17-compatible bytecode, optimized .NET Android consumer.
This proves package linking only; phone ingestion/restart/identity tests remain
pending. No NuGet.org publication or production readiness is claimed.

Runtime Release sample passed on Android emulator: setup/capture/flush/distinct ID
JNI calls resolved and anonymous ID remained unchanged after force-stop/relaunch.
A loopback endpoint prevents data submission; server delivery and queue recovery
remain separate gates. Clean GitHub Actions native build, pack, package-layout
checks and optimized Release consumer build passed.

## Offline restart recovery prerequisite

The native dependency is pinned to 3.73.1 (core 6.47.7). Upstream 3.36.1 loads cached disk
files into the live queue at startup, and 3.38.0 flushes when connectivity returns.
Earlier 3.22.0 performed a one-time cached-file send that could be skipped at
an offline startup. Disk persistence alone is not a delivery guarantee.
Keep the wrapper thin: no custom queue or calls to internal SDK recovery APIs.
Requalify offline capture, process termination, offline restart and reconnection
on a device before claiming delivery support for this package.

Source: https://github.com/PostHog/posthog-android/blob/main/posthog-android/CHANGELOG.md

## SDK 3.73.1 update — 2026-10-27

Bundled PostHog Android 3.38.2 / core 6.8.1 → 3.73.1 / core 6.47.7, the newest
stable release on Maven Central (no 4.x or newer pre-release line exists).
Evidence: the wrapper compiles with Kotlin 2.1.21, AGP 8.3.2, Gradle 8.7, JDK 17
and compileSdk 35; the AAR declares minCompileSdk=1 and minSdk 23; native SHA256
values match the `.sha256` files published on Maven Central; the packed sample
builds in Release with D8 and with R8 (upstream `proguard.txt` is applied,
including its `-dontwarn` rules for the compile-only Firebase Messaging classes).

Transitive runtime dependencies are unchanged except Okio: core now requires
Okio 3.11.0+ for a gzip request body fix, so the package references managed
`Square.OkIO` 3.16.4.2 (same `Xamarin.Kotlin.StdLib` 2.3.10.1 floor). Upstream
also moved to OkHttp 4.12.1 (blocks compromised intermediate certificates); no
managed 4.12.1 binding exists, so apps run the managed OkHttp 4.12.0 classes.
Push token registration and push-open capture became upstream defaults and
auto-arm when Firebase Messaging is present; the wrapper turns both off.
Device runtime requalification (identity, offline restart, opt-out) is still
required before claiming support for this SDK version.

## Consent runtime check — 2026-10-27

Android 16 (API 36) emulator, packed Release sample linked with R8, loopback
endpoint. When the SDK starts with `OptOut = true`, `IsOptOut` is true and
`Capture` is dropped ("PostHog is in OptOut state."). It makes no remote config
request. After `OptIn()`, `Application Opened` is queued, but
`Application Installed` is not, because the first launch started opted out.
Only `/batch` is contacted. A scratch app using `OptOut = false` and
`PersistOptOut = true` called `OptOut()` and then `Reset()`. The next launch
started opted out and had a new anonymous ID. A later `OptIn()` also persisted
across a relaunch.

## Manual publication

After device qualification, configure a NuGet.org Trusted Publishing policy
for this repository and workflow `publish.yml`, using the NuGet profile `Kapusch`.
The job uses GitHub OIDC through `NuGet/login@v1`; no permanent API key is required
scoped to this package on NuGet.org. Run Publish verified NuGet package on main.
The workflow rebuilds, scans, packs and links a consumer before publishing the
version declared in the project. There are no release tags: the project's
`VersionPrefix`/`VersionSuffix` is the release version, currently
`1.0.0-rc.1` (published 0.1.1 before). Never overwrite a published version;
increment the project and sample versions for subsequent releases, and remove
`VersionSuffix` for the final 1.0.0. No publication runs automatically on push. Device qualification remains a human-reviewed prerequisite.
