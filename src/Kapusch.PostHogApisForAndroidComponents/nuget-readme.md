# Kapusch.PostHog.Android

Thin JNI interop for PostHog Android 3.73.1 (core 6.47.7). Upstream owns disk
persistence, identity and lifecycle. Configure once, reset on account changes.
Capture is enabled by default; start opted out with
`NativePostHogOptions.OptOut = true` and call `OptIn()` after consent. An active
opt-out survives `Reset()`; queued events are not purged by `OptOut()`. Flush is
not a delivery receipt. Replay is disabled and not qualified.
