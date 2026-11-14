# Graph Report - UPMS  (2026-11-14)

## Corpus Check
- 131 files · ~1,263,303 words
- Verdict: corpus is large enough that graph structure adds value.
- Unclassified: 49 file(s) not represented in the graph (top: .csv 37, (none) 7, .css 3)

## Summary
- 2729 nodes · 6808 edges · 116 communities (92 shown, 24 thin omitted)
- Extraction: 96% EXTRACTED · 4% INFERRED · 0% AMBIGUOUS · INFERRED: 263 edges (avg confidence: 0.85)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `3e2c9568`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- mermaid_extract.py
- excalidraw_extract.py
- self_check.py
- parse_decision_rules
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
- Booking
- ActionResult
- _normalize
- test_design_system_mode.py
- RtlCliTest
- mountSvelteComponentVariant
- DesignSystemGenerator
- ActionResult
- BM25
- UPMS.Api.Data
- createLiveBrowserDomHelpers
- sloplint-render.mjs
- showToast
- capture.mjs
- createLiveBrowserSessionState
- generate_design_system
- compilerOptions
- package.json
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
- _style_is_dark_primary
- selector_list_parts
- interface
- tailwind-fail/Hero.tsx
- live-browser-ignores.js
- .oxlintrc.json
- TestThresholdGate
- read_rows
- interface
- CatalogSummaryLineEndingsTest
- test_skill_script_paths.py
- TestStackFlagWithDesignSystem
- Trips.tsx
- interface
- Feature.vue
- drawio_extract.py
- tsconfig.json
- TestImeGuidance
- test_core_data_quality.py
- format_ascii_box
- parse_page
- usecase_diagram.py
- _fail
- enableInlineEdit
- _row_identities
- split_values
- TestLandingAndStackContract
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
- `Bookings` --references--> `Booking`  [EXTRACTED]
  apps/api/src/UPMS.Api/Data/AppDbContext.cs → apps/api/src/UPMS.Api/Data/Entities.cs
- `TestEndToEndCoherence` --uses--> `DesignSystemGenerator`  [INFERRED]
  skills/ui-ux-pro-max/scripts/tests/test_design_system_mode.py → skills/ui-ux-pro-max/scripts/design_system.py
- `TestLightResultOutputCoherence` --uses--> `DesignSystemGenerator`  [INFERRED]
  skills/ui-ux-pro-max/scripts/tests/test_design_system_mode.py → skills/ui-ux-pro-max/scripts/design_system.py
- `enableInlineEdit()` --indirect_call--> `own()`  [INFERRED]
  skills/impeccable/scripts/live-browser.js → skills/impeccable/scripts/live-browser-dom.js
- `removalOriginalText()` --indirect_call--> `own()`  [INFERRED]
  skills/impeccable/scripts/live-browser.js → skills/impeccable/scripts/live-browser-dom.js

## Import Cycles
- None detected.

## Communities (116 total, 24 thin omitted)

### Community 0 - "mermaid_extract.py"
Cohesion: 0.06
Nodes (52): analyze(), _block_summary(), classify_shape(), clean_label(), _configure_stdout_utf8(), Diagram, digest(), _discard_nonsemantic() (+44 more)

### Community 1 - "excalidraw_extract.py"
Cohesion: 0.14
Nodes (20): analyze(), clean_label(), _configure_stdout_utf8(), digest(), Edge, _escape_inline(), _escape_markdown(), _escape_table() (+12 more)

### Community 2 - "self_check.py"
Cohesion: 0.07
Nodes (26): BalanceParser, canonical_controller(), check_css_references(), check_document_completeness(), check_motion(), check_scripts(), check_svgs(), check_viewbox() (+18 more)

### Community 3 - "parse_decision_rules"
Cohesion: 0.27
Nodes (4): apply_decision_rules(), _object_without_duplicates(), parse_decision_rules(), _validate_action()

### Community 4 - "usecase_diagram.py"
Cohesion: 0.16
Nodes (7): escape(), _boxes_overlap(), choose_grid(), Diagram, Ellipse, text_width(), wrap_text()

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
Nodes (136): absolutizeUrls(), addManualContextText(), applyOriginalAttrsToSvelteAnchor(), applyPlaceholderDimensions(), applyPlaceholderSizingStyles(), averageRgb01(), beginEditPin(), bindEditBadgeProxy() (+128 more)

### Community 19 - "connectSSE"
Cohesion: 0.05
Nodes (92): abortSvelteComponentInjection(), applyParamDefaults(), applyParamValue(), applySavedParamValues(), applySavedSessionMeta(), buildParamsPanel(), clampVariantIndex(), closedClipPath() (+84 more)

### Community 20 - "sloplint.mjs"
Cohesion: 0.03
Nodes (72): ANSWERED, APP_SKIP, args, BUILD_DIRS, check25(), check62(), check76(), CHECKS (+64 more)

### Community 21 - "setLiveState"
Cohesion: 0.10
Nodes (63): applyEditing(), beginNewLiveConfiguration(), buildPickedAnchorSnapshot(), cancelEditing(), cancelEditingToPicking(), cancelInsertConfigure(), cleanupAcceptedSession(), clearAnnotations() (+55 more)

### Community 22 - "validate_data.py"
Cohesion: 0.09
Nodes (34): TestGeneratedCatalogContract, _catalog_date(), _check_app_interface_contract(), _check_catalog_contract(), _check_catalog_summary(), _check_chart_contract(), _check_color_contract(), _check_core_data_contract() (+26 more)

### Community 23 - "el"
Cohesion: 0.07
Nodes (54): actionLabel(), applyConfigureBarChrome(), bindConfigureCountPillTooltip(), bindConfigureInlineControlHover(), bindConfigureModifierPillHover(), buildConfigureActionControl(), buildConfigureCountControl(), buildConfigureRow() (+46 more)

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
Nodes (55): agentHasWorkInFlight(), applyGlobalBarLabelState(), armPageChatForTyping(), attachSteerFocusDebug(), attachSteerFocusGuard(), buildSteerProcessingDots(), buildSteerQueueHint(), clearSteerAwaitTimer() (+47 more)

### Community 28 - "usecase_diagram.py"
Cohesion: 0.10
Nodes (4): _write_persisted_file(), format_output(), read_rows(), read_rows()

### Community 29 - "connectSSE"
Cohesion: 0.08
Nodes (49): agentStatusText(), applyLiveBarPreference(), barPaletteForTheme(), brandMarkSvg(), buildDesignHeader(), connectSSE(), cursorForInsertAxis(), designPanelCss() (+41 more)

### Community 30 - "Guid"
Cohesion: 0.08
Nodes (17): BusesController, BusDto, BusRequest, RouteDetailDto, RouteDto, RouteRequest, RouteStopDto, RouteStopNameRequest (+9 more)

### Community 31 - "AppDbContext"
Cohesion: 0.04
Nodes (57): AppDbContext, Bookings, Buses, Routes, RouteStops, Stops, Trips, Vendors (+49 more)

### Community 32 - "showToast"
Cohesion: 0.16
Nodes (28): clearStoredManualApplyState(), dismissToast(), fetchPendingCount(), handleManualEditActivity(), hidePendingApplyDock(), manualApplyLoadingText(), manualApplyStateKey(), manualEditEventForCurrentPage() (+20 more)

### Community 33 - "report"
Cohesion: 0.08
Nodes (40): applyWaivers(), appSkip(), baseGate(), check10(), check10Motion(), check11(), check15(), check17() (+32 more)

### Community 34 - "search"
Cohesion: 0.09
Nodes (5): search(), TestSearchDomains, RtlFilterTest, TestStyleTaxonomy, TestTextLayoutRetrieval

### Community 35 - "resolveVars"
Cohesion: 0.11
Nodes (39): baseSelector(), check1(), check11Lift(), check12(), check15Fade(), check23(), check24a(), check37() (+31 more)

### Community 36 - "search_stack"
Cohesion: 0.09
Nodes (6): search_stack(), TestDiagnosticsContracts, _rows(), TestNativeDesktopStackFreshness, _rows(), TestWebStackFreshness

### Community 37 - "core.py"
Cohesion: 0.07
Nodes (19): _contains_phrase(), _domain_keywords(), _exact_stack_identifier(), _file_signature(), _load_csv(), _load_csv_snapshot(), _load_product_keywords(), _load_rows_or_empty() (+11 more)

### Community 38 - "getAttr"
Cohesion: 0.12
Nodes (35): accentedHeads(), accentedWordInHero(), ancestorMap(), capsRatio(), check28(), check3(), check30(), check33() (+27 more)

### Community 39 - "renderDesignVisual"
Cohesion: 0.08
Nodes (35): buildCollapsible(), buildColorModels(), buildListHtml(), buildRadiiModels(), buildTypographyModels(), cssSafe(), designEmptyMessage(), escapeHtml() (+27 more)

### Community 40 - "20261114160207_InitialCreate.Designer.cs"
Cohesion: 0.22
Nodes (3): InitialCreate, AddBookings, AppDbContextModelSnapshot

### Community 41 - "ref_node_fs"
Cohesion: 0.12
Nodes (10): globalRoot(), script, png, script, main(), OK(), script, dir (+2 more)

### Community 42 - "parseColor"
Cohesion: 0.11
Nodes (30): bannedGradient(), check2(), check29Glow(), check48(), check65(), check7(), firstFamily(), gradientHues() (+22 more)

### Community 43 - "http"
Cohesion: 0.07
Nodes (28): ASPNETCORE_ENVIRONMENT, applicationUrl, commandName, dotnetRunMessages, environmentVariables, launchBrowser, launchUrl, applicationUrl (+20 more)

### Community 44 - "Booking"
Cohesion: 0.06
Nodes (29): BookingExpiryService, BookingActionResult, BookingService, Booking, Amount, BoardedAt, CancelledAt, CreatedAt (+21 more)

### Community 45 - "ActionResult"
Cohesion: 0.17
Nodes (6): BookingDto, CreateBookingRequest, BookingsController, PaymentMethod, Cash, Online

### Community 46 - "_normalize"
Cohesion: 0.22
Nodes (5): _exact_match_diagnostic(), _legacy_successor_guidance(), _normalize(), _stack_query_requests_legacy(), _stack_row_filter()

### Community 47 - "test_design_system_mode.py"
Cohesion: 0.08
Nodes (12): _button_outline_text_color(), _contrast_ratio(), _derive_dark_palette(), _filter_anti_patterns_for_mode(), format_master_md(), _palette_is_dark(), _relative_luminance(), _select_palette_for_mode() (+4 more)

### Community 49 - "mountSvelteComponentVariant"
Cohesion: 0.20
Nodes (16): acceptedDomAlreadyClean(), clearHandledWrapperReloadStamp(), commitAcceptedSvelteComponentToDom(), commitAcceptedVariantToDom(), deferredRecoverySuperseded(), ensureAcceptedDomClean(), findAcceptedRuntimeWrappers(), handledWrapperReloadKey() (+8 more)

### Community 50 - "DesignSystemGenerator"
Cohesion: 0.10
Nodes (5): DesignSystemGenerator, _resolve_dial(), TestReasoningMatch, read_rows(), TestReasoningContract

### Community 51 - "ActionResult"
Cohesion: 0.12
Nodes (13): AuthController, AuthResponse, JwtOptions, Audience, ExpiryHours, Issuer, Key, LoginRequest (+5 more)

### Community 52 - "BM25"
Cohesion: 0.16
Nodes (5): BM25, _get_bm25(), _suggest_identities(), TestBm25CoreBehavior, TestTokenizer

### Community 53 - "UPMS.Api.Data"
Cohesion: 0.10
Nodes (4): UPMS.Api.Catalog, UPMS.Api.Migrations, UPMS.Api.Bookings, UPMS.Api.Data

### Community 54 - "createLiveBrowserDomHelpers"
Cohesion: 0.16
Nodes (15): createLiveBrowserDomHelpers(), activeElementDeep(), cloneWithoutChrome(), copyTextViaSelection(), cssId(), liveUiRoot(), makeFrozenAnchor(), raiseHitModal() (+7 more)

### Community 55 - "sloplint-render.mjs"
Cohesion: 0.18
Nodes (21): CHROMES, contrast(), focusRing(), frames(), hideStyles(), inventory(), isUrl(), lineBreaks() (+13 more)

### Community 56 - "showToast"
Cohesion: 0.10
Nodes (34): abandonForeignSession(), abandonSupersededGo(), cleanup(), clearHandled(), clearMountErrorCard(), clearScrollY(), clearSession(), copyToClipboard() (+26 more)

### Community 57 - "capture.mjs"
Cohesion: 0.10
Nodes (15): argv, chrome, CHROMES, HERE, isUrl, made, out, roots (+7 more)

### Community 58 - "createLiveBrowserSessionState"
Cohesion: 0.19
Nodes (17): createLiveBrowserSessionState(), clearHandled(), clearScrollY(), clearSession(), isHandled(), loadSession(), markHandled(), nextCheckpointRevision() (+9 more)

### Community 59 - "generate_design_system"
Cohesion: 0.13
Nodes (7): _detect_page_type(), format_page_override_md(), generate_design_system(), _generate_intelligent_overrides(), persist_design_system(), safe_slug(), TestPersistence

### Community 60 - "compilerOptions"
Cohesion: 0.10
Nodes (19): compilerOptions, allowArbitraryExtensions, allowImportingTsExtensions, erasableSyntaxOnly, jsx, lib, module, moduleDetection (+11 more)

### Community 61 - "package.json"
Cohesion: 0.09
Nodes (28): devDependencies, oxlint@^1.90.0, tailwindcss@^4.3.5, @tailwindcss/vite@^4.3.5, @types/node@^24.19.2, @types/react@^19.3.0, @types/react-dom@^19.3.0, typescript@~7.0.2 (+20 more)

### Community 62 - "install.mjs"
Cohesion: 0.13
Nodes (14): args, copyDir(), DRY, explicit, FLAGS, has(), HERE, HOME (+6 more)

### Community 63 - "actOnAgentTarget"
Cohesion: 0.19
Nodes (22): actOnAgentTarget(), agentTargetBusyReason(), agentTargetOverlayGone(), agentTargetTaken(), claimAgentTarget(), claimAndActOnAgentTarget(), declineAgentTargetBusy(), declineAgentTargetUnresolvable() (+14 more)

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
Cohesion: 0.23
Nodes (7): TripDto, TripsController, TripStatus, Cancelled, Completed, Delayed, Scheduled

### Community 74 - "UPMS.Api.csproj"
Cohesion: 0.20
Nodes (7): net8.0, Microsoft.AspNetCore.Authentication.JwtBearer (8.0.*), Microsoft.AspNetCore.Identity.EntityFrameworkCore (8.0.*), Microsoft.EntityFrameworkCore.Design (8.0.*), Microsoft.EntityFrameworkCore.SqlServer (8.0.*), Swashbuckle.AspNetCore (6.6.2), Microsoft.NET.Sdk.Web

### Community 75 - "install-hook.mjs"
Cohesion: 0.22
Nodes (6): args, HERE, hookEntry, ourHook(), settings, withoutOurs()

### Community 76 - "argparse"
Cohesion: 0.36
Nodes (3): exact_size_scale(), main(), rasterize()

### Community 77 - "_style_is_dark_primary"
Cohesion: 0.21
Nodes (4): _query_wants_dark(), _resolve_color_mode(), _style_is_dark_primary(), TestModeResolution

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

### Community 85 - "interface"
Cohesion: 0.33
Nodes (5): interface, display_name, short_description, policy, allow_implicit_invocation

### Community 87 - "test_skill_script_paths.py"
Cohesion: 0.38
Nodes (3): resolve(), shipped_invocations(), SkillScriptPathsTest

### Community 89 - "Trips.tsx"
Cohesion: 0.07
Nodes (83): dependencies, lucide-react@^1.73.0, react@^19.3.1, react-dom@^19.3.1, react-router-dom@^7.18.5, @tanstack/react-query@^5.107.1, AdminOnly(), App() (+75 more)

### Community 90 - "interface"
Cohesion: 0.50
Nodes (3): interface, display_name, short_description

### Community 92 - "drawio_extract.py"
Cohesion: 0.11
Nodes (11): clean_label(), _configure_stdout_utf8(), _decompress_limited(), _escape_inline(), _escape_markdown(), _escape_table(), _fold_lines(), _inflate() (+3 more)

### Community 95 - "test_core_data_quality.py"
Cohesion: 0.16
Nodes (10): read_rows(), TestAccessibilityGuidance, TestChartsTypographyAndIcons, TestCurrentReactGuidance, TestSemanticColors, _check_typography_contract(), _configured_font_names(), _declared_weights() (+2 more)

### Community 96 - "format_ascii_box"
Cohesion: 0.13
Nodes (9): ansi_ljust(), format_ascii_box(), add_wrapped(), wrap_text(), format_markdown(), hex_to_ansi(), _mode_support_labels(), section_header() (+1 more)

### Community 101 - "parse_page"
Cohesion: 0.15
Nodes (10): _aligned(), analyze(), classify_shape(), Edge, _has_cycle(), Node, _num(), parse_page() (+2 more)

### Community 102 - "usecase_diagram.py"
Cohesion: 0.24
Nodes (11): die(), find_browser(), _has_cycle(), visit(), load_spec(), main(), svg_to_png(), to_plantuml() (+3 more)

### Community 103 - "_fail"
Cohesion: 0.32
Nodes (10): digest(), _fail(), load_mxfile(), main(), Page, page_bounds(), parse_file(), _reject_unsafe_xml() (+2 more)

### Community 104 - "enableInlineEdit"
Cohesion: 0.33
Nodes (6): collectEditableTextRows(), visit(), enableInlineEdit(), onInlineInput(), removalOriginalText(), wrapMixedContentTextNodes()

### Community 105 - "_row_identities"
Cohesion: 0.33
Nodes (3): _exact_row_identity(), _row_identities(), _style_identity()

### Community 106 - "split_values"
Cohesion: 0.47
Nodes (3): split_values(), style_identities(), TestStyleIdentityContract

## Knowledge Gaps
- **81 isolated node(s):** `FullName`, `University`, `Vendor`, `Pending`, `Approved` (+76 more)
  These have ≤1 connection - possible missing edges. (Counts symbols only; 782 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **24 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `Trip` connect `AppDbContext` to `20261114160207_InitialCreate.Designer.cs`, `Booking`, `Guid`, `TripsController`?**
  _High betweenness centrality (0.020) - this node is a cross-community bridge._
- **What connects `FullName`, `University`, `Vendor` to the rest of the system?**
  _81 weakly-connected nodes found - possible missing edges._
- **Should `mermaid_extract.py` be split into smaller, more focused modules?**
  _Cohesion score 0.059097127222982215 - nodes in this community are weakly interconnected._
- **Why does `Booking` connect `Booking` to `20261114160207_InitialCreate.Designer.cs`, `ActionResult`, `ActionResult`, `Guid`, `AppDbContext`?**
  _High betweenness centrality (0.018) - this node is a cross-community bridge._
- **Should `excalidraw_extract.py` be split into smaller, more focused modules?**
  _Cohesion score 0.13725490196078433 - nodes in this community are weakly interconnected._
- **Why does `BookingDto` connect `ActionResult` to `20261114160207_InitialCreate.Designer.cs`, `Booking`, `UPMS.Api.Data`, `Guid`?**
  _High betweenness centrality (0.011) - this node is a cross-community bridge._
- **Should `self_check.py` be split into smaller, more focused modules?**
  _Cohesion score 0.06857142857142857 - nodes in this community are weakly interconnected._