## Summary

- What does this change do?

## Checklist

- [ ] No secrets, real project keys or private diagnostics committed
- [ ] Public tree scan: `python3 scripts/verify-public.py`
- [ ] Built wrapper and checked native artifact integrity: `bash src/Kapusch.PostHogApisForAndroidComponents/Native/Android/build.sh`
- [ ] Packed NuGet: `dotnet pack src/Kapusch.PostHogApisForAndroidComponents/Kapusch.PostHogApisForAndroidComponents.csproj -c Release -o artifacts/nuget`
- [ ] Verified package: `python3 scripts/verify-package.py`
- [ ] Sample built: `dotnet build samples/Smoke/Smoke.csproj -c Release`
- [ ] Updated docs and `CHANGELOG.md` if behavior/integration changed
