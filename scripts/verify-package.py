from pathlib import Path
import zipfile
packages = list(Path("artifacts/nuget").glob("Kapusch.PostHog.Android.*.nupkg"))
assert len(packages) == 1, "Expected one qualification package"
with zipfile.ZipFile(packages[0]) as archive:
 names = archive.namelist()
 assert any(n.startswith("buildTransitive/") and n.endswith(".targets") for n in names)
 assert "THIRD_PARTY_NOTICES.md" in names
 assert any(n.startswith("licenses/") for n in names)
 assert any(n.startswith("lib/") and n.endswith(".dll") for n in names)
 assert "native/aar/kposthog-release.aar" in names
 assert "native/deps/posthog-3.22.0.jar" in names and "native/deps/curtains-1.2.5.aar" in names
 assert not any("/spm/" in n or "/.gradle/" in n for n in names)
print("Package layout verified")
