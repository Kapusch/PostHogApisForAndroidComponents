# Source builds

Run `bash src/Kapusch.PostHogApisForAndroidComponents/Native/Android/build.sh`, then pack.
Inspect the dependency lock and packaged native resources. Source mode uses
`UseKapuschPostHogAndroidInteropFromSource=true` and imports the matching
buildTransitive target explicitly when using a ProjectReference. All downloads
happen here, never in the consumer target.

Qualification toolchain: .NET SDK and workload set **10.0.203**. Install with
`dotnet workload install android --version 10.0.203` rather than floating manifests.
The native Gradle build uses JDK 17, Android SDK 35, AGP 8.3.2, Gradle 8.7 and
Kotlin 2.1.21 (PostHog Android 3.73.x is built with Kotlin 2.1 and targets
Kotlin 2.0 metadata, which the former Kotlin 1.8 compiler cannot read). The
wrapper AAR has minSdk 23 because PostHog Android requires API 23.
