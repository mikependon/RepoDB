# 🔢 Versioning Policy

RepoDB follows [Semantic Versioning 2.0](https://semver.org/) (`MAJOR.MINOR.PATCH`, e.g. `v1.17.0`), with pre-release labels (`-alpha`, `-beta`) for versions still under active testing. This document explains what each part of the version number means, when it changes, and what to expect from each kind of release.

As noted in our [Support Policy](SUPPORT_POLICY.md) and [Security Policy](SECURITY.md#-supported-versions), RepoDB is maintained by a single individual. This policy sets expectations, not an SLA — treat it as our best-effort commitment, and always pin exact versions in production (see the [Enterprise Notice](README.md#enterprise-notice)).

## 📋 Quick Reference

| Segment | Example | Triggered by |
| --- | --- | --- |
| 🧪 Pre-release | `v1.17.0-alpha1`, `v1.17.0-beta2` | A `MAJOR`/`MINOR` release still being tested before it ships |
| 🩹 Patch (3rd digit) | `v1.17.0` → `v1.17.1` | A bug fix against the latest release |
| ✨ Minor (2nd digit) | `v1.17.0` → `v1.18.0` | A new feature, capability, or minor breaking change |
| 🚀 Major (1st digit) | `v1.x.x` → `v2.0.0` | A major breaking change or a brand-new architectural capability |

## 🧪 Pre-Release

Before a `MAJOR` or `MINOR` version ships as final, it is published as one or more pre-release builds, tagged `alpha` or `beta`. Take `v1.17.0` as an example — the version being prepared here is `1.17.0`, and it moves through pre-release builds before that final tag is cut:

```
v1.17.0-alpha1  →  v1.17.0-alpha2  →  ...  →  v1.17.0-beta1  →  v1.17.0-beta2  →  ...  →  v1.17.0
```

- **🌱 Alpha** — The first, non-stable cut of the upcoming version. More changes are still expected, and there can be several alpha builds (`-alpha1`, `-alpha2`, ...) as development stays active. The goal is simple: let consumers try the new functionality as early as possible, without waiting for it to be finished.
- **🔷 Beta** — A stable cut. No further major changes are planned unless a bug is reported or found during this phase. A new, bumped beta (`-beta1`, `-beta2`, ...) is released for each fix until the build is considered the release candidate and promoted to the final version.

**Enterprise guidance:** treat every pre-release as unstable by definition. Do not run alpha or beta builds in production — they exist for early feedback, not for deployment.

## 🩹 Patch (`X.Y.Z`)

A patch release increments the **3rd digit** (`v1.17.0` → `v1.17.1`). It ships when a bug is found in the **latest released version**. Patches are backward-compatible by definition — no API changes, no new features, just a fix.

Only the latest release line receives patches (see [Security Policy - Supported Versions](SECURITY.md#-supported-versions)); there is no backport policy to older major/minor lines.

## ✨ Minor (`X.Y.0`)

A minor release increments the **2nd digit** and resets the patch digit (`v1.17.2` → `v1.18.0`). It ships when the current `MAJOR` line is missing a feature, or needs an additional capability that fits within its existing architecture.

Minor releases may also introduce **minor breaking changes** — small, scoped adjustments needed to deliver the feature cleanly. They are called out explicitly in the release notes; if a change would break most consumers or requires a migration guide, it belongs in a `MAJOR` release instead.

## 🚀 Major (`X.0.0`)

A major release increments the **1st digit** (`v1.x.x` → `v2.0.0`). It is reserved for major breaking changes, or for a genuinely new capability and architecture that was never part of the current major line's vision.

RepoDB has never shipped a `MAJOR` version bump. Every feature delivered so far — and everything currently on the roadmap — is envisioned within `v1.x.x`. Introducing Telemetry/Insights together with Connectors is an example of the kind of addition that stayed within `v1.x.x`: significant, but additive to the existing architecture, not a replacement of it.

A hypothetical **Data Movement** capability — a full architectural evolution rather than an additive feature — is the kind of change that *would* justify a `MAJOR` bump, precisely because it changes the foundation consumers build on, not just what sits on top of it.

**Enterprise guidance:** a `MAJOR` release is your signal to read the release notes in full and plan a migration window before upgrading. A `MINOR`/`PATCH` release should be safe to adopt after your own regression suite passes.

## 🔗 Related Policies

- [Support Policy](SUPPORT_POLICY.md) — how and when support is provided.
- [Security Policy](SECURITY.md) — which versions receive security fixes.
- [Limitations](LIMITATIONS.md) — known, documented gaps per provider.
