# Graph Report - UPMS  (2026-11-14)

## Corpus Check
- 121 files · ~1,258,465 words
- Verdict: corpus is large enough that graph structure adds value.
- Unclassified: 50 file(s) not represented in the graph (top: .csv 37, (none) 7, .css 4)

## Summary
- 2608 nodes · 6453 edges · 97 communities (85 shown, 12 thin omitted)
- Extraction: 96% EXTRACTED · 4% INFERRED · 0% AMBIGUOUS · INFERRED: 258 edges (avg confidence: 0.85)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `ee514388`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- mermaid_extract.py
- drawio_extract.py
- self_check.py
- excalidraw_extract.py
- usecase_diagram.py
- _diagram_css_leaf
- decode_xml_references
- UPMS_UseCase_Diagram_Simple.json
- export_svg_document
- upms-simple.json
- normalize_rgba_presentation_attrs
- UPMS_UseCase_Diagram.json
- css_escape
- _CssRule
- _rewrite_start_tags
- upms.json
- mini.json
- normalize_html_entities
- live-browser.js
- connectSSE
- sloplint.mjs
- setLiveState
- validate_data.py
- el
- modern-screenshot.umd.js
- refcheck.mjs
- theme.mjs
- initPageChat
- usecase_diagram.py
- connectSSE
- Guid
- AppDbContext
- showToast
- report
- search
- resolveVars
- search_stack
- core.py
- getAttr
- renderDesignVisual
- 20261114160207_InitialCreate.Designer.cs
- ref_node_fs
- parseColor
- http
- onAnnotDown
- CatalogDtos.cs
- re
- test_design_system_mode.py
- RtlCliTest
- mountSvelteComponentVariant
- DesignSystemGenerator
- Task
- BM25
- UPMS.Api.Data
- createLiveBrowserDomHelpers
- sloplint-render.mjs
- mountSvelteComponentVariant
- capture.mjs
- createLiveBrowserSessionState
- design_system.py
- compilerOptions
- resolveLiveInjectionAnchor
- install.mjs
- actOnAgentTarget
- compilerOptions
- clsLits
- twGroup
- picture.mjs
- CatalogRefreshTest
- imagegen.mjs
- checkTailwind
- TripsController
- detect_domain
- UPMS.Api.csproj
- install-hook.mjs
- argparse
- JwtOptions
- selector_list_parts
- interface
- tailwind-fail/Hero.tsx
- live-browser-ignores.js
- .oxlintrc.json
- TestThresholdGate
- _er_declaration
- interface
- CatalogSummaryLineEndingsTest
- test_skill_script_paths.py
- TestStackFlagWithDesignSystem
- Trips.tsx
- interface
- Feature.vue
- TripStatus
- tsconfig.json
- TestMetricMath
- TestTextLayoutDataContracts
- TestFixtureValidation

## God Nodes (most connected - your core abstractions)
1. `report()` - 90 edges
2. `search()` - 54 edges
3. `trunc()` - 44 edges
4. `resolveVars()` - 42 edges
5. `DesignSystemGenerator` - 38 edges
6. `connectSSE()` - 37 edges
7. `search_stack()` - 36 edges
8. `setLiveState()` - 34 edges
9. `resumeSession()` - 33 edges
10. `showToast()` - 33 edges

## Surprising Connections (you probably didn't know these)
- `TestEndToEndCoherence` --uses--> `DesignSystemGenerator`  [INFERRED]
  skills/ui-ux-pro-max/scripts/tests/test_design_system_mode.py → skills/ui-ux-pro-max/scripts/design_system.py
- `TestLightResultOutputCoherence` --uses--> `DesignSystemGenerator`  [INFERRED]
  skills/ui-ux-pro-max/scripts/tests/test_design_system_mode.py → skills/ui-ux-pro-max/scripts/design_system.py
- `enableInlineEdit()` --indirect_call--> `own()`  [INFERRED]
  skills/impeccable/scripts/live-browser.js → skills/impeccable/scripts/live-browser-dom.js
- `removalOriginalText()` --indirect_call--> `own()`  [INFERRED]
  skills/impeccable/scripts/live-browser.js → skills/impeccable/scripts/live-browser-dom.js
- `teardown()` --indirect_call--> `stopFocusOutIntoChrome()`  [INFERRED]
  skills/impeccable/scripts/live-browser.js → skills/impeccable/scripts/live-browser-dom.js

## Import Cycles
- None detected.

## Communities (97 total, 12 thin omitted)

### Community 0 - "mermaid_extract.py"
Cohesion: 0.08
Nodes (27): analyze(), _block_summary(), _configure_stdout_utf8(), digest(), Edge, _escape_markdown(), _escape_table(), ExtractError (+19 more)

### Community 1 - "drawio_extract.py"
Cohesion: 0.08
Nodes (31): _aligned(), analyze(), classify_shape(), clean_label(), _configure_stdout_utf8(), _decompress_limited(), digest(), Edge (+23 more)

### Community 2 - "self_check.py"
Cohesion: 0.07
Nodes (26): BalanceParser, canonical_controller(), check_css_references(), check_document_completeness(), check_motion(), check_scripts(), check_svgs(), check_viewbox() (+18 more)

### Community 3 - "excalidraw_extract.py"
Cohesion: 0.14
Nodes (20): analyze(), clean_label(), _configure_stdout_utf8(), digest(), Edge, _escape_inline(), _escape_markdown(), _escape_table() (+12 more)

### Community 4 - "usecase_diagram.py"
Cohesion: 0.10
Nodes (18): escape(), _boxes_overlap(), choose_grid(), Diagram, die(), Ellipse, find_browser(), _has_cycle() (+10 more)

### Community 5 - "_diagram_css_leaf"
Cohesion: 0.21
Nodes (11): _animation_component(), _animation_declarations(), _animation_property(), _css_segments(), _diagram_css_leaf(), _has_animation_property(), has_diagram_stylesheet(), namespace_inline_animations() (+3 more)

### Community 6 - "decode_xml_references"
Cohesion: 0.23
Nodes (13): decoded_css_url_fragment(), escape_css_for_xml(), namespace_defs_ids(), markup_ids(), retarget_style_selectors(), rewrite(), rewrite_comment_token(), rewrite_css_urls() (+5 more)

### Community 7 - "UPMS_UseCase_Diagram_Simple.json"
Cohesion: 0.14
Nodes (13): actors, associations, columns, extends, footnote, generalizations, includes, legend (+5 more)

### Community 8 - "export_svg_document"
Cohesion: 0.15
Nodes (9): assert_export_gate(), decode_xml_references(), ensure_viewbox(), ensure_xmlns(), export_svg_document(), find_defs_ids(), main(), slug_for() (+1 more)

### Community 9 - "upms-simple.json"
Cohesion: 0.14
Nodes (13): actors, associations, columns, extends, footnote, generalizations, includes, legend (+5 more)

### Community 10 - "normalize_rgba_presentation_attrs"
Cohesion: 0.22
Nodes (5): _decoded_css_property_text(), normalize_rgba_presentation_attrs(), normalize_tag(), set_root_id(), replace_attr()

### Community 11 - "UPMS_UseCase_Diagram.json"
Cohesion: 0.17
Nodes (11): actors, associations, extends, footnote, generalizations, includes, subtitle, system (+3 more)

### Community 12 - "css_escape"
Cohesion: 0.24
Nodes (7): css_escape(), css_id_token(), starts_name(), retarget(), retarget_root_selector(), root_bound_compound(), scope_selector()

### Community 13 - "_CssRule"
Cohesion: 0.24
Nodes (7): _css_rule_nodes(), _css_rule_tree(), _CssRule, diagram_css_from_html(), _diagram_css_rules(), _html_css_rules(), _keyframe_names()

### Community 14 - "_rewrite_start_tags"
Cohesion: 0.13
Nodes (9): _css_blocks(), extract_first_svg(), html_tokens(), leading_accessibility_end(), mask_opaque_html(), merge_style_into_defs(), _rewrite_start_tags(), _xml_start_tag() (+1 more)

### Community 15 - "upms.json"
Cohesion: 0.17
Nodes (11): actors, associations, extends, footnote, generalizations, includes, subtitle, system (+3 more)

### Community 16 - "mini.json"
Cohesion: 0.18
Nodes (10): actors, associations, extends, generalizations, includes, subtitle, system, theme (+2 more)

### Community 17 - "normalize_html_entities"
Cohesion: 0.70
Nodes (4): normalize_html_entities(), normalize_region(), replace_entity(), replace_in_attribute()

### Community 18 - "live-browser.js"
Cohesion: 0.03
Nodes (118): absolutizeUrls(), addManualContextText(), applyEditing(), applyPlaceholderSizingStyles(), averageRgb01(), bindEditBadgeProxy(), bufferToBase64(), buildLocatorForLeaf() (+110 more)

### Community 19 - "connectSSE"
Cohesion: 0.05
Nodes (98): applyParamDefaults(), applyParamValue(), applyPlaceholderDimensions(), applySavedParamValues(), applySavedSessionMeta(), buildCyclingRow(), buildInsertPlaceholderSnapshotFromDom(), buildParamsPanel() (+90 more)

### Community 20 - "sloplint.mjs"
Cohesion: 0.03
Nodes (72): ANSWERED, APP_SKIP, args, BUILD_DIRS, check25(), check62(), check76(), CHECKS (+64 more)

### Community 21 - "setLiveState"
Cohesion: 0.10
Nodes (57): abortSvelteComponentInjection(), beginNewLiveConfiguration(), buildPickedAnchorSnapshot(), cancelEditing(), cancelEditingToPicking(), cancelInsertConfigure(), cleanup(), cleanupAcceptedSession() (+49 more)

### Community 22 - "validate_data.py"
Cohesion: 0.05
Nodes (48): read_rows(), TestAccessibilityGuidance, TestChartsTypographyAndIcons, TestCurrentReactGuidance, TestSemanticColors, read_rows(), split_values(), style_identities() (+40 more)

### Community 23 - "el"
Cohesion: 0.08
Nodes (47): actionLabel(), applyConfigureBarChrome(), bindConfigureCountPillTooltip(), bindConfigureInlineControlHover(), bindConfigureModifierPillHover(), buildConfigureActionControl(), buildConfigureCountControl(), buildConfigureRow() (+39 more)

### Community 24 - "modern-screenshot.umd.js"
Cohesion: 0.09
Nodes (53): ae(), be(), bt(), Ce(), Ct(), de(), dt(), _e() (+45 more)

### Community 25 - "refcheck.mjs"
Cohesion: 0.04
Nodes (47): args, AS_JSON, BY_BASE, CANON, catalogFaces, clean, exemplarDir, expectations (+39 more)

### Community 26 - "theme.mjs"
Cohesion: 0.09
Nodes (51): addLog(), ADJUST, args, ASK, BASE_CUSTOM, buildIndex(), check(), contrastOf() (+43 more)

### Community 27 - "initPageChat"
Cohesion: 0.07
Nodes (56): applyGlobalBarLabelState(), armPageChatForTyping(), attachSteerFocusDebug(), attachSteerFocusGuard(), buildSteerProcessingDots(), buildSteerQueueHint(), clearSteerAwaitTimer(), clearSteerFocusRecoverTimer() (+48 more)

### Community 28 - "usecase_diagram.py"
Cohesion: 0.08
Nodes (9): _write_persisted_file(), apply_decision_rules(), _object_without_duplicates(), parse_decision_rules(), _validate_action(), format_output(), read_rows(), read_rows() (+1 more)

### Community 29 - "connectSSE"
Cohesion: 0.07
Nodes (51): agentHasWorkInFlight(), agentStatusText(), applyLiveBarPreference(), barPaletteForTheme(), brandMarkSvg(), buildSavingRow(), connectSSE(), cursorForInsertAxis() (+43 more)

### Community 30 - "Guid"
Cohesion: 0.16
Nodes (4): BusesController, RoutesController, StopsController, VendorsController

### Community 31 - "AppDbContext"
Cohesion: 0.05
Nodes (52): AppDbContext, Buses, Routes, RouteStops, Stops, Trips, Vendors, Bus (+44 more)

### Community 32 - "showToast"
Cohesion: 0.11
Nodes (40): abandonForeignSession(), abandonSupersededGo(), clearStoredManualApplyState(), copyToClipboard(), discardOrphanedSession(), dismissToast(), fetchPendingCount(), handleManualEditActivity() (+32 more)

### Community 33 - "report"
Cohesion: 0.08
Nodes (40): applyWaivers(), appSkip(), baseGate(), check10(), check10Motion(), check11(), check15(), check17() (+32 more)

### Community 34 - "search"
Cohesion: 0.06
Nodes (10): _normalize_rtl_level(), _rtl_row_filter(), search(), _style_search_destination(), TestSearchDomains, TestImeGuidance, read_rows(), RtlDataContractTest (+2 more)

### Community 35 - "resolveVars"
Cohesion: 0.11
Nodes (39): baseSelector(), check1(), check11Lift(), check12(), check15Fade(), check23(), check24a(), check37() (+31 more)

### Community 36 - "search_stack"
Cohesion: 0.08
Nodes (8): _exact_stack_identifier(), search_stack(), TestDiagnosticsContracts, _rows(), TestNativeDesktopStackFreshness, TestTextLayoutRetrieval, _rows(), TestWebStackFreshness

### Community 37 - "core.py"
Cohesion: 0.07
Nodes (23): _contains_phrase(), _domain_keywords(), _exact_match_diagnostic(), _exact_row_identity(), _file_signature(), _legacy_successor_guidance(), _load_csv(), _load_csv_snapshot() (+15 more)

### Community 38 - "getAttr"
Cohesion: 0.12
Nodes (35): accentedHeads(), accentedWordInHero(), ancestorMap(), capsRatio(), check28(), check3(), check30(), check33() (+27 more)

### Community 39 - "renderDesignVisual"
Cohesion: 0.07
Nodes (40): buildCollapsible(), buildColorModels(), buildDesignHeader(), buildListHtml(), buildRadiiModels(), buildTypographyModels(), cssSafe(), designEmptyMessage() (+32 more)

### Community 40 - "20261114160207_InitialCreate.Designer.cs"
Cohesion: 0.16
Nodes (3): InitialCreate, AppDbContextModelSnapshot, UPMS.Api.Migrations

### Community 41 - "ref_node_fs"
Cohesion: 0.12
Nodes (10): globalRoot(), script, png, script, main(), OK(), script, dir (+2 more)

### Community 42 - "parseColor"
Cohesion: 0.11
Nodes (30): bannedGradient(), check2(), check29Glow(), check48(), check65(), check7(), firstFamily(), gradientHues() (+22 more)

### Community 43 - "http"
Cohesion: 0.07
Nodes (28): ASPNETCORE_ENVIRONMENT, applicationUrl, commandName, dotnetRunMessages, environmentVariables, launchBrowser, launchUrl, applicationUrl (+20 more)

### Community 44 - "onAnnotDown"
Cohesion: 0.20
Nodes (17): beginEditPin(), buildAnnotationsForCapture(), buildPinElement(), cancelEditingPin(), clampPlaceholderSize(), finalizeEditingPin(), initAnnotOverlay(), localCoords() (+9 more)

### Community 45 - "CatalogDtos.cs"
Cohesion: 0.09
Nodes (10): BusDto, BusRequest, RouteDetailDto, RouteDto, RouteRequest, RouteStopDto, RouteStopRequest, StopRequest (+2 more)

### Community 46 - "re"
Cohesion: 0.13
Nodes (21): classify_shape(), clean_label(), Diagram, _discard_nonsemantic(), _edge_operators(), _endpoint_group(), _er_table(), _finalize_degrees() (+13 more)

### Community 47 - "test_design_system_mode.py"
Cohesion: 0.06
Nodes (16): _button_outline_text_color(), _contrast_ratio(), _derive_dark_palette(), _filter_anti_patterns_for_mode(), format_master_md(), _palette_is_dark(), _query_wants_dark(), _relative_luminance() (+8 more)

### Community 49 - "mountSvelteComponentVariant"
Cohesion: 0.23
Nodes (14): acceptedDomAlreadyClean(), clearHandledWrapperReloadStamp(), commitAcceptedVariantToDom(), deferredRecoverySuperseded(), ensureAcceptedDomClean(), findAcceptedRuntimeWrappers(), handledWrapperReloadKey(), reloadAfterMissingAcceptedDom() (+6 more)

### Community 50 - "DesignSystemGenerator"
Cohesion: 0.09
Nodes (4): DesignSystemGenerator, _resolve_dial(), TestReasoningMatch, TestReasoningContract

### Community 51 - "Task"
Cohesion: 0.15
Nodes (5): AuthController, AuthResponse, StopDto, AppUser, FullName

### Community 52 - "BM25"
Cohesion: 0.14
Nodes (5): BM25, _get_bm25(), _suggest_identities(), TestBm25CoreBehavior, TestTokenizer

### Community 53 - "UPMS.Api.Data"
Cohesion: 0.12
Nodes (6): LoginRequest, RegisterRequest, Seed, UPMS.Api.Auth, UPMS.Api.Catalog, UPMS.Api.Data

### Community 54 - "createLiveBrowserDomHelpers"
Cohesion: 0.16
Nodes (15): createLiveBrowserDomHelpers(), activeElementDeep(), cloneWithoutChrome(), copyTextViaSelection(), cssId(), liveUiRoot(), makeFrozenAnchor(), raiseHitModal() (+7 more)

### Community 55 - "sloplint-render.mjs"
Cohesion: 0.18
Nodes (21): CHROMES, contrast(), focusRing(), frames(), hideStyles(), inventory(), isUrl(), lineBreaks() (+13 more)

### Community 56 - "mountSvelteComponentVariant"
Cohesion: 0.24
Nodes (13): applyOriginalAttrsToSvelteAnchor(), commitAcceptedSvelteComponentToDom(), componentModuleCandidates(), describeMountFailure(), detectDevServerBase(), getMountedSvelteComponentAnchor(), importFirstReachable(), isSvelteInsertManifest() (+5 more)

### Community 57 - "capture.mjs"
Cohesion: 0.10
Nodes (15): argv, chrome, CHROMES, HERE, isUrl, made, out, roots (+7 more)

### Community 58 - "createLiveBrowserSessionState"
Cohesion: 0.19
Nodes (17): createLiveBrowserSessionState(), clearHandled(), clearScrollY(), clearSession(), isHandled(), loadSession(), markHandled(), nextCheckpointRevision() (+9 more)

### Community 59 - "design_system.py"
Cohesion: 0.07
Nodes (16): ansi_ljust(), _detect_page_type(), format_ascii_box(), add_wrapped(), wrap_text(), format_markdown(), format_page_override_md(), generate_design_system() (+8 more)

### Community 60 - "compilerOptions"
Cohesion: 0.10
Nodes (19): compilerOptions, allowArbitraryExtensions, allowImportingTsExtensions, erasableSyntaxOnly, jsx, lib, module, moduleDetection (+11 more)

### Community 61 - "resolveLiveInjectionAnchor"
Cohesion: 0.16
Nodes (19): buildSvelteExpressionTextMap(), buildSveltePropValuesFromLiveElement(), buildSveltePropValuesV2(), cloneWithoutElements(), collectTextNodes(), collectVisibleTexts(), cssEscapeIdent(), elementMatchesOriginalMarkup() (+11 more)

### Community 62 - "install.mjs"
Cohesion: 0.13
Nodes (14): args, copyDir(), DRY, explicit, FLAGS, has(), HERE, HOME (+6 more)

### Community 63 - "actOnAgentTarget"
Cohesion: 0.22
Nodes (20): actOnAgentTarget(), agentTargetBusyReason(), agentTargetOverlayGone(), agentTargetTaken(), claimAgentTarget(), claimAndActOnAgentTarget(), declineAgentTargetBusy(), declineAgentTargetUnresolvable() (+12 more)

### Community 64 - "compilerOptions"
Cohesion: 0.12
Nodes (16): compilerOptions, allowImportingTsExtensions, erasableSyntaxOnly, lib, module, moduleDetection, noEmit, noFallthroughCasesInSwitch (+8 more)

### Community 65 - "clsLits"
Cohesion: 0.24
Nodes (16): braceEnd(), classCalls(), clsLits(), cutGlued(), jsxPseudo(), el(), js(), jsScan() (+8 more)

### Community 66 - "twGroup"
Cohesion: 0.15
Nodes (16): buildContext(), buildTokens(), check26(), check27(), check34(), checkF16(), importsOf(), linksOf() (+8 more)

### Community 67 - "picture.mjs"
Cohesion: 0.13
Nodes (11): chrome, CHROMES, cssPath, HERE, out, page, roots, [slug, pageArg, yArg] (+3 more)

### Community 69 - "imagegen.mjs"
Cohesion: 0.17
Nodes (7): args, hash, n, prompt, stem, steps, [w, h]

### Community 70 - "checkTailwind"
Cohesion: 0.23
Nodes (12): checkComponentText(), checkTailwind(), closeIndex(), compInner(), directChildren(), tw54(), tw67(), twDataText() (+4 more)

### Community 71 - "TripsController"
Cohesion: 0.24
Nodes (3): TripDto, TripRequest, TripsController

### Community 74 - "UPMS.Api.csproj"
Cohesion: 0.20
Nodes (7): net8.0, Microsoft.AspNetCore.Authentication.JwtBearer (8.0.*), Microsoft.AspNetCore.Identity.EntityFrameworkCore (8.0.*), Microsoft.EntityFrameworkCore.Design (8.0.*), Microsoft.EntityFrameworkCore.SqlServer (8.0.*), Swashbuckle.AspNetCore (6.6.2), Microsoft.NET.Sdk.Web

### Community 75 - "install-hook.mjs"
Cohesion: 0.22
Nodes (6): args, HERE, hookEntry, ourHook(), settings, withoutOurs()

### Community 76 - "argparse"
Cohesion: 0.39
Nodes (3): exact_size_scale(), main(), rasterize()

### Community 77 - "JwtOptions"
Cohesion: 0.29
Nodes (6): JwtOptions, Audience, ExpiryHours, Issuer, Key, JwtService

### Community 78 - "selector_list_parts"
Cohesion: 0.25
Nodes (4): body_inherited_css(), _css_paint_may_target_descendants(), is_chrome_selector(), selector_list_parts()

### Community 79 - "interface"
Cohesion: 0.25
Nodes (7): dependencies, interface, default_prompts, display_name, short_description, policy, allow_implicit_invocation

### Community 80 - "tailwind-fail/Hero.tsx"
Cohesion: 0.33
Nodes (3): badge, Hero(), HeroProps

### Community 81 - "live-browser-ignores.js"
Cohesion: 0.52
Nodes (6): globToRegex(), matchesScope(), normalizeIgnoreRule(), normalizeIgnoreValue(), pageCandidates(), resolveDetectIgnores()

### Community 82 - ".oxlintrc.json"
Cohesion: 0.33
Nodes (5): plugins, rules, react/only-export-components, react/rules-of-hooks, $schema

### Community 84 - "_er_declaration"
Cohesion: 0.25
Nodes (4): _er_declaration(), _er_entity(), _er_label(), _er_name()

### Community 85 - "interface"
Cohesion: 0.33
Nodes (5): interface, display_name, short_description, policy, allow_implicit_invocation

### Community 87 - "test_skill_script_paths.py"
Cohesion: 0.38
Nodes (3): resolve(), shipped_invocations(), SkillScriptPathsTest

### Community 89 - "Trips.tsx"
Cohesion: 0.06
Nodes (87): dependencies, lucide-react@^1.73.0, react@^19.3.1, react-dom@^19.3.1, react-router-dom@^7.18.5, @tanstack/react-query@^5.107.1, devDependencies, oxlint@^1.90.0 (+79 more)

### Community 90 - "interface"
Cohesion: 0.50
Nodes (3): interface, display_name, short_description

### Community 92 - "TripStatus"
Cohesion: 0.40
Nodes (5): TripStatus, Cancelled, Completed, Delayed, Scheduled

## Knowledge Gaps
- **53 isolated node(s):** `Capacity`, `Id`, `Ownership`, `Registration`, `Vendor` (+48 more)
  These have ≤1 connection - possible missing edges. (Counts symbols only; 715 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **12 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `Trip` connect `AppDbContext` to `20261114160207_InitialCreate.Designer.cs`, `TripStatus`, `Guid`?**
  _High betweenness centrality (0.030) - this node is a cross-community bridge._
- **What connects `Capacity`, `Id`, `Ownership` to the rest of the system?**
  _53 weakly-connected nodes found - possible missing edges._
- **Should `mermaid_extract.py` be split into smaller, more focused modules?**
  _Cohesion score 0.08309178743961353 - nodes in this community are weakly interconnected._
- **Why does `DesignSystemGenerator` connect `DesignSystemGenerator` to `design_system.py`, `usecase_diagram.py`, `test_design_system_mode.py`?**
  _High betweenness centrality (0.018) - this node is a cross-community bridge._
- **Should `drawio_extract.py` be split into smaller, more focused modules?**
  _Cohesion score 0.08116883116883117 - nodes in this community are weakly interconnected._
- **Why does `TripDto` connect `TripsController` to `20261114160207_InitialCreate.Designer.cs`, `CatalogDtos.cs`, `Guid`?**
  _High betweenness centrality (0.008) - this node is a cross-community bridge._
- **Should `self_check.py` be split into smaller, more focused modules?**
  _Cohesion score 0.06857142857142857 - nodes in this community are weakly interconnected._