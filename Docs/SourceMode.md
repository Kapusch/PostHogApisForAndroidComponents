# Source builds

Run `bash src/Kapusch.PostHogApisForAndroidComponents/Native/Android/build.sh`, then pack.
Inspect the dependency lock and packaged native resources. Source mode uses
`UseKapuschPostHogAndroidInteropFromSource=true` and imports the matching
buildTransitive target explicitly when using a ProjectReference. All downloads
happen here, never in the consumer target.
