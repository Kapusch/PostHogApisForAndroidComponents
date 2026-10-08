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
