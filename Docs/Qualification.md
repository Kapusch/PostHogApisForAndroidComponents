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

The native dependency is pinned to 3.38.2. Upstream 3.36.1 loads cached disk
files into the live queue at startup, and 3.38.0 flushes when connectivity returns.
Earlier 3.22.0 performed a one-time cached-file send that could be skipped at
an offline startup. Disk persistence alone is not a delivery guarantee.
Keep the wrapper thin: no custom queue or calls to internal SDK recovery APIs.
Requalify offline capture, process termination, offline restart and reconnection
on a device before claiming delivery support for this package.

Source: https://github.com/PostHog/posthog-android/blob/main/posthog-android/CHANGELOG.md

## Manual publication

After device qualification, configure repository Actions secret NUGET_API_KEY
scoped to this package on NuGet.org. Run Publish verified NuGet package on main.
The workflow rebuilds, scans, packs and links a consumer before publishing the
version declared in the project. Never overwrite a published version; increment
the project and sample versions for subsequent releases. No publication runs
automatically on push. Device qualification remains a human-reviewed prerequisite.
