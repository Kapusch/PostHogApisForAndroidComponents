# Consent and opt-out

Verified against the upstream source of the bundled SDK, PostHog Android 3.73.1
/ core 6.47.7 (tag `android-v3.73.1`). Re-verify this page on every SDK bump.

## API

| Call | Effect |
| --- | --- |
| `Configure(context, key, host, new NativePostHogOptions { OptOut = true })` | Starts the SDK opted out. |
| `NativePostHogOptions.PersistOptOut` (default `true`) | Whether the SDK stores the runtime choice and restores it at the next setup. |
| `OptIn()` / `OptOut()` | Changes consent at runtime. Persisted unless `PersistOptOut = false`. |
| `IsOptOut` | `true` while capture is disabled, and before `Configure`. |

The existing `Configure(context, key, host, debug, captureLifecycle)` overload
is kept and uses the defaults.

**Default.** `OptOut` defaults to `false`, the upstream default. Capture is
enabled at setup, so apps upgrading from 0.1.x see no behavior change. Whether
prior consent is legally required is the app's decision. Apps that need it must
pass `OptOut = true`.

## First launch, before any other call

These always happen at `Configure`, whether the SDK is opted out or not:

- It creates the private SharedPreferences file `posthog-android-<project key>`,
  then generates and stores an anonymous ID (UUIDv7) and a device ID equal to
  that first anonymous ID. The device ID survives `Reset()` and is sent with
  every feature flag request, so `Reset()` does not fully unlink the device. In
  Direct Boot, before the first unlock, both stay in memory until storage can be
  read.
- It records the app version name and code, which are used to classify later
  launches as installs or updates.
- It starts a session ID in memory and the event queue. The queue flushes every
  5 s or at 20 events. It loads events that a previous launch left in
  `cacheDir/posthog-disk-queue` and sends them. **The queue is not gated by
  opt-out.**
- It makes no remote config or feature flag request at setup: the wrapper turns
  off remote config and flag preloading. Replay, surveys, push, screen views,
  deep links and exception autocapture are also off. When the SDK is opted out
  and the disk queue is empty, `Configure` makes no network request.
  Upstream deprecates the remote config switch as "always enabled", but
  3.73.1 still honors it at setup. Check it again on every bump.

When the SDK is **not** opted out and `CaptureLifecycle` is `true` (the
default), it queues these events:

- `Application Installed` on the first launch, or `Application Updated` when
  the version code has changed.
- `Application Opened` when the process comes to the foreground, and
  `Application Backgrounded` when it leaves.

Events carry the anonymous distinct ID, the session ID, and device, OS, app and
network context properties. They are queued in memory and on disk, and sent in
batches.

When the SDK **is** opted out, `Capture` and `Identify` events are dropped, and
the lifecycle and install integrations are not installed. The version is still
recorded.

## Start disabled, enable after consent

```csharp
NativePostHog.Configure(context, "phc_example", "https://eu.i.posthog.com",
    new NativePostHogOptions { OptOut = true });
// later, once the user consents, and again after every Reset() while consent stands:
NativePostHog.OptIn();
```

`OptIn()` installs the integrations. If the app is in the foreground,
`Application Opened` is captured immediately. The anonymous ID is the one
created at setup. `Application Installed` is never sent for a first launch that
started opted out, because the version was already recorded. Later updates still
send `Application Updated`.

**Feature flag reloads are not gated by opt-out.** Upstream `identify()` (with
a new ID) and `reset()` always reload feature flags. That is a `POST /flags`
carrying the distinct ID, `$anon_distinct_id`, `$device_id` and the default
person properties (app, OS and device). It happens even while opted out and
with flag preloading off. 3.38.2 behaved the same way. An app that needs zero
traffic before consent should therefore call `Identify` and `Reset` only after
`OptIn()`.

`Identify` while opted out stores the account ID locally as the distinct ID and
reloads flags as above, but captures no event. After `OptIn()` the SDK does not
resend `$identify`. No anonymous events were sent before that, so there is
nothing to merge.

**Persisted choice (`PersistOptOut = true`, default).** `OptIn()` and `OptOut()`
are written to disk. The stored value outranks `OptOut` from the next setup on.
It is read at the first consent check once storage is readable. A stored opt-in
does **not** survive `Reset()` (see [Reset caveat](#reset-caveat)): after a
logout, the next launch falls back to the `OptOut` setup value. An app that
passes `OptOut = true` must therefore keep its own consent record and call
`OptIn()` again after every `Reset()`, or simply on every launch while consent
stands. Otherwise analytics silently stop for a user who consented and later
logged out.

**App-owned consent (`PersistOptOut = false`).** Recommended when the app
already stores consent. Pass `OptOut = !hasConsent` from your own store at
every launch. `OptIn()` and `OptOut()` then change only the running process,
and `Reset()` has no consent effect beyond it. Upstream marks `persistOptOut` as `@PostHogInternal`. It
was added in 3.66.0 for wrapper SDKs that own consent, and it carries no
upstream stability guarantee. The wrapper pins the SDK version, so check this
property on every bump.

## Withdrawal

`OptOut()` stops new capture immediately, uninstalls the lifecycle
integrations, and persists the choice. It does **not**:

- purge events that are already queued in memory or on disk. The queue still
  sends them on its timer, on `Flush()` or at the next launch, and the wrapper
  exposes no upstream API to drop them;
- delete the stored anonymous or device IDs;
- delete data on the server (use PostHog person deletion for that).

To also detach the identity, call `Reset()` after `OptOut()`. It keeps the
opt-out, but it sends one flag reload under the new anonymous ID, still carrying
the unchanged device ID (the first anonymous ID). Only uninstalling the app or
clearing its data removes the device ID.

## Reset caveat

Upstream `reset()` clears every stored key except version/build, device ID,
replay/error-tracking/performance/survey config and push bookkeeping. **It also
clears the persisted opt-out** (`PostHog.reset()` at `android-v3.73.1`). The
running process stays opted out in memory, but the next launch falls back to
the `OptOut` setup value.

The wrapper's `Reset()` handles this. If the SDK is opted out at call time, it
calls `optOut()` again after the reset, which rewrites the stored value. This
wrapper behavior differs from upstream and fails closed. An opted-in state is not
rewritten. It falls back to the setup default, so an app that starts with
`OptOut = true` must call `OptIn()` again from its own consent record after
`Reset()` (see [persisted choice](#start-disabled-enable-after-consent)). A process
kill between the two calls would lose the rewrite.

`Reset()` creates a new anonymous ID and reloads flags (see above). The device
ID survives `reset()` (upstream) and is sent with every flag reload. Uninstalling
the app or clearing its data removes all of this state, including the queue.
