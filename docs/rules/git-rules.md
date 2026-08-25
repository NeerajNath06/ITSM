# Git Rules

**Phase:** 37

## Branch strategy

- **Trunk-based development**: short-lived feature branches off `main`,
  merged via PR within days, not long-lived module branches that drift.
- Branch naming: `feature/<short-desc>`, `fix/<short-desc>`,
  `chore/<short-desc>`, optionally prefixed with a ticket reference:
  `feature/ITSM-1234-incident-major-flag`.
- No direct commits to `main` — every change goes through a PR, including
  from maintainers/admins.

## Commit conventions

- **Conventional Commits** format:
  `<type>(<scope>): <short summary>` —
  types: `feat`, `fix`, `refactor`, `test`, `docs`, `chore`, `perf`,
  `security`.
- Scope matches the bounded context/module touched (`feat(incident): ...`,
  `fix(cmdb): ...`) for changelog generation and blast-radius clarity at a
  glance.
- Commit body explains **why**, not just what — the diff already shows
  what changed.
- Squash-merge PRs into a single, well-formed commit on `main` by default —
  keeps `main`'s history readable; a PR with multiple genuinely distinct
  logical changes should be split into multiple PRs instead of preserved as
  a messy squashed commit.

## PR rules

- Every PR: links the relevant issue/ticket, describes the change and the
  reasoning, and — for anything touching schema, security, or a bounded
  context boundary — cites the relevant architecture document section.
- A PR that changes an architectural decision locked in this documentation
  set must include or reference the ADR that authorizes the change (Phase
  41/50) — reviewers reject architecture-violating PRs without one.
- CI must be green (build, tests, SAST, architecture tests, migration
  check) before merge is allowed — no "merge despite red CI, fix later."

## Code review

- At least one approval required, from someone outside the PR's author,
  before merge.
- A reviewer checks: does this violate the architecture (`docs/architecture/`),
  does it skip a quality gate (`docs/governance/definition-of-done.md`),
  does it introduce an anti-pattern (`docs/rules/anti-patterns.md`) — not
  just "does it compile and look reasonable."
- Security-sensitive changes (ACL, auth, tenancy, encryption) require
  review from someone with `security_admin`-equivalent review authority,
  not just any team member.

## Merge strategy

- Squash-merge to `main` (see Commit conventions).
- No merge commits accumulating unreviewed intermediate history into
  `main`.
- Merge only after CI green and required approvals — no admin
  override/force-merge outside a documented, exceptional incident-response
  situation, itself logged.

## Release tags

- `main` is continuously deployed to `dev`; promotion to `staging`/`prod`
  is tagged (`vYYYY.MM.DD` or semantic versioning per service, decided per
  `docs/devops/cicd.md`) — a tag marks exactly what's running in a given
  environment, for rollback and audit purposes.

## Hotfixes

- A production hotfix branches from the currently-deployed tag (not from
  a possibly-ahead `main`), goes through the same PR/CI/review process
  (expedited, not skipped), and is merged back to `main` immediately after
  release — never left to drift as an untracked patch applied only to
  `prod`.

---

*Next: `docs/testing/testing-strategy.md` (Phase 38).*
