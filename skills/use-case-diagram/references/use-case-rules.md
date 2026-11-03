# Use Case Modelling Rules (UML 2.5)

Reference for building a use case model that survives review. Read this when the
model has more than ~8 use cases, when include/extend is ambiguous, or when the
diagram will be graded, reviewed by a supervisor, or turned into test cases.

## Contents
1. What the diagram is for
2. Actors
3. Use cases
4. System boundary
5. Relationships (association, include, extend, generalization)
6. Naming
7. Layout and readability
8. Review checklist
9. Extracting a model from an SRS
10. When to split the diagram

---

## 1. What the diagram is for

A use case diagram is a **goal-level** view: who uses the system and what
meaningful goals they achieve with it. It is not a flowchart, not a screen map,
and not a process model.

- Good: "Book Ticket", "Cancel Booking", "Assign Replacement Bus".
- Bad: "Click Login Button", "Enter Password", "Validate Email Format",
  "Save to Database". Those are steps, UI actions, or internal design.

If a bubble describes one click or one validation rule, remove it. If a bubble
describes a whole workflow of 15 steps, it is fine at goal level — put the steps
in a written use case description or an activity diagram instead.

## 2. Actors

An actor is a **role** played by a person, device, or external system — never the
system itself and never a named individual.

- Name roles, not job titles or people: "Administrator", "Passenger",
  "Payment Gateway". Not "Mr. Ali" and not "Admin2".
- Every actor sits **outside** the system boundary.
- An external system (payment gateway, SMS provider, university ERP) can be an
  actor. Mark it `"external": true` so readers understand it is not built by the
  project.
- One person may play several roles; model the roles separately.
- Actors can generalize: if a child actor can do everything the parent can plus
  more, draw an inheritance arrow (hollow triangle) from child to parent. Use it
  sparingly — it helps only when the shared behaviour is real.

**Actor test:** if the actor disappears, does the use case still make sense? If
yes, it may not be an actor (it could be a timer, or a step inside the system).

## 3. Use cases

- One use case = one observable goal that delivers value to an actor.
- Name with a **verb + object**: "Book Ticket", "Issue Offline Ticket",
  "View Operational Reports".
- Keep the abstraction level flat. Do not mix "Manage System" with
  "Enter OTP".
- A use case may serve several actors; that produces several association lines.
  That is normal.
- A use case should be traceable to at least one requirement (FR/BR) and later
  to at least one test scenario. Untraceable bubbles are decoration.

## 4. System boundary

- One rectangle encloses all use cases and carries the system name.
- Everything the project builds is inside; every actor is outside.
- If the scope has releases or modules, prefer separate diagrams (per release
  or per module) over many nested rectangles.
- Do not draw a boundary around a single use case just for decoration.

## 5. Relationships

### Association (solid line, no arrowhead)
Actor ↔ use case: this actor participates in this goal. Per UML style guidance,
avoid arrowheads; the line is not a data flow.

### «include» (dashed arrow, points at the included use case)
Mandatory shared behaviour. Use only when the base use case **always** performs
the included behaviour, and the behaviour is a meaningful chunk reused by two or
more use cases.

Example: "Book Ticket" and "Issue Offline Ticket" both must check capacity, so
"Check Trip Capacity" is included by both.

Do **not** use include for optional behaviour, and do not chain includes deeply
(A includes B, B includes C). More than one level of includes is a smell.

### «extend» (dashed arrow, points at the base use case)
Optional or conditional behaviour that plugs into a base use case at an extension
point. The base use case must be complete and valid **without** the extension.

Example: "Pay Online" extends "Book Ticket" — the booking exists and is valid
when the passenger chooses cash at boarding instead.

Warning: an extend arrow points from the extension to the base. Getting the
direction backwards is the single most common use case diagram error.

### Generalization (solid line, hollow triangle at the parent)
"Is-a" between actors or between use cases. Use it when a child really inherits
the parent's full behaviour and adds specialisation. Do not use it as a shortcut
to avoid drawing several association lines.

### Quick decision test
| Question | Answer | Relationship |
|---|---|---|
| Does the base always need it? | Yes, and it is reusable | «include» |
| Does the base work without it? | Yes, conditionally added | «extend» |
| Is the child a specialised kind of parent? | Yes | Generalization |
| Does the actor simply take part in the goal? | Yes | Association |

Do not invent a relationship just to make the picture look richer. A clean
diagram with fewer relationships is better than a busy inaccurate one.

## 6. Naming

- Use case: verb + object, singular, no "system" prefix ("Book Ticket", not
  "System Books Ticket").
- Actor: singular role ("Passenger", "Vendor", "Administrator").
- Keep sibling names parallel in grammar ("Report Delay", "Report Breakdown").
- Avoid synonyms for the same concept across the diagram; pick one term and use
  it in the SRS too.

## 7. Layout and readability

- Place actors so their lines are short: group use cases by primary actor and
  keep that group on the actor's side.
- Inside the boundary, use 2–4 columns and order rows by related behaviour.
- Keep <<include>> targets next to their base use case; keep <<extend>>
  extensions directly above/below the base.
- Avoid crossing lines. If two actor-use case lines must cross, that is a hint
  the grouping is wrong.
- One diagram should stay readable at A4/Letter width. More than ~18 use cases
  usually needs splitting.

## 8. Review checklist

Run this before delivering the diagram:

1. Every bubble is a goal, not a step or a click.
2. Every bubble is inside the boundary; every actor is outside.
3. Every use case has at least one actor association (or is an include/extend
   target of one).
4. Every actor has at least one association.
5. No actor is the system itself or a named person.
6. Include arrows point to mandatory, reused behaviour.
7. Extend arrows point **to the base** and represent optional behaviour.
8. No include/extend cycles.
9. Relationship depth is one level, two at most.
10. Names use verb + object and match the SRS vocabulary.
11. Every use case traces to at least one requirement ID.
12. The diagram is readable without zooming; no line runs through a label.

## 9. Extracting a model from an SRS

1. Read the role definitions first; each role becomes a candidate actor. Add
   external systems named in the requirements (payment gateway, SMS gateway).
2. Read each functional requirement (FR-xx). Ask: "what goal does this serve for
   which actor?" Merge FRs that serve one goal; split FRs that serve several.
3. Read the business rules (BR-xx). Rules usually do not become use cases, but
   they constrain them and may justify an include (capacity check) or an extend
   (refund on cancellation).
4. Read the workflows. Each distinct happy path is usually one use case; optional
   branches become extends, not new use cases, unless they are goals of their own.
5. Cross-check with the entity list — if a use case touches no entity, it may be
   too abstract; if it needs five entities, it may be too big.
6. Write the requirement trace (FR → use case) in the accompanying document.

## 10. When to split the diagram

Split when any of these is true:

- more than ~18 use cases;
- more than ~6 actors on one side;
- lines must cross the middle of the diagram to reach an actor;
- the diagram covers clearly different modules (booking vs. administration).

Split by actor group (passenger-facing / operations / administration) or by
module, and keep the same system boundary name with a suffix ("UPMS — Booking").
