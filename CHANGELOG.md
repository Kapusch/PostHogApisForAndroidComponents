# Changelog

## 1.0.0-rc.1

Native SDK: PostHog Android 3.73.1 / core 6.47.7 (was 3.38.2 / 6.8.1).

### Breaking or behavior changes

- **Minimum Android version is API 23** (Android 6.0), up from API 21,
  following the upstream SDK. An app whose `SupportedOSPlatformVersion` is
  below 23 fails at build time with an explicit error.
- **`Reset()` keeps an active opt-out across launches.** Upstream `reset()`
  erases the persisted opt-out; the wrapper re-applies it. A persisted opt-in is
  still erased: apps that start opted out must call `OptIn()` again after every
  `Reset()` (see [consent](Docs/Consent.md)).
- **No remote config request at setup.** 0.1.x fetched the project's remote
  config at startup, even while opted out. The wrapper now turns remote config
  off; it only drives features this package does not expose.
- **New direct dependency `Square.OkIO` 3.16.4.2**, required by core 6.47.x
  (Okio 3.11.0+).

### Unchanged

- Capture stays enabled by default (upstream default, `OptOut = false`).
- The `Configure(context, key, host, debug, captureLifecycle)` overload is kept.
- Push token registration and push-open capture stay off. Upstream added both
  (on by default); the wrapper neither exposes nor qualifies them.

### Added

- `NativePostHogOptions` (`Debug`, `CaptureLifecycle`, `OptOut`,
  `PersistOptOut`) and a `Configure` overload that takes it.
- `NativePostHog.IsOptOut`.
- [Consent and opt-out](Docs/Consent.md) documentation.
