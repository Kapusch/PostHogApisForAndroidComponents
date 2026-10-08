# Source builds

Run `bash src/Kapusch.PostHogApisForAndroidComponents/Native/Android/build.sh`, then pack.
Inspect the dependency lock and packaged native resources. Source mode uses
`UseKapuschPostHogAndroidInteropFromSource=true` and imports the matching
buildTransitive target explicitly when using a ProjectReference. All downloads
happen here, never in the consumer target.

Qualification toolchain: .NET SDK and workload set **10.0.203**. Install with
`dotnet workload install android --version 10.0.203` rather than floating manifests.
The native Gradle build uses JDK 17 and Android SDK 35.
