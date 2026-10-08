# PostHogApisForAndroidComponents

Public .NET 10 Android interop for the official PostHog mobile SDK.
NuGet ID: **Kapusch.PostHog.Android**. Version 0.1.0 is a qualification build.

The thin wrapper exposes setup, identify/reset, capture, flush, distinct ID and
opt-in/out. Persistence, batching, retry and lifecycle belong to PostHog, with
its bounded queue and platform storage limits. A returned call is not proof of
network delivery. Replay, surveys and feature flags are not exposed or qualified.

See [integration](Docs/Integration.md), [source builds](Docs/SourceMode.md),
[qualification](Docs/Qualification.md), [samples](samples/README.md) and
[third-party licenses](THIRD_PARTY_NOTICES.md).

Build native artifacts, then:

```sh
dotnet pack src/Kapusch.PostHogApisForAndroidComponents/Kapusch.PostHogApisForAndroidComponents.csproj
-c Release -o artifacts/nuget
```

Consumer builds do not download native SDKs.
