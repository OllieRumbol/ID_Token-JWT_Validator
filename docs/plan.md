# JWTValidator 2.0.0 — Plan

> **What this document is for:** It answers *how will we build it?* It turns the approved requirements in spec.md into a design, and it is agreed before any code is written.

Status: Approved
Last updated: 2026-10-20

---

## 1. Approach

The work is in four parts, done in this order:

1. **Clean the repository** so that a clean checkout can build and pack (spec section 6.1.10).
2. **Upgrade** every project to .NET 10 and every package to its latest stable version (spec section 6.1.1). Fix the tests that fail for reasons unrelated to the library (spec section 4.1, items 2 and 3).
3. **Fix the library defects** that spec section 3, goal 3 lists, each with tests (spec sections 6.1.3 to 6.1.6). Add offline tests for the OpenID URL path (spec section 6.1.2, items 3 and 4).
4. **Add one GitHub Actions workflow** that builds and tests every push and pull request and publishes when a version tag is pushed (spec sections 6.1.8 and 6.1.9). Then release 2.0.0 through it.

The solution stays in the `JWTValidator/` folder, with the same three projects, namespaces and class names (spec section 3, non-goals).

### 1.1 Options considered

**Release workflow shape**

| Option | Pros | Cons |
| --- | --- | --- |
| A. One workflow, all steps in one job | Simplest file. | The publish credential would be available to every step on every run, including steps that run test code. |
| B. One workflow, a build job and a separate publish job | Only the publish job can request a publish credential, and it runs only for tags. The package that was tested is the package that is pushed. | Passes the package between jobs as an artifact. |
| C. Two workflows: CI, and a release workflow that rebuilds on tags | Clear separation. | Duplicated steps. The released package is not the one CI tested. |

Chosen: **B**. It limits the publish credential to one job that runs only on tags (spec section 6.1.9, item 7), and it is still one short file.

**First release**

| Option | Pros | Cons |
| --- | --- | --- |
| A. Push `v2.0.0` directly | One release. | If the pipeline or the Trusted Publishing policy is wrong, the first real attempt is the 2.0.0 release. |
| B. Push `v2.0.0-beta.1` first, then `v2.0.0` | Proves the whole pipeline and the policy end to end before the real release. | A pre-release stays on nuget.org permanently (spec section 5.1). |

Chosen: **B**. A pre-release is the expected place to prove a new release process. It also tests the pre-release rule in spec section 6.1.9, item 2.

---

## 2. Configuration

| Setting | Value | Where | Notes |
| --- | --- | --- | --- |
| .NET SDK | `10.0.100`, roll forward to the latest feature band | `global.json` at the repository root | The local machine and CI use the same SDK major version. |
| Target framework | `net10.0` | `JWTValidator/Directory.Build.props` | One place for all three projects (spec section 6.1.1, item 1). |
| Package versions | One version per package | `JWTValidator/Directory.Packages.props` | Central package management. |
| Package version for local builds | `0.0.0-local` | Library project file | A placeholder. It cannot be mistaken for a release, and it is replaced in CI. |
| Package version in CI | From the tag, without the `v`. `0.0.0-ci.<run number>` when there is no tag. | Workflow | The only place a release version comes from (spec section 6.1.9, item 3). |
| nuget.org user name | `ollie` | Workflow `env` | The profile name the login step needs. Public information, not a secret. |
| Trusted Publishing policy | Owner `OllieRumbol`, repository `ID_Token-JWT_Validator`, workflow `ci.yml`, no environment | nuget.org account | Created once by the maintainer (spec section 5.2). |

There are no secrets. Nothing is stored in GitHub secrets or in user secrets.

---

## 3. Architecture

### 3.1 Components

```
global.json                               SDK version
.github/workflows/ci.yml                  Build, test, pack, and publish on tags
docs/adr/0001-release-by-tag-with-trusted-publishing.md

JWTValidator/
  JWTValidator.slnx                       Solution file, migrated from .sln
  Directory.Build.props                   Target framework, nullable, implicit usings, audit warnings as errors
  Directory.Packages.props                Package versions
  JWTValidatorService/                    The library (package ID JWTValidator)
    icon.jpg                              Package icon, moved into the repository
  JWTValidatorTests/                      MSTest tests
  JWTValidator/                           Console app for local checks. Not packable.
```

### 3.2 Existing code to follow or reuse

* `JWTValidatorTests/Helpers/Factory/JWTFactory.cs` — creates signed test tokens. Extend it for tokens with fixed `exp`, `nbf` and `iat` values. Add one method that signs a token by hand with `HMACSHA256` or `HMACSHA512` from a header and payload that the test supplies. Add an option to sign with a given RSA key (RS256), with its key ID in the header. Do not add a second token factory.
* `JWTValidatorTests/Helpers/Builder/` — the options builder for test tokens. Add "not before" and "issued at" options here.
* `JWTValidatorTests/JWTExpiryCheckerTests.cs` — the Arrange, Act, Assert layout to follow.
* `JWTValidatorService/JWTValidatorService.csproj` — keep the package metadata (title, authors, tags, URLs, licence file, readme file).

### 3.3 Interfaces and contracts

The public API does not change, except for the nullability annotations on the dictionary extensions: a nullable dictionary parameter, and a `notnull` constraint on `TKey`.

`JWTExpiryChecker` gets one **internal** constructor so tests can control the current time. The two public constructors call it with `TimeProvider.System`.

```csharp
public JWTExpiryChecker() : this(new JWTValidator(), TimeProvider.System) { }

public JWTExpiryChecker(IJWTValidator jwtValidator) : this(jwtValidator, TimeProvider.System) { }

internal JWTExpiryChecker(IJWTValidator jwtValidator, TimeProvider timeProvider) { ... }
```

`JWTValidator` gets one **internal** constructor so tests can supply the OpenID configuration, with its signing keys, instead of downloading it. The public parameterless constructor keeps today's behaviour.

```csharp
public JWTValidator() : this(CreateConfigurationManager) { }

internal JWTValidator(Func<String, IConfigurationManager<OpenIdConnectConfiguration>> configurationManagerFactory) { ... }

private static IConfigurationManager<OpenIdConnectConfiguration> CreateConfigurationManager(String openIdUrl)
{
    return new ConfigurationManager<OpenIdConnectConfiguration>(openIdUrl, new OpenIdConnectConfigurationRetriever());
}
```

`GetTokenValidationParameters` calls the factory with `options.OpenIdUrl` in place of creating the configuration manager itself. The rest of that method, including the blocking `.Result` (spec section 4.1, item 11), does not change. Tests pass a factory that records the URL it was given and returns IdentityModel's `StaticConfigurationManager<OpenIdConnectConfiguration>` holding the test keys.

The library project makes internals visible to `JWTValidatorTests` only.

The expiry conversion becomes:

```csharp
Double seconds = Double.Parse(expiryDate.First());
DateTime expirationDateTime = DateTime.UnixEpoch.AddSeconds(seconds);
```

`DateTime.UnixEpoch` has `Kind = Utc`, so the result is UTC. An out-of-range `exp` makes `AddSeconds` throw, which gives the behaviour spec section 6.1.3, item 5 requires. The parsing line is unchanged.

`HasJWTExpired` compares `expiryDate < timeProvider.GetUtcNow().UtcDateTime`.

`IsEmpty` becomes:

```csharp
public static Boolean IsEmpty<TKey, TValue>([NotNullWhen(false)] this Dictionary<TKey, List<TValue>>? dictionary)
    where TKey : notnull
{
    return dictionary is null || dictionary.Count == 0;
}
```

`[NotNullWhen(false)]` tells the compiler that the dictionary is not null after an `IsEmpty` check, so the other methods get no new nullable warnings. The other three extension methods accept a nullable dictionary in the same way.

### 3.4 Key decisions

| Decision | Choice | Why | Alternatives rejected |
| --- | --- | --- | --- |
| Expiry type | `DateTime` with `Kind = Utc` | Keeps the public signatures. The `Kind` makes the time zone explicit (spec section 6.1.3, item 1). | `DateTimeOffset`. It is clearer, but it changes the public API. |
| OpenID keys in tests | Internal constructor on `JWTValidator` that takes a configuration manager factory | Tests the OpenID path offline (spec section 6.1.2, item 4) without changing the public API. Same pattern as the clock. | A local HTTPS server in the tests. IdentityModel refuses plain HTTP by default, and a test certificate on the CI runner adds setup. A public constructor, which adds to the public API. |
| Current time | Injected `TimeProvider`, internal constructor | Lets tests check the exact boundary in spec section 6.2 without changing the public API. | Tests based on the real clock cannot check "expires exactly now". A public constructor adds to the public API. |
| Fake clock in tests | `FakeTimeProvider` from `Microsoft.Extensions.TimeProvider.Testing` | The framework's own test type. | A hand-written fake. |
| Personal-data logging | Delete the `ShowPII` line | Spec section 6.1.5. | None. |
| Claim type map | Unchanged | Spec section 3, non-goals. | — |
| Short secret | No library change. The IdentityModel 8 check enforces the minimum for each algorithm. | Spec section 6.1.6. | A check in the builder. |
| Test framework | `MSTest` metapackage, VSTest mode | One package reference replaces three. `dotnet test` works as it does now. | `MSTest.Sdk` with Microsoft.Testing.Platform. It needs extra `global.json` settings for `dotnet test` on .NET 10 and gives this project nothing. |
| `coverlet.collector` | Remove | Nothing uses coverage. The MSTest metapackage includes Microsoft's coverage extension if it is needed later. | Update it. |
| `Newtonsoft.Json` in the console app | Remove | The console app does not use it. | Update it. |
| Console app's reference to the test project | Remove | The console app does not use it, and it pulls MSTest into the app. | Keep it. |
| Console app input | Prompts for the token and the OpenID URL at run time | Spec section 6.1.10, item 5. Nothing is stored or kept in shell history. | Command-line arguments. User secrets, which add setup for one manual check. |
| Console app error output | The console app turns `ShowPII` on for itself, calls `ValidateJWT` instead of `TryValidateJWT`, and prints the error message when validation fails | `ShowPII` was added to the library in 2022 to see why validation failed. This keeps that debugging view for the maintainer. The console app is never published, so no consumer is affected. Spec section 6.1.5 applies to the library only. | Keeping `ShowPII` in the library. Printing only "Invalid JWT", which hides the reason. |
| Packing | Explicit `dotnet pack`. Remove `GeneratePackageOnBuild`. Console app has `IsPackable` set to `false`. | A build should not create packages. The console app must never be packed (spec section 6.1.9, item 8). | — |
| Console app package ID | `JWTValidator.ConsoleApp` | The library's package ID is `JWTValidator`, the same as the console project's name. NuGet restore then fails with "Ambiguous project name 'JWTValidator'", so the solution does not build (spec section 6.1.10, items 1 and 3). | Renaming the console project, which changes more files. Changing the library's package ID, which would break the nuget.org package. |
| Icon location | `JWTValidator/JWTValidatorService/icon.jpg` | Belongs to the package project. Shortest pack path. | The repository root. |
| Shared build settings | `Directory.Build.props` and `Directory.Packages.props` in `JWTValidator/` | Every project file changes for the upgrade anyway. One place for the target framework and versions. | Repeat the settings in each project. |
| Solution file | Migrate to `.slnx` with `dotnet sln migrate`, then delete the `.sln` | It is the .NET 10 default, it is short enough to read and edit, and it rarely causes merge conflicts. The maintainer's Visual Studio opens it. | Keep `.sln`. No solution file, which would mean building each project on its own. |
| Release notes | `PackageReleaseNotes` in the library project file | Existing convention. nuget.org shows it. One place. | A `CHANGELOG.md` as well. It would be a second copy. |
| Vulnerable packages | Set `NuGetAuditMode` to `all` and treat `NU1901` to `NU1904` as errors in `Directory.Build.props`. `NU1900` (vulnerability data could not be downloaded) stays a warning, so a nuget.org outage does not fail the build. | Spec section 6.1.8, item 2. The same rule applies locally and in CI. | Only in CI. |
| `SECURITY.md` and `CONTRIBUTING.md` | Not added in this release | The owner's standard asks for them in public repositories, but they are not part of the spec. They are a candidate for a later change. | Adding them now, which widens the scope. |
| Code style in changed files | Follow the existing style: `String` and `Boolean` type names, and private fields in `PascalCase` | Consistency within each file. Changing the style everywhere is unrelated work. | The style in the global C# rules (`string`, `_camelCase`) for new lines only. It would mix two styles in one file. |
| Other warnings | Fix the nullable warnings in `DictionaryExtensions.cs` and `JWTExpiryChecker.cs`: a `notnull` constraint on `TKey`, `Equals(a, b)` in place of `a.Equals(b)` for list values, and a nullable `out` variable. Warnings are still not treated as errors. | They appear on every pull request that changes these files. Each fix is one line. For non-null values, the results do not change. As a side effect, a `null` value inside a list no longer throws. No spec item requires this; plan 7.2 test 18 keeps it. | Leaving them, which keeps the noise. `TreatWarningsAsErrors`, which is more than this release needs. |

The release decision gets an ADR: `docs/adr/0001-release-by-tag-with-trusted-publishing.md`.

---

## 4. Error handling

| Situation | Behaviour |
| --- | --- |
| A short secret during validation | IdentityModel throws. `ValidateJWT` lets it through. `TryValidateJWT` returns `false`. Unchanged code. |
| No `exp` claim | Unchanged. The expiry methods throw, and the `Try…` methods return `false`. |
| A build or test step fails | The build job fails. The publish job does not run (spec section 6.1.9, item 4). |
| A tag that does not match `vMAJOR.MINOR.PATCH[-suffix]` | The version step fails with a message naming the tag. Nothing is packed or published (spec section 6.1.9, item 5). |
| The version already exists on nuget.org | The push returns a conflict and the publish job fails. `--skip-duplicate` is **not** used (spec section 6.1.9, item 6). |
| A published version turns out to be wrong | A version cannot be replaced (spec section 5.1). Unlist the bad version on nuget.org and release a patch version with a new tag. |
| The publish job is re-run more than 7 days after the build | The package artifact has expired, so the re-run fails. Use "Re-run all jobs" on the original run, which GitHub allows for 30 days. It builds, tests and packs the same tagged commit again, then publishes. After 30 days, delete the tag on GitHub and push the same tag again. Both are safe only while the version has not been published. |
| The NuGet login fails (policy missing or wrong) | The publish job fails before the push. Fix the policy, then re-run the failed job. This is safe because nothing was pushed. |

Re-running the publish job is safe until a push succeeds. After that, a re-run fails on the duplicate, which changes nothing.

---

## 5. Observability

* The workflow logs the computed package version before the build.
* GitHub shows the run result on the commit, the pull request and the tag.
* nuget.org emails the owner when a version is published. The maintainer should check that this notification is turned on.
* A release is confirmed when the version appears on the package page with the expected README, icon and release notes.

---

## 6. Security

| Item | Decision |
| --- | --- |
| Publish credential | Trusted Publishing (spec section 2), through the `NuGet/login` action. No long-lived key exists (spec section 6.1.9, item 7). The temporary key is passed to `dotnet nuget push` as a step output and must appear masked (`***`) in the run log; the beta release checks this. |
| Branch and tag protection | The GitHub ruleset "Protect main" (id 24939760) is active on the default branch and has no bypass actors. It blocks deletion and force pushes, and requires a pull request with 0 approvals. Merge, squash and rebase are allowed. It has a required-status-checks rule with no checks selected yet. The `build` check becomes required after the CI `build` job (section 8.1) first runs, because GitHub can only select a check that has run. `v*` tags are not protected, because no ruleset covers tags. Tagging only `main` stays a working practice (spec section 8, assumptions 2 and 5). |
| Workflow permissions | The default for the workflow is `contents: read`. Only the publish job gets `id-token: write`. |
| Pull requests from forks | They run the build job only. The publish job runs only for tag pushes, and only maintainers can push tags. GitHub does not issue identity tokens to fork pull requests. |
| Personal data | The real token is removed from the console app and the tests (spec section 6.1.10, item 2). Test tokens are generated at run time with random secrets. |
| Personal-data logging | The `ShowPII` line is removed (spec section 6.1.5). |
| Third-party actions | First-party actions only: `actions/checkout`, `actions/setup-dotnet`, `actions/upload-artifact`, `actions/download-artifact` and `NuGet/login`. Pinned to their current major version. `NuGet/login` runs with `id-token: write`; pinning it to a commit hash was considered and rejected, because it is published by the NuGet team, it runs only on tags that the maintainer pushes, and a hash needs manual updates. |
| Job time limits | Each job has `timeout-minutes: 15`, so a stuck run cannot use minutes for hours. |
| Old token in git history | Not changed (spec section 3, non-goals). |

---

## 7. Testing

### 7.1 Principles

* MSTest 4. Tests follow Arrange, Act, Assert. New tests use the name format `Method_Condition_ExpectedResult`. Existing passing tests are not renamed.
* No test calls the network (spec section 6.6, item 2). Every token is generated in the test.
* Tests of `JWTExpiryChecker` use a `FakeTimeProvider` for the current time. `ValidateJWT` has no clock seam, because IdentityModel checks the lifetime against the real clock. So test 14 uses the real clock with time margins of at least one hour, which keeps it reliable.
* **Time zones.** The machine time zone cannot be changed inside a test, so the defect is caught in two ways (spec section 6.6, item 1):
  1. Tests assert `Kind == DateTimeKind.Utc` on the expiry result. The old code returns `Local`, so these tests fail on any machine, including a UTC one.
  2. CI runs the whole test suite twice: once in UTC, and once with `TZ=Asia/Kolkata`. Kolkata is UTC+5:30 all year, so the second run is the same every day. Locally, the maintainer's machine runs in Europe/London.
* The OpenID URL path is tested offline through the internal constructor (section 3.3). The download itself is not tested automatically. The maintainer checks it by hand with the console app before release (spec section 9).

### 7.2 Tests

| # | Test | Spec | Defect it catches |
| --- | --- | --- | --- |
| 1 | `WhenDoesJWTExpire` returns `2026-10-31 13:00:00` with `Kind` `Utc` for `exp` 1793451600 | 6.1.3 item 1, 6.2 | Expiry returned in local time |
| 2 | `HasJWTExpired` data-driven over the rows in spec section 6.2. For each row, the fake clock is set to that row's "current UTC time", and its local time zone is set to that row's zone with `SetLocalTimeZone` | 6.1.3 item 2, 6.2 | Comparison in the wrong zone; wrong boundary (`<=` in place of `<`) |
| 3 | `HasJWTExpired` returns `true` for a token that expired yesterday, and for one that expired in 1971. For the 1971 token, `WhenDoesJWTExpire` returns that date with `Kind` `Utc`. | 6.1.3 items 3 and 6 | Expiry methods that check the lifetime and so reject expired tokens |
| 4 | `HasJWTExpired` and `WhenDoesJWTExpire` throw, and the `Try…` methods return `false`, for a token without `exp` and for a token with `exp` 253402300800 (1 January 10000) | 6.1.3 items 4 and 5 | A changed no-`exp` behaviour; a crash or wrong date for an out-of-range `exp` |
| 5 | Existing `WhenDoesJWTExpire…` tests, also asserting `Kind == Utc` | 6.1.3 item 1 | Same as test 1 |
| 6 | `ValidateJWT` returns exactly `role`, `exp`, `nbf` and `iat` with the expected values (replaces the 1-claim assertion). The token sets all three times to fixed values. | 6.1.2 item 1, 6.2 | A changed claims output after the IdentityModel upgrade |
| 7 | `TryValidateJWT` with a 32-byte secret returns `true` | 6.1.6 item 1, 6.2 | A key-length rule that is stricter than expected |
| 8 | Data-driven over the secret-length rows in spec section 6.2: an HS256 token with a 31-byte key, and HS512 tokens with 63-byte and 64-byte keys. Each token is correctly signed with its own key. Too-short keys: `TryValidateJWT` returns `false` and `ValidateJWT` throws. The 64-byte key validates. | 6.1.6 item 1, 6.2 | Short secrets still accepted; a minimum that does not depend on the algorithm |
| 9 | The builder accepts a 31-byte secret and still rejects an empty one | 6.1.6 item 2 | A length check added to the builder |
| 10 | `IsEmpty`, `ContainsValueInList` and `Print` on a `null`, an empty and a one-entry dictionary | 6.1.4, 6.2 | The `\|\|` bug; null reference exceptions |
| 11 | `ContainsKeyAndValue` on a `null` dictionary returns `false` | 6.1.4 item 2 | Null reference exception |
| 12 | After `ValidateJWT`, `IdentityModelEventSource.ShowPII` keeps the value the caller set: `false` stays `false`, and `true` stays `true` | 6.1.5 | Personal-data logging turned on |
| 13 | Delete `ValidateCliamsFromJWT` | 6.6 item 2, 6.1.10 item 2 | (Removes the network call and the real token.) |
| 14 | Data-driven: issuer, audience and lifetime each set to match, set to not match, and not set. A token is rejected only when an option is set and does not match. A token whose "not before" time is in the future is rejected only when the lifetime option is set. | 6.1.2 item 2 | A changed issuer, audience or lifetime check after the IdentityModel upgrade |
| 15 | Data-driven over the OpenID rows in spec section 6.2: RS256 tokens signed with K1, K2 and K3, validated against a configuration that holds K1 and K2. Checks the result, the `role` claim, and that the factory received the URL from the options. | 6.1.2 items 3 and 4, 6.2 | The OpenID path broken by the IdentityModel upgrade; key rotation not supported; a token signed with an unknown key accepted; the wrong URL used |
| 16 | `JWTExpiryChecker`, built with a `JWTValidator` that uses the test configuration and with the fake clock, returns the `exp` of an RS256 token from `WhenDoesJWTExpire(jwt, Uri)` as UTC. `HasJWTExpired(jwt, Uri)` returns `false` one second before `exp` and `true` one second after. | 6.1.2 item 3, 6.1.3 items 1 and 2 | The `Uri` overloads of the expiry checker broken; the comparison defect on the `Uri` path (spec section 4.1, item 1) |
| 17 | Claims baseline: a token with a string, a boolean, an integer, a decimal number, an array and a nested object claim. The expected dictionary is recorded from what the library returns **before** the IdentityModel upgrade. | 6.1.2 item 1 | A change in how claim values are written after IdentityModel moved from Newtonsoft.Json to System.Text.Json |
| 18 | `ContainsKeyAndValue` and `ContainsValueInList` on a dictionary whose list holds a `null` value return `false` for `"member"`, and do not throw | — (side effect of the warning fix in section 3.4) | A null reference exception from a `null` value inside a list |

**Test token notes.**
* Tests 4 and 8 use the hand-signing method in the test helper (section 3.2). IdentityModel 8 cannot sign with a key shorter than the algorithm's minimum (test 8). The helper supports HS256 and HS512. IdentityModel's own token handler always adds an `exp` claim, and a .NET date cannot hold year 10000, so test 4 needs the hand-signing method.
* IdentityModel 8 rejects a token whose expiry is before its "not before" time. Tests 1 to 3, 6 and 14 set "not before" and "issued at" with the builder, earlier than the expiry.
* Tests 15 and 16 generate the RSA keys K1, K2 and K3 at run time. Each token header carries the signing key's ID.
* Test 12 sets `ShowPII` for each row and restores the earlier value at the end, because the flag is process-wide.

---

## 8. Infrastructure and deployment

### 8.1 Workflow `.github/workflows/ci.yml`

Triggers: a push to any branch, a push of a tag that matches `v*`, and any pull request (spec section 6.1.8, item 1). A commit on a pull request branch therefore runs twice, once for the push and once for the pull request. This is accepted, because minutes are free and the spec asks for both.

Each job has `timeout-minutes: 15`.

**Job `build`** (ubuntu-latest, `contents: read`):

1. Check out the repository.
2. Set up .NET from `global.json`.
3. Work out the version:
   * On a tag, take the tag name and check it against `^v(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)(-[0-9A-Za-z.-]+)?$`. If it matches, remove the `v`. If not, fail. The numbers have no leading zeros, because NuGet would turn `v2.0.01` into version 2.0.1, a different and permanent version.
   * Otherwise, use `0.0.0-ci.<run number>`.
4. Restore, then build in Release with that version.
5. Run the tests in UTC.
6. Run the tests again with `TZ=Asia/Kolkata` and `--no-build`.
7. Pack the library with that version and `--no-build`.
8. Upload the package as an artifact, kept for 7 days.

**Job `publish`** (needs `build`; runs only when the ref is a tag; `id-token: write`):

1. Download the package artifact.
2. `NuGet/login` with user `ollie`, to get a temporary API key.
3. `dotnet nuget push` to `https://api.nuget.org/v3/index.json`, without `--skip-duplicate`.

### 8.2 Release steps

1. Merge the work to `main` through a pull request. CI must pass.
2. The maintainer creates the Trusted Publishing policy on nuget.org (section 2).
3. Tag `main` with `v2.0.0-beta.1` and push the tag. Check that the run passes and the pre-release appears on nuget.org.
4. Push a tag that does not match, for example `vtest`. Check that the run fails at the version step and that nothing is published. Delete the tag.
5. Re-run the `publish` job of the beta run. Check that it fails on the duplicate version.
6. Run the console app against a real token and OpenID URL (spec section 9).
7. Tag `main` with `v2.0.0` and push the tag. Check the package page.
8. Optionally unlist `2.0.0-beta.1`.

### 8.3 Cost

GitHub Actions on a public repository: free. One run takes about 2 to 3 minutes. No Azure resources (spec section 6.6, item 4).

---

## 9. Risks

| Risk | Likelihood | Mitigation |
| --- | --- | --- |
| IdentityModel 8 changes the claims output, for example value formats or extra claims | Medium | Test 6 pins the exact claims. Compare with a 1.0.3 run if it fails. |
| IdentityModel 8 changes other validation behaviour that callers rely on | Medium | Run the full suite and the console app check. List any changes found in the release notes. |
| MSTest 4 breaking changes or new analyzer warnings | Low | Small test surface. Build and fix in the upgrade step. |
| Consumers on .NET 8 or 9 cannot upgrade | Certain for some | Accepted by spec section 8, assumption 6. 1.0.3 stays available. |
| Consumers' short secrets stop working | Medium | Stated in the release notes and README (spec sections 6.1.7 and 6.6, item 5). |
| The Trusted Publishing policy or user name is wrong | Medium | Found by the beta release. Nothing is pushed, and the job can be re-run. |
| A first-party action changes behaviour in a new major version | Low | Pinned to a major version. |
| The real token stays in git history | Certain | Accepted (spec section 3, non-goals). |
| `TZ` is not respected by .NET on the runner | Low | Test 1 catches the defect in any zone. The second run is extra protection. Check the Kolkata run's output once. |
