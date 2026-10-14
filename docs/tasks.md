# JWTValidator 2.0.0 — Tasks

> **What this document is for:** It answers *what do we do next?* It splits the approved design into small, verifiable steps and tracks progress against them.

Last updated: 2026-10-14

Tasks T1 to T13 are each one branch from `main` and one pull request into `main`. Name branches `feature/<short-task-name>`, as earlier work in this repository did. Tasks T14 to T17 change no code, so they have no branch or pull request. Claude pushes each branch and opens its pull request after the maintainer approves the push. The maintainer merges it. A task is done only when its pull request is merged into `main`. Run commands from the repository root, in Git Bash.

## Phase 1 — Clean the repository

Goal: The repository holds only source files and no personal data, and a clean checkout can build and pack.

- [x] **T1 — Untrack build output and IDE state**
  - Refs: spec 4.1 items 5 and 7, spec 6.1.10 item 1, plan 3.4 (console app package ID)
  - Depends on: —
  - Done when:
    - No tracked file is under a `bin/`, `obj/` or `.vs/` folder.
    - `JWTValidator/TextFile1.txt` is deleted.
    - The console app's project file sets `PackageId` to `JWTValidator.ConsoleApp`, so the solution restores.
    - A build followed by `git status` shows no changed tracked files.
    - `docs/spec.md`, `docs/plan.md` and `docs/tasks.md` are committed.
  - Verify: `git ls-files | grep -E '/(bin|obj|\.vs)/' ; dotnet build JWTValidator/JWTValidator.sln && git status --porcelain`. Expected: no output from either `grep` or `git status`.
  - Notes: Use `git rm -r --cached`. Do not delete the folders from disk. `.gitignore` already lists `[Bb]in/`, `[Oo]bj/` and `.vs/`. The build still targets .NET 6 at this point. A warning that .NET 6 is out of support is expected. The planning documents were already committed in PR #7. This task also updated plan 6 (branch and tag protection) and T7 for the "Protect main" ruleset, as the maintainer approved. During T1, the build failed on `main` with "Ambiguous project name 'JWTValidator'", with SDK 10.0.401 and 9.0.306. The maintainer approved the package ID fix in plan 3.4 as part of T1. `git status --porcelain` also lists staged changes. So before the commit, Verify checked that the build left no unstaged change, and the exact Verify command runs again after the commit.

- [ ] **T2 — Make the package build from a clean checkout**
  - Refs: spec 4.1 item 6, spec 6.1.9 item 8, spec 6.1.10 items 3 and 4, plan 2, plan 3.4 (packing, icon location)
  - Depends on: T1
  - Done when:
    - `icon.jpg` is at `JWTValidator/JWTValidatorService/icon.jpg`, and the project file packs it from there.
    - `GeneratePackageOnBuild` is removed.
    - The hard-coded version `1.0.3` is replaced by the placeholder `0.0.0-local`.
    - The console app has `IsPackable` set to `false`.
    - A pack of the solution creates exactly one package, `JWTValidator.0.0.0-local.nupkg`. It contains the library, `README.md`, `LICENSE.txt` and `icon.jpg`, and no test or console assemblies.
  - Verify: `dotnet pack JWTValidator/JWTValidator.sln -o artifacts && unzip -l artifacts/JWTValidator.0.0.0-local.nupkg`
  - Notes: Copy the icon from the maintainer's local copy (`icon.jpg` in the Documents folder, 8 KB). Delete `artifacts/` after checking. `.gitignore` already ignores `artifacts/`. Run `/code-review` before ticking.

- [ ] **T3 — Remove the real token**
  - Refs: spec 4.1 items 3 and 4, spec 6.1.10 items 2 and 5, plan 3.4 (console app input, console app error output), plan 7.2 test 13
  - Depends on: T1
  - Done when:
    - The test `ValidateCliamsFromJWT` is deleted.
    - The console app asks for the token and the OpenID URL when it runs, then prints the claims and the expiry as before.
    - The console app turns `ShowPII` on for itself, calls `ValidateJWT`, and prints the error message when validation fails (plan 3.4, console app error output).
    - No tracked file outside `docs/` contains `personifygo` or the token text. The planning documents name the provider and the token prefix on purpose, to describe the defect.
  - Verify: `git grep -n -i -E "personifygo|eyJhbGciOiJSUzI1NiIsImtpZCI6IkMx" -- ':!docs'` (expected: no output), then `printf 'not-a-token\nhttps://example.invalid/.well-known/openid-configuration\n' | dotnet run --project JWTValidator/JWTValidator/JWTValidator.csproj` (expected: both prompts appear, then an error message that says why validation failed, in place of only "Invalid JWT").
  - Notes: The token stays in git history. That is accepted in spec 3, non-goals. Run `/code-review` before ticking.

## Phase 2 — Upgrade

Goal: Every project targets .NET 10, and every package is at its latest stable version.

- [ ] **T4 — Move to .NET 10 and .slnx**
  - Refs: spec 6.1.1 item 1, spec 6.1.2 item 1, plan 2, plan 3.1, plan 3.4 (solution file, shared build settings), plan 7.2 test 17
  - Depends on: T2, T3
  - Done when:
    - `JWTValidator/JWTValidator.slnx` exists, lists the same three projects, and `JWTValidator.sln` is deleted.
    - `global.json` at the repository root selects SDK `10.0.100` with roll forward to the latest feature band.
    - `JWTValidator/Directory.Build.props` sets `net10.0`, nullable and implicit usings. No project file sets them itself.
    - `JWTValidator/Directory.Packages.props` holds every package version, still at today's versions. Project files have no `Version` on package references.
    - The solution builds.
    - Plan 7.2 test 17 exists. Its expected values are what the library returns now, with IdentityModel 6.22.0, and it passes.
  - Verify: `dotnet sln JWTValidator/JWTValidator.slnx list && dotnet build JWTValidator/JWTValidator.slnx && dotnet test JWTValidator/JWTValidator.slnx --filter "FullyQualifiedName!~WhenDoesJWTExpireTestWithJWTAndSecret&FullyQualifiedName!~ValidateJWTFromSecretTest"`. Expected: exit code 0. The filter leaves out the three tests that fail because of known defects.
  - Notes: Use `dotnet sln JWTValidator/JWTValidator.sln migrate`. Package versions do not change in this task, so restore warns about the vulnerable package. That warning is fixed in T5. Restore may also warn about other vulnerable packages that the old test packages bring in. Expected test result: `ValidateJWTFromSecretTest` still fails (spec 4.1 item 2). The two `WhenDoesJWTExpire…` tests fail when the machine is not in UTC (spec 4.1 item 1); Europe/London is UTC+1 until 25 October 2026. Nothing else fails. Test 17 records today's behaviour, so add it before any package changes. Run `/code-review` before ticking.

- [ ] **T5 — Update every package**
  - Refs: spec 6.1.1 items 2 and 3, spec 6.1.2, plan 3.2, plan 3.4 (test framework, coverlet, Newtonsoft.Json, console reference, vulnerable packages), plan 7.2 tests 6, 14 and 17, plan 9
  - Depends on: T4
  - Done when:
    - IdentityModel packages are at their latest stable version (8.23.0 on 2026-10-11).
    - The test project uses the `MSTest` metapackage at its latest stable version (4.5.1 on 2026-10-11) in place of `Microsoft.NET.Test.Sdk`, `MSTest.TestAdapter` and `MSTest.TestFramework`.
    - `coverlet.collector`, `Newtonsoft.Json` and the console app's reference to the test project are removed.
    - `NuGetAuditMode` is `all`, and `NU1901` to `NU1904` are errors, in `Directory.Build.props`. Before the packages are upgraded, restore fails with `NU1902` for `System.IdentityModel.Tokens.Jwt` 6.22.0. This proves the rule works.
    - Plan 7.2 test 17 passes without changes to its expected values, or each difference is recorded in Notes for the release notes.
    - The test token builder can set "not before" and "issued at" (plan 3.2).
    - `ValidateJWTFromSecretTest` asserts exactly the claims `role`, `exp`, `nbf` and `iat`, with fixed expected values (plan 7.2 test 6), and passes.
    - Plan 7.2 test 14 exists and passes.
    - No package is outdated, and restore reports no vulnerabilities.
  - Verify: `dotnet list JWTValidator/JWTValidator.slnx package --outdated && dotnet list JWTValidator/JWTValidator.slnx package --vulnerable --include-transitive && dotnet test JWTValidator/JWTValidator.slnx --filter "FullyQualifiedName!~WhenDoesJWTExpireTestWithJWTAndSecret"`. Expected: no outdated or vulnerable packages listed, and exit code 0.
  - Notes: Expected test result: only the two `WhenDoesJWTExpire…` tests can fail, and only when the machine is not in UTC (spec 4.1 item 1). T6 fixes them. If the claims test fails after the upgrade, record the difference here. It is a behaviour change for the release notes (plan 9). Fix any MSTest 4 build errors. Do not fix unrelated analyzer warnings. If this task does not fit one session, split it into an IdentityModel task and an MSTest task. Run `/code-review` before ticking.

## Phase 3 — Fix the expiry defect and add CI

Goal: The expiry defect is fixed, and every push and pull request is built and tested.

- [ ] **T6 — Return expiry in UTC and compare in UTC**
  - Refs: spec 4.1 item 1, spec 6.1.3, spec 6.2 (expiry), spec 6.6 item 1, plan 3.3, plan 3.4 (expiry type, current time, fake clock), plan 7.2 tests 1 to 5
  - Depends on: T5
  - Done when:
    - `WhenDoesJWTExpire` returns a `DateTime` with `Kind` `Utc`.
    - `HasJWTExpired` uses an injected `TimeProvider`. The public constructors use `TimeProvider.System`. The new constructor is internal, and internals are visible to the test project only.
    - The test token factory can sign a token by hand from a supplied header and payload (plan 3.2).
    - Plan 7.2 tests 1 to 5 exist and pass. Test 2 covers every row of the expiry table in spec 6.2. Tests 3 and 4 cover the far past and far future cases in spec 6.1.3, items 5 and 6.
    - All tests pass.
  - Verify: `dotnet test JWTValidator/JWTValidator.slnx`
  - Notes: Add `Microsoft.Extensions.TimeProvider.Testing` at its latest stable version. Before the fix, test 1 must fail; check this once to prove the test catches the defect. Run `/code-review` before ticking.

- [ ] **T7 — Add the CI build job**
  - Refs: spec 6.1.8, spec 6.6 items 1 and 4, plan 1.1 (workflow shape), plan 7.1 (time zones), plan 8.1 (job `build`)
  - Depends on: T6
  - Done when:
    - `.github/workflows/ci.yml` runs on a push to any branch, a push of a `v*` tag, and any pull request.
    - The workflow has `contents: read` permission only, and the job has `timeout-minutes: 15`.
    - The `build` job follows plan 8.1 steps 1 to 8, including the version step.
    - The pull request for this task shows a passing run, and the run log shows the tests running twice: once in UTC and once in Asia/Kolkata.
    - The maintainer has added the build check to the required status checks of the Protect main ruleset.
  - Verify: `gh run list --workflow ci.yml --branch <branch> --limit 1` shows `completed success`. `gh run view <id> --log` shows both test runs. `gh api repos/OllieRumbol/ID_Token-JWT_Validator/rulesets/24939760` lists `build` in `required_status_checks`.
  - Notes: No publish job yet. Use the current major versions of `actions/checkout`, `actions/setup-dotnet` and `actions/upload-artifact`. From this task on, every pull request must pass CI before merging. Run `/code-review` before ticking.

## Phase 4 — Remaining library changes

Goal: The other defects in spec 3, goal 3 are fixed, and the package page is correct.

- [ ] **T8 — Test the minimum secret length**
  - Refs: spec 6.1.6, spec 6.2 (secret length), plan 3.4 (short secret), plan 7.2 tests 7 to 9
  - Depends on: T7
  - Done when:
    - Plan 7.2 tests 7 to 9 exist and pass.
    - The library code is unchanged.
  - Verify: `dotnet test JWTValidator/JWTValidator.slnx`
  - Notes: Test 8 uses the hand-signing method added in T6, so each token is correctly signed with its own key and fails only because of the key length.

- [ ] **T9 — Test the OpenID URL path offline**
  - Refs: spec 6.1.2 items 3 and 4, spec 6.2 (OpenID URL path), plan 3.2, plan 3.3, plan 3.4 (OpenID keys in tests), plan 7.2 tests 15 and 16
  - Depends on: T7
  - Done when:
    - `JWTValidator` has the internal constructor in plan 3.3. The public constructor downloads the configuration as before.
    - The test token factory can sign an RS256 token with a given RSA key and key ID.
    - Plan 7.2 tests 15 and 16 exist and pass, and make no network calls.
    - All tests pass.
  - Verify: `dotnet test JWTValidator/JWTValidator.slnx`
  - Notes: Generate the RSA keys in the test with `RSA.Create(2048)`. Before wiring the factory into `GetTokenValidationParameters`, test 15 must fail; check this once. Run `/code-review` before ticking.
- [ ] **T10 — Fix `IsEmpty` and null dictionaries**
  - Refs: spec 4.1 item 8, spec 6.1.4, spec 6.2 (dictionary extensions), plan 3.3, plan 7.2 tests 10 and 11
  - Depends on: T7
  - Done when:
    - `IsEmpty` returns `true` for a `null` or empty dictionary.
    - All four extension methods accept a nullable dictionary and do not throw on `null`.
    - Plan 7.2 tests 10 and 11 exist and pass.
  - Verify: `dotnet test JWTValidator/JWTValidator.slnx`
  - Notes: Before the fix, the `IsEmpty` test on an empty dictionary must fail. Run `/code-review` before ticking.

- [ ] **T11 — Stop turning on personal-data logging**
  - Refs: spec 4.1 item 10, spec 6.1.5, plan 3.4 (personal-data logging, claim type map), plan 7.2 test 12
  - Depends on: T7
  - Done when:
    - The `ShowPII` line is removed. The claim type map line is unchanged.
    - Plan 7.2 test 12 exists and passes.
  - Verify: `dotnet test JWTValidator/JWTValidator.slnx`
  - Notes: Before the fix, test 12 must fail. Run `/code-review` before ticking.

- [ ] **T12 — Update the README and release notes**
  - Refs: spec 4.1 item 9, spec 6.1.7, spec 6.6 item 5, plan 3.4 (release notes)
  - Depends on: T8, T9, T10, T11
  - Done when:
    - The README example uses the real class and method names and compiles against the current code.
    - The README states that the library targets .NET 10, and the minimum secret length for HS256, HS384 and HS512.
    - `PackageReleaseNotes` in the library project file describes 2.0.0 and lists the six changes in spec 6.1.7.
  - Verify: Replace the whole contents of the console app's `Program.cs` with the README example. `dotnet build JWTValidator/JWTValidator.slnx` succeeds. Then restore `Program.cs` with `git checkout`.
  - Notes: Any behaviour changes found in T5 also go into the release notes.

## Phase 5 — Release

Goal: Version 2.0.0 is on nuget.org, published by the pipeline.

- [ ] **T13 — Add the publish job and the ADR**
  - Refs: spec 6.1.9, plan 1.1, plan 2 (nuget.org user name), plan 4, plan 6, plan 8.1 (job `publish`)
  - Depends on: T12
  - Done when:
    - The `publish` job follows plan 8.1. It needs `build`, runs only for tags, and is the only job with `id-token: write`.
    - The push does not use `--skip-duplicate`.
    - `docs/adr/0001-release-by-tag-with-trusted-publishing.md` records the context, the decision (plan 1.1) and the consequences.
    - The pull request run passes, and its publish job is skipped.
  - Verify: `gh run view <id>` for this pull request shows `build` passed and `publish` skipped.
  - Notes: Use the current major version of `actions/download-artifact` and `NuGet/login`. Run `/code-review` before ticking.

- [ ] **T14 — Create the Trusted Publishing policy (maintainer)**
  - Refs: spec 5.2, plan 2 (Trusted Publishing policy), plan 5
  - Depends on: T13
  - Done when:
    - On nuget.org, signed in as `ollie`: a policy exists for owner `OllieRumbol`, repository `ID_Token-JWT_Validator`, workflow file `ci.yml`, with no environment.
    - The nuget.org notification for published packages is turned on.
  - Verify: The maintainer checks the Trusted Publishing page on nuget.org.
  - Notes: Claude does not do this step. It changes account settings.

- [ ] **T15 — Release 2.0.0-beta.1 and check the failure cases**
  - Refs: spec 6.1.9 items 2, 5 and 6, spec 6.2 (release tags), plan 1.1 (first release), plan 8.2 steps 3 to 5
  - Depends on: T14
  - Done when:
    - Tag `v2.0.0-beta.1` on `main` produces a passing run, and nuget.org lists `2.0.0-beta.1` as a pre-release with the icon, licence, README and release notes.
    - Tag `vtest` produces a run that fails at the version step and publishes nothing. The tag is then deleted locally and on GitHub.
    - Re-running the beta run's `publish` job fails because the version already exists.
    - The run log shows the temporary API key masked as `***`.
  - Verify: `gh run list --workflow ci.yml --limit 3`, and the package page at https://www.nuget.org/packages/JWTValidator/2.0.0-beta.1
  - Notes: Pushing tags publishes to nuget.org. Confirm each tag push, and the deletion of the `vtest` tag on GitHub, with the maintainer first. If the login fails, fix the policy and re-run the `publish` job (plan 4).

- [ ] **T16 — Check the OpenID path and release 2.0.0**
  - Refs: spec 9, plan 7.1 (OpenID path), plan 8.2 steps 6 to 8
  - Depends on: T15
  - Done when:
    - The maintainer runs the console app with a real token and its OpenID URL, and the token validates.
    - Tag `v2.0.0` on `main` produces a passing run.
    - nuget.org lists `2.0.0` as the latest version, with the icon, licence, README and release notes, and with `.NET 10.0` as the target.
    - Every item in spec 9 is true.
  - Verify: The package page at https://www.nuget.org/packages/JWTValidator/2.0.0, and the spec 9 checklist.
  - Notes: Confirm the tag push with the maintainer first. Unlisting `2.0.0-beta.1` is optional.

- [ ] **T17 — Propose the documentation site project**
  - Refs: spec 3, non-goals
  - Depends on: T16
  - Done when:
    - A separate session is proposed to update the documentation site for 2.0.0, with its own spec, plan and tasks in the `JWTValidator` repository.
  - Verify: The proposal is shown to the maintainer.
  - Notes: Include these observations about the site's own maintenance, which no document records yet: the Node 12 deploy workflow, `actions/checkout@v1`, the stored token, the React 16 peer dependency and the uncommitted favicon changes.

---

## Blocked

None.

## Completed log

- 2026-10-14 — T1
