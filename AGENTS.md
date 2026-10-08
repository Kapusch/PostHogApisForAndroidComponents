# Public native SDK working agreement

Produce a reproducible `Kapusch.PostHog.Android` NuGet. Keep all documentation,
samples and artifacts generic. No application/company names, real project keys,
private diagnostics or customer data. Follow src/Native, buildTransitive, Docs
and samples conventions. Package native dependencies; consumer builds never
fetch them. No custom event queue: use the upstream SDK's persistence and retries.
Pin versions and dependency integrity, include third-party licenses. Verify a
clean packed-package consumer and device/runtime semantics before claiming support.
