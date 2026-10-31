# Contributing

Build the pinned native SDK, pack and test the clean sample consumer. Keep the
wrapper thin. Propose upstream fixes instead of duplicating transport/storage.
Run scripts/verify-public.py before publishing. Do not publish real keys or logs.
The scan rejects real-looking PostHog project keys and, case-insensitively, any
private term read at run time from the KAPUSCH_PRIVATE_TERMS environment variable
(CI: the repository secret of the same name) or the untracked
.git/info/private-terms file (one term per line or comma-separated). Private
terms are never tracked; failures name the file and the term's position only.
The private-term check is skipped with a notice when no term is configured.

## Pull requests

- Keep PRs focused and well-scoped; use the pull request template checklist.
- Branch from `main` and target `main`; this repository does not use long-lived `release/*` branches.
- Follow `.editorconfig` for formatting.
- Do not commit secrets or real project keys.

See `Docs/Release.md` before changing versions, tags, or publishing workflows.

By contributing, you agree that your contributions will be licensed under the repository license (MIT). Participation is covered by the [Code of Conduct](CODE_OF_CONDUCT.md).
