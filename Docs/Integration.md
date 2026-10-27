# Integration

Reference `Kapusch.PostHog.Android` 1.0.0-rc.1 from a .NET 10 Android
application (Android 6.0 / API 23 or later).
Initialize once, on the platform UI thread during startup, with a public project
key and host. Use separate test and production projects. No secret API keys in
apps. Capture is enabled at setup by default (upstream default); to wait for
consent, start opted out with `NativePostHogOptions.OptOut` and call `OptIn()`
later. See [consent and opt-out](Consent.md) for first-launch state, withdrawal
and the reset caveat.

Use native anonymous identity until sign-in; identify with a stable account ID.
Call Reset on logout and before direct account A to B transitions. Reset keeps
an opt-out but clears a persisted opt-in: if the app starts opted out, call
OptIn again after Reset while its own consent record says yes. Never identify
one account as another. Persist business event IDs to deduplicate downstream.
Flush requests sending; it does not await or prove ingestion. OptOut stops new
capture but does not purge queued events. Auto screen capture, replay, push,
remote config and feature flag preloading are disabled; Identify and Reset still
send a feature flag request, even while opted out (see [consent](Consent.md)).
Lifecycle capture is configurable. SDK debug logs may contain event properties; disable in
production and redact before sharing.

## API example

```csharp
Kapusch.PostHog.Android.NativePostHog.Configure(applicationContext, "phc_example", "https://eu.i.posthog.com",
    new Kapusch.PostHog.Android.NativePostHogOptions { OptOut = true });
Kapusch.PostHog.Android.NativePostHog.OptIn(); // once the user consents
Kapusch.PostHog.Android.NativePostHog.Capture("item_completed", "{\"item_id\":\"example\"}");
Kapusch.PostHog.Android.NativePostHog.Identify("account-example");
Kapusch.PostHog.Android.NativePostHog.Reset();
```

Properties are a JSON object. Omit missing properties rather than sending JSON
nulls (unsupported by this Android surface). Event capture time is the SDK time;
keep business completion time in a separate UTC property for events recorded
later than they happened.
SDK lifetime is process-wide: configure once before using this singleton API.
