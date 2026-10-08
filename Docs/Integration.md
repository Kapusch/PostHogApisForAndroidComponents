# Integration

Reference `Kapusch.PostHog.Android` 0.1.0 from a .NET 10 Android application.
Initialize once, on the platform UI thread during startup, with a public project
key and host. Use separate test and production projects. No secret API keys in
apps. Respect user consent before setup/capture.

Use native anonymous identity until sign-in; identify with a stable account ID.
Call Reset on logout and before direct account A to B transitions. Never identify
one account as another. Persist business event IDs to deduplicate downstream.
Flush requests sending; it does not await or prove ingestion. OptOut/OptIn defer
to upstream behavior. Auto screen capture and replay are disabled; lifecycle
capture is configurable. SDK debug logs may contain event properties; disable
in production and redact before sharing.

## API example

```csharp
Kapusch.PostHog.Android.NativePostHog.Configure(applicationContext, "phc_example", "https://eu.i.posthog.com");
Kapusch.PostHog.Android.NativePostHog.Capture("practice_credit_recorded", "{\"session_id\":\"example\"}");
Kapusch.PostHog.Android.NativePostHog.Identify("account-example");
Kapusch.PostHog.Android.NativePostHog.Reset();
```

Properties are a JSON object. Omit missing properties rather than sending JSON
nulls (unsupported by this Android surface). Event capture time is the SDK time;
keep business completion time in a separate UTC property for deferred credits.
SDK lifetime is process-wide: configure once before using this singleton API.
