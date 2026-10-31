# Release workflow

`main` is the only long-lived branch. Feature work uses short-lived branches and pull requests targeting `main`; this repository does not create `release/*` branches.

## Channels

The `.github/workflows/publish.yml` workflow routes each run to a channel:

- Manual workflow run without `manual_version`: build a preview package `<VersionPrefix>-preview.<run>.<sha>` and publish it to GitHub Packages.
- `vX.Y.Z-rc.N` tag reachable from `origin/main`: publish that prerelease to NuGet.org.
- `vX.Y.Z` tag reachable from `origin/main`: publish that stable version to NuGet.org.
- Manual workflow run with `manual_version`: publish to NuGet.org only when the workflow runs from `main`.

Every run first scans the public tree, then builds the native wrapper, packs, verifies the package and builds the R8-linked consumer before anything is pushed. Pushes use `--skip-duplicate`, so re-running a job does not fail on a version that already exists.

NuGet versions and Git tags are immutable. Query the official NuGet index before publishing and never move or recreate an existing tag.

## Versions

The version pushed is the tag without its leading `v` (or `manual_version`), passed to `dotnet pack` as `PackageVersion`. Keep the project's `VersionPrefix`/`VersionSuffix`, the sample's `PackageReference` and `CHANGELOG.md` equal to the version you are about to tag, in the same pull request, so that PR CI qualifies exactly what is released. During a publish run the sample reference is aligned to the resolved version in the CI checkout only; nothing is committed.

## One-time owner setup

- Repository secret `NUGET_USER`: the nuget.org profile username (not the email), read by `NuGet/login@v1`.
- nuget.org Trusted Publishing policy for this repository and the workflow file name `publish.yml` (do not rename the file). No long-lived NuGet API key is used.
- GitHub Packages publishing must be allowed for the repository (the workflow requests `packages: write`).
- Optional repository secret `KAPUSCH_PRIVATE_TERMS`: private terms for the public tree scan; the scan skips them when it is not set.

## Release sequence

1. Fetch `origin`, start a short-lived branch from `origin/main`, and verify the worktree is clean.
2. Build the native wrapper (this also checks native artifact integrity), pack the NuGet and run `python3 scripts/verify-package.py` (see `README.md` and `Docs/Qualification.md`).
3. Open a PR to `main` and require CI to pass.
4. Merge and fetch the resulting `origin/main` commit.
5. Create the RC or stable tag on that exact commit and push only the new tag.
6. Verify the publish workflow and the resulting NuGet package before updating consumers.

Device qualification remains a human-reviewed prerequisite before a stable tag.

Do not delete historical tags. Delete a temporary branch only after every retained commit is reachable from `origin/main`.
