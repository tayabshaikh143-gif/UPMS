# Skills and tooling

## Installed skills (global — available in every session)

| Skill | Path | Notes |
|---|---|---|
| `use-case-diagram` | `C:\Users\Taha\.config\opencode\skill\use-case-diagram\` | Built in this project. UML use case modelling + JSON renderer (SVG/PNG, optional PlantUML). |
| `diagram-design` | `C:\Users\Taha\.config\opencode\skill\diagram-design\` | Third-party, MIT, v2.6. 45 diagram types, editorial HTML + SVG. Source: https://github.com/cathrynlavery/diagram-design |
| `lazy-senior-dev` | `C:\Users\Taha\.config\opencode\skill\lazy-senior-dev\` | Our adaptation of Ponytail (MIT). **Disabled by owner on 2026-11-14** — moved to `disabled-skills/`, not loaded. |
| `hallmark` | `C:\Users\Taha\.config\opencode\skill\hallmark\` | Anti-AI-slop design skill, v2.0.0, MIT. Floor rules, 50 themes, slop lint, audit/redesign/study. |
| `impeccable` | `C:\Users\Taha\.config\opencode\skill\impeccable\` | Design vocabulary for coding agents, v4.9.1, Apache 2.0. Modes, critique/audit/polish/adapt, design system. |
| `ui-ux-pro-max` | `C:\Users\Taha\.config\opencode\skill\ui-ux-pro-max\` | Design intelligence database v2.15.0. Local search (styles, palettes, fonts, UX rules, stacks). Search paths patched to local skill folder. |
| `graphify` | `C:\Users\Taha\.config\opencode\skills\graphify\` | Installed by Graphify 0.9.108. Code knowledge graph + query/path/explain. |
| `find-skills` | `C:\Users\Taha\.agents\skills\find-skills\` | Vercel Labs skill discovery via `npx skills find` / `add`. MIT. |
| `grilling` | `C:\Users\Taha\.agents\skills\grilling\` | Matt Pocock. Relentless design-tree interview in rounds; use before big decisions. |
| `grill-me` | `C:\Users\Taha\.agents\skills\grill-me\` | User-invoked alias for `grilling`. |

Both are also copied into the repo under `skills/`.

## Skill commands

Use case renderer:

```bash
python skills/use-case-diagram/scripts/usecase_diagram.py <spec.json> \
  --out <diagram.svg> [--png <diagram.png>] [--plantuml <diagram.puml>] \
  [--theme dark] [--scale 2]
```

`--schema` prints the JSON format. `columns` in the spec controls explicit
grouping; `"legend": false` hides the legend.

Diagram-design export (from the skill folder):

```bash
python scripts/export_svg.py <diagram.html> <diagram.svg>
python scripts/export_png.py <diagram.html> <diagram.png>   # needs Playwright
python scripts/self_check.py <diagram.html>                 # accessibility/geometry check
```

PNG rendering in this project used headless Chrome directly:

```powershell
chrome --headless=new --disable-gpu --hide-scrollbars --no-first-run `
  --window-size=1220,1000 --force-device-scale-factor=2 `
  --virtual-time-budget=8000 --screenshot=out.png "file:///C:/path/diagram.html"
```

## Environment

- OS: Windows 10 (win32), PowerShell shell.
- Python 3.11 at `C:\Users\Taha\AppData\Local\Programs\Python\Python311\python.exe`.
- Chrome at `C:\Program Files\Google\Chrome\Application\chrome.exe`.
- Git at `C:\Program Files\Git\cmd\git.exe`.
- No `gh` CLI, no PlantUML/Graphviz/Mermaid CLI installed; not needed — the
  renderers above cover SVG/PNG generation.
- `pandoc` not installed (relevant if `.docx` generation is requested later;
  python-docx may be needed).

## GitHub

- Account: `tayabshaikh143-gif`
- Repo: https://github.com/tayabshaikh143-gif/UPMS (public, `main`)
- Auth: OAuth token stored in Windows Credential Manager through Git Credential
  Manager (`git config --global credential.helper manager`). Pushing works
  without re-login while the token stays valid.
- Commit policy: see `AGENTS.md` — no commit/push without owner permission, one
  commit per completed module.

## Project conventions

- Diagrams: one folder per type (`diagrams/activity/`, `diagrams/class/`,
  `diagrams/sequence/`), always `.html` + `.png` + `.svg`.
- Every completed module updates `README.md` and the files in `AI_docs/`.
- Word deliverables: one `.docx` per project step (SRS already done).

## Graphify workflow

Graphify 0.9.108 is installed (`graphify`, `graphify-mcp`) and its skill is
registered. The project graph lives in `graphify-out/` and is **kept in the
repo** — do not add it to `.gitignore` (owner's instruction).

- Refresh after code changes: `graphify update .`
- Ask the graph before grepping: `graphify query "..."`,
  `graphify path "A" "B"`, `graphify explain "X"`
- Full rebuild: `graphify .` (code-only needs no key; docs/images need an LLM
  key such as `GEMINI_API_KEY`, `OPENAI_API_KEY`, `ANTHROPIC_API_KEY`, or
  `DEEPSEEK_API_KEY`)
- After each module: refresh the graph, then commit it with the module.
- The current graph is code-only: 459 nodes / 953 edges / 19 communities from
  12 code files. Docs and images were skipped without an LLM key.

