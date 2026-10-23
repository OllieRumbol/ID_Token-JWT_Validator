# JWTValidator 2.0.0 — Specification

> **What this document is for:** It answers *what are we building, and why?* It is agreed before any design or code, and it is the source of truth for requirements.

Status: Approved
Last updated: 2026-10-12

---

## 1. Purpose

`JWTValidator` is a public NuGet package that makes it simpler to validate a JWT in C#. The last version, 1.0.3, was released in November 2022. It targets .NET 6, which is out of support, and it depends on a package version with a known vulnerability. Every release is built and pushed by hand.

Version 2.0.0 brings the package up to date and makes releases automatic. It is for developers who use the package, and for the maintainer who releases it.

---

## 2. Background

* **Package.** ID `JWTValidator` on nuget.org. Published versions: 1.0.0, 1.0.1, 1.0.2 and 1.0.3. Owner: the maintainer's nuget.org account.
* **Source.** Public GitHub repository `OllieRumbol/ID_Token-JWT_Validator`. The default branch is `main`. MIT licence.
* **Documentation site.** A separate repository, `OllieRumbol/JWTValidator`, published at https://ollierumbol.github.io/JWTValidator. The package's project URL points to it.
* **.NET support.** .NET 10 is the current long-term support release. .NET 6 is out of support. .NET 8 and .NET 9 reach end of support in November 2026.
* **Known vulnerability.** `System.IdentityModel.Tokens.Jwt` 6.22.0 has a moderate-severity advisory, GHSA-59j7-ghrg-fj52.
* **HMAC key length.** Current versions of the Microsoft IdentityModel libraries reject an HMAC key that is shorter than the hash size of the token's algorithm: 32 bytes for HS256, 48 bytes for HS384 and 64 bytes for HS512. Version 1.0.3 accepted shorter secrets.
* **Semantic versioning.** Version 2.0.0 is a major version because it removes support for older .NET versions and changes behaviour (see section 6.1.7).
* **Trusted Publishing.** nuget.org can accept a push from a GitHub Actions workflow without a long-lived API key. The workflow exchanges a short-lived GitHub identity token for an API key that is valid for one hour. The package owner sets up a policy on nuget.org once. The policy names the repository owner, the repository and the workflow file.
* **Cost.** GitHub Actions minutes are free for public repositories. No Azure resources are needed.

---

## 3. Goals and non-goals

### Goals

1. The library, the tests and the console app target .NET 10 only.
2. Every package the solution depends on is at its latest stable version, and the build reports no known vulnerabilities.
3. These defects from section 4.1 are fixed: item 1 (expiry), item 8 (`IsEmpty`), item 9 (README example), and the personal-data logging part of item 10.
4. Every automated test passes, on any machine, in any time zone, without network access.
5. Every push and pull request is built and tested automatically.
6. Pushing a version tag publishes that version to nuget.org with no manual steps.
7. Version 2.0.0 is published to nuget.org through that pipeline.
8. The repository contains only source files. It contains no build output, no IDE state and no personal data.

### Non-goals

* No new features in the library.
* No changes to the public API beyond the breaking changes in section 6.1.7. Marking parameters as accepting `null` is allowed, because it does not break callers.
* No renaming of the package, the namespaces or the classes.
* No changes to the documentation site. Updating it for 2.0.0 is a follow-up.
* No rewrite of git history. The personal data in section 4.1, item 4 stays in old commits. This is accepted because the token expired in August 2022 and cannot be used, the data belongs to what looks like a test account at a third-party provider, and rewriting the history of a public repository breaks every existing clone. The maintainer may decide on a history rewrite separately.
* The claim type map part of section 4.1, item 10 is not changed. Item 11 is not fixed.
* No deletion or unlisting of versions 1.0.0 to 1.0.3.

---

## 4. Current state of the code

The solution is in the `JWTValidator/` folder.

| Project | Purpose | State |
| --- | --- | --- |
| `JWTValidatorService` | The library. Its package ID is `JWTValidator`. | net6.0. Version 1.0.3 is hard-coded. A package is created on every build. |
| `JWTValidatorTests` | MSTest unit tests (17 tests). | net6.0. On 2026-10-11, 13 pass and 4 fail (see section 4.1). |
| `JWTValidator` | A console app the maintainer uses to try the library locally. It is not published. | net6.0. It references the test project. |

There is no CI pipeline. Releases are packed and pushed by hand.

### 4.1 Defects and gaps

1. **Expiry is in local time but compared with UTC.**
   `JWTValidatorService/Validator/JWTExpiryChecker.cs:156` converts the `exp` claim to local time, and lines 27 and 39 compare it with the current UTC time. When the machine is ahead of UTC, a token is reported as not expired after it has expired. When the machine is behind UTC, a token is reported as expired before it has expired. The error equals the UTC offset. Two tests fail because of this.
2. **A test expects the wrong number of claims.**
   `JWTValidatorTests/JWTValidatorTests.cs:46` expects 1 claim. A token made with one custom claim and an expiry has 4 claims: `role`, `exp`, `nbf` and `iat`.
3. **A test depends on a third-party service.**
   `JWTValidatorTests/JWTValidatorTests.cs:50` downloads keys from `login.personifygo.com` to validate a token from 2022. The test fails. It also needs network access.
4. **Personal data in the repository.**
   `JWTValidator/Program.cs:5` and `JWTValidatorTests/JWTValidatorTests.cs:57` contain a real access token, issued in August 2022. Its payload includes an email address, a name and customer IDs.
5. **Build output is committed.**
   About 270 of the 303 tracked files are in `bin/`, `obj/` and `.vs/` folders. A build changes them.
6. **The package icon is outside the repository.**
   `JWTValidatorService/JWTValidatorService.csproj:26` points to `..\..\..\..\icon.jpg`, a file outside the repository. A clean checkout cannot create the package.
7. **An empty file.** `JWTValidator/TextFile1.txt` is empty and is not used.
8. **`IsEmpty` is wrong.**
   `JWTValidatorService/Extensions/DictionaryExtensions.cs:63` returns `false` for an empty dictionary and throws for `null`.
9. **The README example does not compile.**
   `README.md` uses `JWTValidatorOptionsBuilder.Create()` and `DictionaryContainsKeyAndValue`. The real names are `JWTValidatorOptionsBuilderCreator.Create()` and `ContainsKeyAndValue`. The README is shown on the nuget.org package page.
10. **Global side effects.**
    `JWTValidatorService/Validator/JWTValidator.cs:39-40` turns on personal-data logging (`ShowPII`) and clears the default claim type map. Both settings are process-wide, so they affect every other library in the consumer's application.
11. **Blocking network call.**
    `JWTValidatorService/Validator/JWTValidator.cs:66` waits synchronously for the OpenID configuration and downloads it again on every validation.

---

## 5. External interfaces

### 5.1 nuget.org

* Package ID `JWTValidator`. Versions are immutable. A version that has been pushed cannot be pushed again. It can be unlisted, but not deleted.
* The package page shows the package README, icon, licence, description, tags, release notes, project URL and repository URL.

### 5.2 Authentication

* Publishing uses nuget.org Trusted Publishing (see section 2).
* The maintainer creates the Trusted Publishing policy on nuget.org. Automation does not do this.

### 5.3 OpenID Connect providers

* The library downloads signing keys from the OpenID configuration URL that the caller gives. This behaviour does not change.

---

## 6. Functional specification

### 6.1 Behaviour

#### 6.1.1 Platform and dependencies

1. The package SHALL support .NET 10 and SHALL NOT support any older .NET version.
2. Every package dependency of the solution SHALL be at its latest stable version when the dependencies are upgraded.
3. Restoring the solution SHALL report no known vulnerabilities, when the dependencies are upgraded and again at release.

#### 6.1.2 Validation (unchanged behaviour)

1. For a valid token, the claims result SHALL contain the same keys and values as version 1.0.3 returned for the same token and options.
2. Issuer, audience and lifetime SHALL be checked only when the caller asks for them, as in version 1.0.3.
3. WHEN the options give an OpenID configuration URL, the library SHALL get the signing keys from that URL. It SHALL accept a token signed with any of those keys and reject a token signed with any other key, as in version 1.0.3.
4. The OpenID URL path SHALL be covered by automated tests that do not call the network (section 6.6, item 2).

#### 6.1.3 Expiry

1. `WhenDoesJWTExpire` SHALL return the instant in the token's `exp` claim as a UTC date and time. Callers who want local time convert it themselves.
2. `HasJWTExpired` SHALL return `true` when the `exp` instant is before the current instant, and `false` otherwise. The result SHALL NOT depend on the machine's time zone.
3. The expiry methods SHALL work for an expired token. They check the signature, not the lifetime.
4. WHEN the token has no `exp` claim, `HasJWTExpired` and `WhenDoesJWTExpire` SHALL throw, and the `Try…` methods SHALL return `false`. This is unchanged.
5. WHEN the `exp` claim is later than the last date a .NET date can hold (31 December 9999), the methods SHALL behave as in item 4.
6. WHEN the `exp` claim is far in the past, for example in 1971, `WhenDoesJWTExpire` SHALL return that date and `HasJWTExpired` SHALL return `true`.

#### 6.1.4 Dictionary extensions

1. `IsEmpty` SHALL return `true` when the dictionary is `null` or has no entries, and `false` otherwise.
2. WHEN the dictionary is `null` or empty, `ContainsKeyAndValue` and `ContainsValueInList` SHALL return `false`, and `Print` SHALL return an empty string. None of them SHALL throw.

#### 6.1.5 Logging

1. The library SHALL NOT turn on personal-data logging in the Microsoft IdentityModel libraries. Values that those libraries hide by default stay hidden, unless the calling application turns them on itself.

#### 6.1.6 Secret length

1. WHEN the secret, in UTF-8, is shorter than the minimum for the token's algorithm (section 2), validation SHALL fail. `ValidateJWT` throws and `TryValidateJWT` returns `false`.
2. The builder SHALL NOT check the secret length. It still rejects an empty secret, as in version 1.0.3.

#### 6.1.7 Breaking changes in 2.0.0

The package release notes SHALL list these changes:

1. .NET 6, 7, 8 and 9 are no longer supported.
2. A secret shorter than the minimum for the token's algorithm no longer validates: 32 bytes for HS256, 48 for HS384 and 64 for HS512 (section 6.1.6).
3. `WhenDoesJWTExpire` returns UTC, not local time. `HasJWTExpired` gives the correct result in every time zone (section 6.1.3).
4. `IsEmpty` returns `true` for an empty or `null` dictionary (section 6.1.4).
5. The library no longer turns on personal-data logging (section 6.1.5).
6. The package now depends on version 8 of the Microsoft IdentityModel packages, not version 6. This can affect applications that also use those packages directly.

#### 6.1.8 Continuous integration

1. WHEN a commit is pushed to any branch, or a pull request is opened or updated, THE PIPELINE SHALL restore, build and run every test.
2. THE PIPELINE SHALL report failure when the build fails, a test fails, or the build has a warning about a vulnerable package.

#### 6.1.9 Release

1. WHEN a tag that matches `vMAJOR.MINOR.PATCH` is pushed, THE PIPELINE SHALL build, test, pack and publish that version to nuget.org.
2. WHEN the tag has a pre-release suffix, for example `v2.0.0-beta.1`, the package SHALL be published as a pre-release with that version.
3. The released package version SHALL come from the tag. The project files hold only a placeholder version for local builds.
4. WHEN any build or test step fails, nothing SHALL be published.
5. WHEN a tag does not match the pattern, nothing SHALL be published.
6. WHEN the version already exists on nuget.org, the run SHALL fail and the existing version SHALL NOT change.
7. No long-lived nuget.org API key SHALL be stored in GitHub. The pipeline SHALL use Trusted Publishing.
8. The published package SHALL contain the library, the README, the licence and the icon. It SHALL NOT contain the tests or the console app.
9. The pipeline SHALL NOT check which branch the tagged commit is on. The maintainer tags only commits on `main`.

#### 6.1.10 Repository

1. The repository SHALL NOT track build output or IDE state. A build SHALL NOT change any tracked file.
2. The repository SHALL NOT contain real tokens or personal data.
3. A clean checkout SHALL be able to build, test and pack without any file from outside the repository.
4. The console app SHALL stay in the repository. It SHALL run locally and SHALL NOT be packed or published.
5. The console app SHALL take the token and the OpenID URL at run time. They SHALL NOT be stored in a tracked file.

### 6.2 Worked examples

#### Expiry

`exp` = 1793451600 (2026-10-31 13:00:00 UTC).

| Machine time zone | Current UTC time | `WhenDoesJWTExpire` | `HasJWTExpired` |
| --- | --- | --- | --- |
| UTC | 2026-10-31 12:59:59 | 2026-10-31 13:00:00 UTC | `false` |
| Europe/London (UTC+0 on that date) | 2026-10-31 13:00:01 | 2026-10-31 13:00:00 UTC | `true` |
| Asia/Kolkata (UTC+5:30) | 2026-10-31 12:30:00 | 2026-10-31 13:00:00 UTC | `false` |
| America/New_York (UTC-4) | 2026-10-31 13:30:00 | 2026-10-31 13:00:00 UTC | `true` |
| Any | 2026-10-31 13:00:00 exactly | 2026-10-31 13:00:00 UTC | `false` |

The last row: a token that expires exactly now is not yet expired, as in version 1.0.3.

#### Claims

A token signed with a 36-character secret, with claim `role` = `member` and an expiry in 2 days:

| Key | Values |
| --- | --- |
| `role` | `member` |
| `exp` | the expiry, as Unix seconds |
| `nbf` | the issue time, as Unix seconds |
| `iat` | the issue time, as Unix seconds |

#### Secret length

| Secret (HS256 unless stated) | Result |
| --- | --- |
| 36 ASCII characters (a GUID) | The token validates. |
| 32 ASCII characters | The token validates. |
| 31 ASCII characters | `TryValidateJWT` returns `false`. |
| 63 ASCII characters, token signed with HS512 | `TryValidateJWT` returns `false`. |
| 64 ASCII characters, token signed with HS512 | The token validates. |

#### OpenID URL path

The provider at the OpenID URL publishes two RSA keys, K1 and K2. Every token uses RS256 and contains the claim `role` = `member`.

| Token signed with | `TryValidateJWT` | Claims |
| --- | --- | --- |
| K1 | `true` | include `role` → [`member`] |
| K2 (the provider rotated keys) | `true` | include `role` → [`member`] |
| K3 (not published by the provider) | `false` | empty |

#### Dictionary extensions

| Dictionary | `IsEmpty` | `ContainsValueInList("member")` | `Print` |
| --- | --- | --- | --- |
| `null` | `true` | `false` | empty string |
| no entries | `true` | `false` | empty string |
| `role` → [`member`] | `false` | `true` | `"role: member \n"` (format unchanged from 1.0.3) |

#### Release tags

| Tag pushed | Result |
| --- | --- |
| `v2.0.0` | Version 2.0.0 is published. |
| `v2.0.0-beta.1` | Pre-release 2.0.0-beta.1 is published. |
| `2.0.0` | Nothing is published. |
| `v2.0` | Nothing is published. |
| `v1.0.3` | The run fails. 1.0.3 already exists. |

### 6.4 Outcomes and states

| Pipeline outcome | Meaning |
| --- | --- |
| Build passed | Restore, build and tests passed. Nothing was published. |
| Build failed | A step failed. Nothing was published. |
| Released | All steps passed, and the version is on nuget.org. |
| Release failed | A step failed. Nothing was published, or nuget.org rejected the push. |

### 6.6 Non-functional requirements

1. **Time-zone independence.** The tests SHALL detect the defect in section 4.1, item 1 even when the machine running them is in UTC.
2. **No network in tests.** No automated test SHALL call an external service.
3. **Security.** No real secret, real token or personal data SHALL be committed or written to pipeline logs. Random values that tests generate are not real secrets.
4. **Cost.** The pipeline SHALL use only free GitHub Actions minutes for public repositories.
5. **Package page.** The README example on the package page SHALL compile against version 2.0.0. The README SHALL state that the library targets .NET 10, and the minimum secret length for each HMAC algorithm.

### 6.7 Edge cases

* A token with an `exp` far in the future or far in the past (section 6.1.3, items 5 and 6).
* A token with no `exp` claim.
* A machine time zone that is ahead of UTC, behind UTC, or has a half-hour offset.
* A token that expires exactly now.
* A secret of exactly the minimum length, and one byte shorter, for HS256 and HS512.
* The same release tag pushed twice, or deleted and pushed again.
* A release tag on a commit that is not on `main`. It publishes (section 6.1.9, item 9).
* A pull request from a fork. It must build and test but must not be able to publish.
* A `null` claims dictionary passed to an extension method.

---

## 7. Open questions

None. Seven questions were raised and answered on 2026-10-11. The answers are in sections 3, 6.1 and 6.6.

---

## 8. Assumptions

1. The maintainer can sign in to nuget.org as an owner of the `JWTValidator` package and create a Trusted Publishing policy.
2. The maintainer pushes release tags. No one else releases the package.
3. The GitHub repository stays public, so Actions minutes are free.
4. The package icon is the documentation site's logo: three rounded bars for the header, payload and signature, as a 128 × 128 PNG with a transparent background. It may be committed.
5. Releases are made from `main` after the change is merged.
6. Moving to .NET 10 only is acceptable to existing users, because 2.0.0 is a major version and 1.0.3 stays available.

---

## 9. Definition of done

* Every project in the solution targets .NET 10 only.
* Every package dependency was at its latest stable version when the dependencies were upgraded, and restore reports no vulnerabilities at release.
* All automated tests pass on a machine set to UTC, and on a machine set to a non-UTC zone.
* No automated test calls an external service.
* The OpenID URL path is covered by automated tests (section 6.1.2, items 3 and 4).
* The expiry, claims, secret length and dictionary examples in section 6.2 pass as tests.
* The library does not turn on personal-data logging.
* The README example compiles against 2.0.0, and the README states .NET 10 and the minimum secret length for each HMAC algorithm.
* The repository tracks no `bin/`, `obj/` or `.vs/` files, no real tokens and no files from outside it.
* A build on a clean checkout changes no tracked file.
* A push or pull request runs the build and tests in GitHub Actions.
* A non-matching tag publishes nothing.
* Version 2.0.0 is on nuget.org, published by the pipeline from tag `v2.0.0`.
* The 2.0.0 package page shows the icon, licence, README and release notes, and the release notes list the breaking changes.
* The console app still runs locally against the library. Before tag `v2.0.0` is pushed, the maintainer uses it to validate a real token through a real provider's OpenID URL, and the token validates. This checks the network download, which the automated tests do not.
