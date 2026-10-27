# PostHogApisForAndroidComponents

Public .NET 10 Android interop for the official PostHog mobile SDK.
NuGet ID: **Kapusch.PostHog.Android**. The next release is 1.0.0-rc.1 (the
version declared in the project; see [manual publication](Docs/Qualification.md#manual-publication)).

The thin wrapper exposes setup, identify/reset, capture, flush, distinct ID and
consent: start opted out (`NativePostHogOptions.OptOut`), `OptIn()`, `OptOut()`
and `IsOptOut`. Capture is enabled by default (upstream default). Persistence,
batching, retry and lifecycle belong to PostHog, with its bounded queue and
platform storage limits. A returned call is not proof of
network delivery. Replay, surveys and feature flags are not exposed or qualified.

See the [changelog](CHANGELOG.md) (1.0.0 has breaking changes),
[integration](Docs/Integration.md), [consent](Docs/Consent.md),
[source builds](Docs/SourceMode.md), [qualification](Docs/Qualification.md), [samples](samples/README.md) and
[third-party licenses](THIRD_PARTY_NOTICES.md).

Build native artifacts with `bash src/Kapusch.PostHogApisForAndroidComponents/Native/Android/build.sh`, then:

```sh
dotnet pack src/Kapusch.PostHogApisForAndroidComponents/Kapusch.PostHogApisForAndroidComponents.csproj
-c Release -o artifacts/nuget
```

Consumer builds do not download native SDKs.
