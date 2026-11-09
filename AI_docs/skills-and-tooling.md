# Skills and tooling

## Installed skills (global — available in every session)

| Skill | Path | Notes |
|---|---|---|
| `use-case-diagram` | `C:\Users\Taha\.config\opencode\skill\use-case-diagram\` | Built in this project. UML use case modelling + JSON renderer (SVG/PNG, optional PlantUML). |
| `diagram-design` | `C:\Users\Taha\.config\opencode\skill\diagram-design\` | Third-party, MIT, v2.6. 45 diagram types, editorial HTML + SVG. Source: https://github.com/cathrynlavery/diagram-design |

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
