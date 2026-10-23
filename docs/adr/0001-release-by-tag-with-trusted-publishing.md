# 0001 — Release by tag with Trusted Publishing

Status: Accepted
Date: 2026-10-23

## Context

* Versions 1.0.0 to 1.0.3 of the `JWTValidator` package were built on the maintainer's machine and uploaded by hand. The version number was in the project file.
* Version 2.0.0 must be released from a pipeline (spec section 6.1.9). The version must come from a git tag, and nothing may be published when a build or test step fails.
* The pipeline must not store a long-lived nuget.org API key (spec section 6.1.9, item 7). nuget.org Trusted Publishing lets a GitHub Actions workflow exchange its short-lived identity token for an API key that is valid for one hour (spec section 2).
* A published version on nuget.org cannot be replaced or deleted, only unlisted (spec section 5.1). A mistake in the first real release cannot be undone.
* The repository is public. Pull requests from forks also run the workflow.

## Decision

1. **One workflow, two jobs** (plan section 1.1, option B). `.github/workflows/ci.yml` has a `build` job and a `publish` job.
   * `build` runs for every push, every `v*` tag and every pull request. It takes the version from the tag, or uses `0.0.0-ci.<run number>` when there is no tag. Then it builds, tests in UTC and in Asia/Kolkata, packs, and uploads the package as an artifact.
   * `publish` needs `build` and runs only for a tag. It downloads that same package, logs in with `NuGet/login`, and pushes it to nuget.org.
2. **Trusted Publishing.** In `ci.yml`, only the `publish` job has `id-token: write`, so only it can request a publish credential. No secret is stored in GitHub. The nuget.org policy names no GitHub environment (plan section 2), so nuget.org itself does not limit publishing to tag runs. See the consequences.
3. **No `--skip-duplicate`.** Pushing a version that already exists fails the run.
4. **Pre-release first** (plan section 1.1). The maintainer releases `v2.0.0-beta.1` before `v2.0.0`, to prove the pipeline and the Trusted Publishing policy.

Options that were rejected:

* One job for every step (plan section 1.1). The publish credential would be available to every step on every run, including steps that run test code.
* Two workflows, with a release workflow that rebuilds on tags (plan section 1.1). The steps would be duplicated, and the released package would not be the one that CI tested.
* A long-lived API key in GitHub secrets (spec section 6.1.9, item 7, and plan section 6). It can leak, and it must be rotated by hand.
* A GitHub environment limited to `v*` tags, named in the nuget.org policy. It would make nuget.org refuse a key to any other run. It was not chosen, because only the maintainer releases the package (spec section 8, assumption 2).

## Consequences

* **No secret to protect or rotate.** The key that `NuGet/login` returns is valid for one hour and appears masked in the run log.
* **The tested package is the published package.** The publish job does not rebuild.
* **A bad tag publishes nothing.** A tag that does not start with `v` starts no run. `v2.0` fails the version step because it has no patch number, and `v2.0.01` fails because the version numbers may not have leading zeros. A tag with an invalid pre-release label, such as `v2.0.0-.`, passes the version step but fails later in `build`, when the version is used. In every case `build` fails or never runs, so `publish` does not run.
* **A duplicate version fails the run.** Re-running the `publish` job is safe until a push succeeds; after that, a re-run fails without changing anything.
* **A wrong release cannot be replaced.** The fix is to unlist the bad version and release a patch version with a new tag (plan section 4).
* **The package artifact is kept for 7 days.** A `publish` re-run after that fails. The maintainer uses "Re-run all jobs" on the original run, which GitHub allows for 30 days and which builds the same tagged commit again. After 30 days, the maintainer deletes the tag and pushes it again. Both are safe only while the version is not published (plan section 4).
* **The maintainer must create the Trusted Publishing policy on nuget.org** before the first tag: owner `OllieRumbol`, repository `ID_Token-JWT_Validator`, workflow `ci.yml`, no environment (plan section 2). Without it, the login step fails and nothing is pushed.
* **Renaming or moving `ci.yml` breaks publishing** until the policy is updated to match.
* **Publishing is limited to tag runs by `ci.yml`, not by nuget.org.** The policy matches any run of `ci.yml`. Someone who can push a branch could edit `ci.yml` to request a key from a job without the tag check, and publish an untested package. This risk is accepted while the maintainer is the only person with write access to the repository. A GitHub environment limited to `v*` tags would close it.
* **Tags are not protected.** No ruleset covers tags, so tagging only commits on `main` is a working practice, not a rule the pipeline checks (spec section 6.1.9, item 9).
* **`NuGet/login` is pinned to its major version, not to a commit hash** (plan section 6). It is published by the NuGet team and runs only on tags that the maintainer pushes. A hash would need manual updates.
* **A first pre-release stays on nuget.org.** `2.0.0-beta.1` cannot be deleted. It can be unlisted after `2.0.0` is released.
