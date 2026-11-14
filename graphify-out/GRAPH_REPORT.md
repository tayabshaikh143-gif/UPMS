# Graph Report - UPMS  (2026-11-14)

## Corpus Check
- cluster-only mode — file stats not available

## Summary
- 459 nodes · 953 edges · 19 communities (18 shown, 1 thin omitted)
- Extraction: 97% EXTRACTED · 3% INFERRED · 0% AMBIGUOUS · INFERRED: 31 edges (avg confidence: 0.85)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `3d9f1858`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- mermaid_extract.py
- drawio_extract.py
- self_check.py
- excalidraw_extract.py
- usecase_diagram.py
- export_svg.py
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
- assert_export_gate

## God Nodes (most connected - your core abstractions)
1. `export_svg_document()` - 21 edges
2. `Diagram` - 20 edges
3. `Diagram` - 17 edges
4. `_fail()` - 16 edges
5. `parse_page()` - 16 edges
6. `parse_scene()` - 14 edges
7. `decode_xml_references()` - 14 edges
8. `parse_block()` - 13 edges
9. `clean_label()` - 13 edges
10. `_parse_flowchart()` - 13 edges

## Surprising Connections (you probably didn't know these)
- `_decoded_css_property_text()` --indirect_call--> `escape()`  [INFERRED]
  skills/diagram-design/scripts/export_svg.py → skills/diagram-design/scripts/export_svg.py  _Bridges community 10 → community 4_
- `_diagram_css_leaf()` --references--> `_CssRule`  [EXTRACTED]
  skills/diagram-design/scripts/export_svg.py → skills/diagram-design/scripts/export_svg.py  _Bridges community 13 → community 5_
- `parse_page()` --calls--> `math`  [EXTRACTED]
  skills/diagram-design/scripts/drawio_extract.py →   _Bridges community 1 → community 3_
- `clean_label()` --calls--> `html`  [EXTRACTED]
  skills/diagram-design/scripts/mermaid_extract.py →   _Bridges community 0 → community 1_
- `_run()` --calls--> `argparse`  [EXTRACTED]
  skills/diagram-design/scripts/mermaid_extract.py →   _Bridges community 0 → community 3_

## Import Cycles
- None detected.

## Communities (19 total, 1 thin omitted)

### Community 0 - "mermaid_extract.py"
Cohesion: 0.06
Nodes (52): analyze(), _block_summary(), classify_shape(), clean_label(), _configure_stdout_utf8(), Diagram, digest(), _discard_nonsemantic() (+44 more)

### Community 1 - "drawio_extract.py"
Cohesion: 0.08
Nodes (31): _aligned(), analyze(), classify_shape(), clean_label(), _configure_stdout_utf8(), _decompress_limited(), digest(), Edge (+23 more)

### Community 2 - "self_check.py"
Cohesion: 0.07
Nodes (26): BalanceParser, canonical_controller(), check_css_references(), check_document_completeness(), check_motion(), check_scripts(), check_svgs(), check_viewbox() (+18 more)

### Community 3 - "excalidraw_extract.py"
Cohesion: 0.10
Nodes (23): analyze(), clean_label(), _configure_stdout_utf8(), digest(), Edge, _escape_inline(), _escape_markdown(), _escape_table() (+15 more)

### Community 4 - "usecase_diagram.py"
Cohesion: 0.09
Nodes (18): escape(), _boxes_overlap(), choose_grid(), Diagram, die(), Ellipse, find_browser(), _has_cycle() (+10 more)

### Community 5 - "export_svg.py"
Cohesion: 0.15
Nodes (16): _animation_component(), _animation_declarations(), _animation_property(), body_inherited_css(), _css_blocks(), _css_segments(), _diagram_css_leaf(), _has_animation_property() (+8 more)

### Community 6 - "decode_xml_references"
Cohesion: 0.23
Nodes (12): decode_xml_references(), escape_css_for_xml(), find_defs_ids(), namespace_defs_ids(), markup_ids(), retarget_style_selectors(), rewrite(), rewrite_comment_token() (+4 more)

### Community 7 - "UPMS_UseCase_Diagram_Simple.json"
Cohesion: 0.14
Nodes (13): actors, associations, columns, extends, footnote, generalizations, includes, legend (+5 more)

### Community 8 - "export_svg_document"
Cohesion: 0.15
Nodes (8): ensure_viewbox(), ensure_xmlns(), export_svg_document(), leading_accessibility_end(), main(), merge_style_into_defs(), slug_for(), xml_safe_comments()

### Community 9 - "upms-simple.json"
Cohesion: 0.14
Nodes (13): actors, associations, columns, extends, footnote, generalizations, includes, legend (+5 more)

### Community 10 - "normalize_rgba_presentation_attrs"
Cohesion: 0.17
Nodes (6): _css_paint_may_target_descendants(), _decoded_css_property_text(), normalize_rgba_presentation_attrs(), normalize_tag(), set_root_id(), replace_attr()

### Community 11 - "UPMS_UseCase_Diagram.json"
Cohesion: 0.17
Nodes (11): actors, associations, extends, footnote, generalizations, includes, subtitle, system (+3 more)

### Community 12 - "css_escape"
Cohesion: 0.24
Nodes (8): css_escape(), css_id_token(), starts_name(), decoded_css_url_fragment(), retarget(), rewrite_css_urls(), rewrite_raw_url(), retarget_root_selector()

### Community 13 - "_CssRule"
Cohesion: 0.24
Nodes (7): _css_rule_nodes(), _css_rule_tree(), _CssRule, diagram_css_from_html(), _diagram_css_rules(), _html_css_rules(), _keyframe_names()

### Community 14 - "_rewrite_start_tags"
Cohesion: 0.18
Nodes (6): extract_first_svg(), html_tokens(), mask_opaque_html(), _rewrite_start_tags(), _xml_start_tag(), xmlify_attributes()

### Community 15 - "upms.json"
Cohesion: 0.17
Nodes (11): actors, associations, extends, footnote, generalizations, includes, subtitle, system (+3 more)

### Community 16 - "mini.json"
Cohesion: 0.18
Nodes (10): actors, associations, extends, generalizations, includes, subtitle, system, theme (+2 more)

### Community 17 - "normalize_html_entities"
Cohesion: 0.70
Nodes (4): normalize_html_entities(), normalize_region(), replace_entity(), replace_in_attribute()

## Knowledge Gaps
- **1 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `namespace_defs_ids()` connect `decode_xml_references` to `export_svg_document`, `css_escape`, `export_svg.py`?**
  _High betweenness centrality (0.020) - this node is a cross-community bridge._
- **Should `mermaid_extract.py` be split into smaller, more focused modules?**
  _Cohesion score 0.059097127222982215 - nodes in this community are weakly interconnected._
- **Why does `decode_xml_references()` connect `decode_xml_references` to `export_svg_document`, `assert_export_gate`, `export_svg.py`?**
  _High betweenness centrality (0.019) - this node is a cross-community bridge._
- **Should `drawio_extract.py` be split into smaller, more focused modules?**
  _Cohesion score 0.0750151240169389 - nodes in this community are weakly interconnected._
- **Should `self_check.py` be split into smaller, more focused modules?**
  _Cohesion score 0.06857142857142857 - nodes in this community are weakly interconnected._
- **Should `excalidraw_extract.py` be split into smaller, more focused modules?**
  _Cohesion score 0.10101010101010101 - nodes in this community are weakly interconnected._
- **Should `usecase_diagram.py` be split into smaller, more focused modules?**
  _Cohesion score 0.09191583610188261 - nodes in this community are weakly interconnected._