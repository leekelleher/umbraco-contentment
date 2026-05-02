---
name: contentment-release-draft
description: Use when drafting GitHub release notes for a new Contentment tag (e.g. "draft release notes for 6.1.3", "/contentment-release-draft 6.1.3"). Produces a Markdown file at .github/release-notes/{tag}.md following the project's established release-notes pattern, leaving the personal-anecdote intro and per-bullet polish to the maintainer.
---

# Contentment release draft

Generates a near-publishable Markdown draft for a Contentment GitHub release. The output follows the maintainer's established pattern (greeting → anecdote slot → compare link → "What's new?" bullets → static NuGet/Sponsorship blocks → contributors thanks → sign-off + `:v::heart::dove:`). The skill fills in everything mechanical from `git`/`gh` between the previous tag and the new tag, and leaves a clearly-marked `<!-- TODO -->` block for the intro the maintainer wants to write by hand.

The skill **never** publishes anything. It writes a local file and prints the publish command for the maintainer to run when ready.

## When to invoke

Trigger on any of:
- "draft release notes for {tag}"
- "release notes for v{tag}"
- "/contentment-release-draft {tag}"
- The user mentioning a target version and wanting to assemble release notes.

## Inputs

- **`tag`** (required) — the target version, e.g. `6.1.3` or `6.0.0-beta002`. No leading `v`.
- **`prev_tag`** (optional) — the previous version to diff against. If omitted, compute it (see Step 1).

If only a partial input is given (e.g. just "draft release notes"), ask the user for the target tag.

## Repo constants

- `OWNER=leekelleher`
- `REPO=umbraco-contentment`
- `MAINTAINER=leekelleher` (excluded from the contributors thanks)
- `OUTPUT_DIR=.github/release-notes`

## Steps

### 1. Resolve the version range

1. **Reject if the release already exists** — run `gh release view {tag} --repo {OWNER}/{REPO}` and check the exit code. If it succeeds, stop and tell the user the release already exists; do not write a file.

2. **Resolve `prev_tag` if not provided** — list semver tags newest-first:

   ```bash
   git tag --list --sort=-version:refname
   ```

   Pick the highest existing tag strictly less than `tag` on the **same major line**. Examples:
   - `tag=6.1.3` → `prev_tag=6.1.2`
   - `tag=6.1.0` → `prev_tag=6.0.0` (no `6.0.x` patches; if there had been, e.g. `6.0.1`, that would win)
   - `tag=6.0.0-beta002` → `prev_tag=6.0.0-beta001` (previous pre-release on same major)
   - First stable after pre-releases: `tag=6.0.0` → `prev_tag=6.0.0-beta003` (last pre-release)
   - First-ever release on a fresh major: no `prev_tag`; record `prev_tag=null` and skip the compare-URL line later.

   If the inferred `prev_tag` is ambiguous, ask the user.

3. **Resolve `head_ref`** — the upper bound of the diff:
   - If `tag` exists as a git tag locally (`git rev-parse --verify "refs/tags/{tag}"` succeeds), use `head_ref={tag}` (this supports re-running against historical tags for smoke tests).
   - Otherwise use `head_ref=HEAD` (drafting before the tag is cut).

### 2. Gather facts

The maintainer's release-notes pattern references **three** kinds of work, not just merged PRs:

1. **Issues** that were addressed during the release cycle (closed in the time window between `prev_tag` and `tag`/`HEAD`). The bullet text often matches the issue title verbatim.
2. **PRs** that contributed to the release (whether merged in range or closed as superseded but credit is still due to the author).
3. **Direct commits** to `contrib` / `dev/vX.x` not associated with any PR.

The skill therefore gathers **all three** sets and presents them: a starter bullet list of merged PRs in the compare range (often near-empty for this repo), plus an HTML-comment **candidate block** containing closed issues, closed PRs in the date window, and orphan commits — for the maintainer to triage and pull into the bullets.

#### 2a. Compute the date window

```bash
# lower bound — committer date of the previous tag
git log -1 --format='%cI' {prev_tag}
# upper bound — committer date of the target tag if it exists, else "now"
git rev-parse --verify "refs/tags/{tag}" >/dev/null 2>&1 \
  && git log -1 --format='%cI' {tag} \
  || date -u +'%Y-%m-%dT%H:%M:%SZ'
```

Trim both to date-only (`YYYY-MM-DD`) for the GitHub `closed:start..end` search syntax.

If `prev_tag` is null (first-ever release), skip the issue/PR search; only use orphan-commit history.

#### 2b. Commits in the compare range

```bash
gh api "repos/{OWNER}/{REPO}/compare/{prev_tag}...{head_ref}" \
  --paginate \
  --jq '.commits[] | {sha: .sha, message: .commit.message, author: (.author.login // .commit.author.name)}'
```

If `prev_tag` is null, fall back to `git log --pretty=format:'%H%x09%s%x09%an' {head_ref}`.

For each commit:
- **Skip merge commits**: a commit message starting with `Merge ` (GitHub merge-commit convention) is not a substantive change.
- **Skip bot authors**: any author ending in `[bot]` (e.g. `dependabot[bot]`, `github-actions[bot]`).

#### 2c. Associated merged PRs per commit

For each remaining commit:

```bash
gh api "repos/{OWNER}/{REPO}/commits/{sha}/pulls" \
  --jq '.[] | select(.merged_at != null) | {number: .number, title: .title, user: .user.login, merged_at: .merged_at}'
```

Cache by SHA to avoid duplicate calls.

#### 2d. Closed issues in the date window

```bash
gh issue list --repo {OWNER}/{REPO} --state closed \
  --search "closed:{start_date}..{end_date}" \
  --json number,title,author,closedAt \
  --limit 50 \
  --jq '.[] | {number, title, author: .author.login, closedAt}'
```

Filter out issues authored by `MAINTAINER` and `*[bot]`.

#### 2e. Closed PRs in the date window

```bash
gh pr list --repo {OWNER}/{REPO} --state closed \
  --search "closed:{start_date}..{end_date}" \
  --json number,title,author,mergedAt,closedAt \
  --limit 50 \
  --jq '.[] | {number, title, author: .author.login, mergedAt, closedAt, merged: (.mergedAt != null)}'
```

Filter out PRs authored by `MAINTAINER` and `*[bot]` for the candidate list. (Maintainer-authored PRs are still included in the merged-PRs-in-range bullet seed via 2c — they just aren't surfaced as "needs credit".)

#### 2f. Build collections

- `prs_in_range` = unique merged PRs across all in-range commits, in `merged_at` ascending order, dedup by number.
- `orphan_commits` = commits with no associated merged PR — direct pushes. Keep `(short_sha=sha[:7], subject=first_line_of_message, author)`.
- `closed_issue_candidates` = closed issues from 2d, sorted by `closedAt` descending.
- `closed_pr_candidates` = closed (merged or not) PRs from 2e, **excluding** any PR already in `prs_in_range`, sorted by `closedAt` descending.
- `auto_contributors` = unique users from `prs_in_range[*].user`, excluding maintainer and bots, in first-seen order. This is the **conservative** auto-fill for the thanks line — anything else needs maintainer review.
- `candidate_contributors` = unique users from `closed_issue_candidates[*].author`, `closed_pr_candidates[*].author`, and `orphan_commits[*].author`, minus anyone already in `auto_contributors`, minus maintainer and bots. Surfaced in the comment block for triage. (Orphan-commit authors land here because in this repo the orphan list is mostly maintainer noise — the rare community direct-push deserves a manual look rather than an automatic thank-you.)

### 3. Render the draft

Build the file content from the template below, substituting the placeholders.

#### Starter bullets

These go straight into the visible bullet list; the maintainer polishes wording. Only **merged PRs in the compare range** are seeded here:

- **`prs_in_range`**: `* #{number} {title}` — one line per merged PR.

Why no orphan commits here? In this repo most substantive work lands as direct commits to `contrib` / `dev/vX.x`, alongside trivia (`Incremented version number`, `Updated README`, etc.). Dumping all of those into the visible bullets would force the maintainer to delete more than they keep. Orphan commits move into the candidate block below for triage.

If `prs_in_range` is empty, render `{bullets}` as a single comment-line: `<!-- no merged PRs in range; pull substantive items from the candidates above -->`.

#### Candidate block (HTML comment, above the bullets)

Renders as an HTML comment so it doesn't appear in the published release notes. The maintainer reads it, pulls relevant items into the bullet list, then deletes the comment. Format:

```
<!-- Candidates to consider (window {start_date}..{end_date}).
     Pull what's relevant into the bullets above, then delete this block.

Issues closed in window:
  - #{number} [@{author}] {title}
  - ...

PRs closed in window (not already in the bullets):
  - #{number} [@{author}] {title} ({merged|closed})
  - ...

Orphan commits (no associated merged PR):
  - {short_sha} [@{author}] {subject}
  - ...
-->
```

Omit any sub-section that is empty. Omit the entire comment block if all three are empty (e.g. truly first-ever release with no commits).

#### Compare URL

`https://github.com/{OWNER}/{REPO}/compare/{prev_tag}...{tag}` (use `{tag}`, not `{head_ref}` — the published URL needs the actual tag name).

If `prev_tag` is null (first-ever release on a fresh major), omit the entire compare-URL line and the surrounding blank line / `---` separator above it.

#### Contributors thanks line

Auto-fill from `auto_contributors` (the conservative set):

- 0 auto contributors: render the thanks paragraph as a TODO placeholder (see below) so the maintainer can fill in once they've curated bullets.
- 1: `Thanks to @x for your contribution during this release cycle. 🙏`
- 2: `Thanks to @x and @y for your contributions during this release cycle. 🙏`
- 3+: `Thanks to @a, @b, and @c for your contributions during this release cycle. 🙏` (Oxford comma — matches v6.1.0/v6.1.2 style).

If there are any `candidate_contributors`, render an HTML comment **immediately above** the thanks line listing them, so the maintainer can add names as they pull issues/PRs into the bullets:

```
<!-- Candidate contributors (issue/PR authors closed in window — add as relevant):
  @user1 (#533), @user2 (#534, #536), ...
-->
```

If `auto_contributors` is empty AND `candidate_contributors` is empty, omit both the comment and the thanks paragraph entirely.

When `auto_contributors` is empty but `candidate_contributors` is non-empty, render a TODO thanks line:

```
Thanks to <!-- TODO: pick from the candidates above --> for your contributions during this release cycle. 🙏
```

#### Pre-release framing

If `tag` matches `/-(alpha|beta|rc)/i`, the TODO comment in the template gains the extra hint shown below. Otherwise `{prerelease_hint}` is the empty string.

### 4. Write the file

- Path: `.github/release-notes/{tag}.md` (relative to repo root).
- Create `.github/release-notes/` if missing (`mkdir -p`).
- If the file already exists, **ask the user before overwriting** — they may have started polishing it.

### 5. Report back

After writing, output a short summary to the user:

```
Drafted .github/release-notes/{tag}.md

Compare range: {prev_tag}...{head_ref}
Window:        {start_date}..{end_date}

In bullets (auto-seeded):
  - {N} merged PR(s) in range

Candidates (in HTML comment for triage):
  - {I} closed issue(s) in window
  - {P} closed PR(s) in window not already in the bullets
  - {M} orphan commit(s) (no associated PR)

Contributors:
  - Auto-thanked: @a, @b, @c   (or "none")
  - Candidates:   @x, @y       (or "none")

You'll want to:
1. Replace the <!-- TODO: anecdote --> block with your intro.
2. Read the candidate comment block above the bullets; pull in relevant
   issues/PRs (often the issue title is the bullet text verbatim).
3. Polish bullets (bold component names, group multi-ref bullets, etc.).
4. Update the thanks line with any candidate contributors you pulled in.
5. Delete the HTML comment blocks.
6. Publish:

   gh release create {tag} --title "Contentment v{tag}" --notes-file .github/release-notes/{tag}.md
```

## Template

```markdown
Hey there Umbraco fans!

<!-- TODO: anecdote / personal intro paragraph(s).
Pattern from past releases: 1–2 short paragraphs giving context for the release
(what's been quiet/busy, what motivated the new feature, a forum thread, etc.).
End the last paragraph with: ...**Contentment v{tag}**! 🎉
{prerelease_hint}-->

---

A full changelog can be found here: {compare_url}

### What's new? Features and bug fixes...

<!-- Polish hints (delete this comment when done):
  - Bold component names: **Data List**: Checkbox List: ...
  - Italic asides: _(in Settings section)_
  - Group multi-ref entries: * #503, 1da433c **Data List**: ...
  - Inline thanks for outside contributors: // Thanks to @user! 🎉
-->
{candidate_block}
{bullets}

### Where can I get it?

This release is available on NuGet...

> [`dotnet add package Umbraco.Community.Contentment`](https://www.nuget.org/packages/Umbraco.Community.Contentment)


### Sponsorship

I am developing Contentment in my own personal time, so if it is of great value to you and/or your business, then [**please do sponsor me on GitHub!**](https://github.com/sponsors/leekelleher) ...or if an ongoing sponsorship is too much of a commitment, then you could consider _a **one off** sponsorship_ instead.
_Think of it as gifting me Netflix or Spotify for a month._ 😻

---
{contributor_candidate_block}
{thanks_line}

**Enjoy the release!**

Cheers,
@leekelleher
:v::heart::dove:
```

### Placeholders

- `{tag}` — the target version, no `v`.
- `{compare_url}` — full compare URL. The entire `A full changelog…` line + the blank line and `---` above it are omitted if `prev_tag` is null.
- `{candidate_block}` — HTML-comment block listing closed issues / closed PRs in the date window, separated by a leading blank line. Empty string if there are no candidates.
- `{bullets}` — the rendered starter list: merged PRs in range, one per line. If empty, a single comment-line placeholder.
- `{contributor_candidate_block}` — HTML-comment listing candidate contributors not already in `auto_contributors`, separated by a leading blank line. Empty string if there are no candidates.
- `{thanks_line}` — the contributors thanks paragraph, **or** empty (no thanks paragraph) if both `auto_contributors` and `candidate_contributors` are empty.
- `{prerelease_hint}` — for pre-release tags only:
  ```
  This is a pre-release; past pre-release notes (e.g. v6.0.0-alpha001 / v6.0.0-beta001)
  framed the prose around migration status, breaking changes, or call-for-testing.
  ```
  For stable releases, `{prerelease_hint}` is the empty string.

## Edge cases

- **No previous tag found** (first-ever release): omit the compare-URL line and its surrounding blank line + `---`. Note this in the report.
- **Empty range**: refuse and tell the user the range has no commits.
- **PR with multiple commits in range**: dedupe by PR number — the PR appears once.
- **Commit closes multiple PRs** (rare): the PRs each appear in `prs_in_range` normally; do not try to combine them into one bullet.
- **Bots in contributors**: filter `*[bot]`.
- **Maintainer in contributors**: filter `leekelleher`.
- **Output file exists**: ask before overwriting.
- **gh not authenticated**: surface the `gh` error verbatim and stop.

## Out of scope

- Auto-bolding component names. Past releases vary too much (`**Data List**:`, `**Data List**: Checkbox List:`, no bolding) for a heuristic to be reliable.
- Creating the GitHub draft release. The maintainer publishes manually after polishing.
- Anything beyond writing the single Markdown file and printing the publish command.
