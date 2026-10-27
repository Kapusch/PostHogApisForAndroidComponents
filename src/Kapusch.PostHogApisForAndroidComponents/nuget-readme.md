# Kapusch.PostHog.Android

Thin JNI interop for PostHog Android 3.73.1 (core 6.47.7). Upstream owns disk
persistence, identity and lifecycle. Configure once, reset on account changes.
Capture is enabled by default; start opted out with
`NativePostHogOptions.OptOut = true` and call `OptIn()` after consent. An active
opt-out survives `Reset()`; a persisted opt-in does not, so keep your own consent
record and call `OptIn()` again after every `Reset()` (or on every launch).
Queued events are not purged by `OptOut()`. Flush is not a delivery receipt.
Replay is disabled and not qualified.
