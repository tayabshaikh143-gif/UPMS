# Working Rules — UPMS

These rules override speed, convenience, and habit. Read them at the start of every
session and follow them for all work in this project.

Owner: Tayab Shaikh (`tayabshaikh143-gif` on GitHub).

---

## 1. GitHub — permission first

- **Never commit, push, create a branch, delete a branch, change visibility, or
  change repository settings without the owner's explicit permission** in the
  current conversation.
- **Do not commit after every small change.** Work until a module or step is
  complete, show the result, and then ask: *"Module complete — should I commit
  and push?"*
- One commit per completed module, with a clear message. No micro-commits, no
  "wip" commits.
- If the owner asks for more changes while a completed module is still
  uncommitted, remind them: *"Previous module is still pending your approval
  for commit/push."*
- Never force-push, rewrite history, amend published commits, or change repo
  visibility without explicit permission.
- If the owner forgets to approve, remind them — do not assume approval from
  earlier sessions.

## 2. Deletion — ask before removing anything

- **Never delete files, folders, branches, data, database rows, or git history
  without explicit permission.**
- Before any destructive action (`rm`, `Remove-Item`, `git reset --hard`,
  `git clean`, force push, dropping tables, overwriting user files), stop and
  ask. State exactly what will be lost and what the alternative is.
- Prefer moving things to an `archive/` folder over deleting them.
- Temporarily generated files (temp renders, caches) may be cleaned without
  asking, but never project source, documents, diagrams, or commits.

## 3. Corrections — never repeat a reported mistake

- When the owner says something is wrong, **fix it and write it in the
  Corrections Log below** with the rule that prevents it from happening again.
- Before similar work, check the log. A corrected mistake must not return in a
  later deliverable.
- If a new instruction conflicts with an earlier correction, ask which one wins
  instead of guessing.
- Do not argue with a correction; clarify only if the requirement is ambiguous.

## 4. Scope and delivery

- Do only what was asked. Do not add unrequested features, files, or
  restructuring.
- Do not rename or move existing folders/files without permission.
- Deliverables are files in this project (`.png`, `.svg`, `.html`, `.docx`,
  `.md`) and must be shown/verified before being called complete.
- When something is blocked (missing login, missing data, unclear decision),
  say so plainly instead of inventing a workaround.

---

## Corrections Log

Record every reported mistake here. Newest first.

| Date | What went wrong | Rule to prevent it |
|---|---|---|
| 2026-11-09 | First use case diagram (18 use cases, 3 columns) was cluttered and hard to read; owner rejected it. | Use case diagrams must stay readable: prefer 2 columns, group goals by actor, avoid crossing lines, and keep a simplified view alongside the detailed one. |
| 2026-11-09 | Activity diagrams were mixed into `diagrams/` root. | Group diagrams by type in their own folder (`diagrams/activity/`, `diagrams/class/`) from the start. |
