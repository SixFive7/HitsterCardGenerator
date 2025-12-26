---
description: Run full milestone lifecycle automatically with fresh context per step
argument-hint: ""
allowed-tools:
  - Read
  - Write
  - Glob
  - Grep
  - Task
  - AskUserQuestion
---

<objective>
Run the FULL milestone lifecycle with complete context isolation.

Just run `/gsd:lazy` - it:
1. Detects where you are in the lifecycle
2. Runs everything: discuss-milestone → new-milestone → phases → complete-milestone
3. Each step gets fresh 200k context via subagent
4. All questions surface to you and block until answered
5. Exits after milestone with instructions for next

**Serial execution, blocking questions** - automation with human control points.
</objective>

<context>
@.planning/STATE.md
@.planning/ROADMAP.md
@.planning/config.json
</context>

<process>
## 0. Show Usage Summary

Display at start:

```
🚀 /gsd:lazy - Full Milestone Automation

📋 What happens:
   • Runs entire milestone: discuss → new → phases → complete
   • Each step gets fresh context (subagent)
   • Questions surface and wait for your input

⏱️ Timeout:
   • 10 min limit during questions
   • If away longer, just re-run /gsd:lazy (state is saved)
   • No auto-retry, no wasted tokens

🎯 Detecting current position...
```

## 1. Auto-Detect Position

Read STATE.md and ROADMAP.md to determine lifecycle position:

**State A: No active milestone** (no 🚧 in ROADMAP.md)
→ Start from discuss-milestone

**State B: Milestone exists, phases incomplete** (🚧 exists, phases without SUMMARY.md)
→ Find first incomplete phase, detect step within phase:
- No CONTEXT.md → start at discuss-phase
- Has CONTEXT.md, no RESEARCH.md → start at research-phase
- Has RESEARCH.md, no PLAN.md → start at plan-phase
- Has PLAN.md, no SUMMARY.md → start at execute-plan

**State C: All phases complete** (all have SUMMARY.md)
→ Run complete-milestone

Show detected position:
```
📍 Current position: [State A/B/C description]
   Next step: [what will run next]
```

## 2. Milestone Setup (if State A)

### discuss-milestone (subagent)

Spawn general-purpose Task with prompt:
```
You are running the discuss-milestone workflow for /gsd:lazy.

Read the discuss-milestone workflow from:
@~/.claude/get-shit-done/workflows/discuss-milestone.md

Your job:
1. Read .planning/STATE.md and .planning/ROADMAP.md for context
2. Ask the user what they want to add, improve, or fix
3. Use AskUserQuestion for ALL questions (never ask inline)
4. Probe and prioritize features
5. When user is ready, summarize the scope

Return: A summary of features/scope for the new milestone
```

### new-milestone (subagent)

Spawn general-purpose Task with prompt:
```
You are running the new-milestone workflow for /gsd:lazy.

Read the new-milestone workflow from:
@~/.claude/commands/gsd/new-milestone.md

Context from discuss-milestone: [pass feature summary]

Your job:
1. Calculate next milestone version from ROADMAP.md
2. Ask for milestone name if not clear from context
3. Break features into 3-6 phases
4. Use AskUserQuestion to confirm phase breakdown
5. Create ROADMAP.md entries and phase directories
6. Update STATE.md
7. Commit changes

Return: List of phases created with their numbers
```

## 3. Phase Loop (for each phase)

For the current phase (or first incomplete phase):

### discuss-phase (subagent)

Skip if {phase}-CONTEXT.md exists.

Spawn general-purpose Task with prompt:
```
You are running discuss-phase for Phase [N]: [name].

Read the discuss-phase workflow from:
@~/.claude/commands/gsd/discuss-phase.md
@~/.claude/get-shit-done/workflows/discuss-phase.md

Phase directory: .planning/phases/[NN]-[slug]/

Your job:
1. Read ROADMAP.md for phase description
2. Ask user how they imagine this phase working
3. Use AskUserQuestion for ALL questions
4. Probe what's essential vs out of scope
5. Create {phase}-CONTEXT.md in phase directory

Return: Confirmation that CONTEXT.md was created
```

### research-phase (subagent)

Skip if {phase}-RESEARCH.md exists.

Spawn general-purpose Task with prompt:
```
You are running research-phase for Phase [N]: [name].

Read the research-phase workflow from:
@~/.claude/commands/gsd/research-phase.md
@~/.claude/get-shit-done/workflows/research-phase.md

Phase directory: .planning/phases/[NN]-[slug]/
Context: Read {phase}-CONTEXT.md first

Your job:
1. Identify knowledge gaps for this phase
2. Research ecosystem: libraries, patterns, pitfalls
3. Use Context7, WebSearch, official docs
4. Create {phase}-RESEARCH.md in phase directory

Return: Confirmation that RESEARCH.md was created
```

### plan-phase (subagent)

Skip if {phase}-*-PLAN.md exists.

Spawn general-purpose Task with prompt:
```
You are running plan-phase for Phase [N]: [name].

Read the plan-phase workflow from:
@~/.claude/commands/gsd/plan-phase.md
@~/.claude/get-shit-done/workflows/plan-phase.md

Phase directory: .planning/phases/[NN]-[slug]/
Context: Read CONTEXT.md and RESEARCH.md first

Your job:
1. Read project context (STATE.md, codebase/)
2. Break phase into concrete tasks
3. Create {phase}-01-PLAN.md (or multiple plans if large)
4. Follow plan format template

Return: List of PLAN.md files created
```

### execute-plan (subagent, per plan)

For each PLAN.md without corresponding SUMMARY.md:

Spawn general-purpose Task with prompt:
```
You are executing a plan for /gsd:lazy.

Read the execute-plan workflow from:
@~/.claude/commands/gsd/execute-plan.md
@~/.claude/get-shit-done/workflows/execute-phase.md

Plan path: [full path to PLAN.md]

Your job:
1. Read the PLAN.md
2. Execute all tasks in order
3. Handle deviations per deviation rules
4. Create SUMMARY.md when complete
5. Update STATE.md with plan/phase progress
6. Update ROADMAP.md with plan completion (mark plan checkbox done)
7. Commit changes with proper message

**CRITICAL BOUNDARIES - DO NOT:**
- Do NOT mark the milestone as SHIPPED or complete
- Do NOT create milestone archive files (.planning/milestones/)
- Do NOT collapse milestone sections in ROADMAP.md
- Do NOT create git tags
- These are handled by complete-milestone subagent AFTER all phases

Return: Confirmation that SUMMARY.md was created and changes committed
```

### Transition

After all plans for a phase are complete:
1. Check if more phases remain in milestone
2. If yes: Continue to next phase (back to discuss-phase/plan-phase/execute-plan)
3. If no (all phases done): **MUST spawn complete-milestone subagent** (Section 4)

## 4. Milestone Completion (REQUIRED after all phases done)

**CRITICAL:** This step MUST be run as a separate subagent after all phases complete.
The orchestrator must ALWAYS spawn this subagent - never skip it or let execute-plan do this work.

### complete-milestone (subagent)

Spawn general-purpose Task with prompt:
```
You are running complete-milestone for /gsd:lazy.

Read the complete-milestone workflow from:
@~/.claude/commands/gsd/complete-milestone.md
@~/.claude/get-shit-done/workflows/complete-milestone.md

Your job:
1. Verify all phases have SUMMARY.md
2. Ask user to confirm shipping (AskUserQuestion)
3. Archive milestone to .planning/milestones/
4. Update ROADMAP.md (mark milestone SHIPPED, collapse to one-line)
5. Update STATE.md (reset for next milestone)
6. Create git tag
7. Ask about pushing tag to remote (AskUserQuestion) - THIS IS REQUIRED
8. If user says yes, push the tag
9. Commit all changes

Return: Confirmation of milestone version, git tag, and whether it was pushed
```

## 5. Milestone Complete - Exit

**No auto-loop between milestones.** Display completion message:

```
✅ Milestone [version] complete!

📊 Summary:
- Phases completed: [X-Y]
- Plans executed: [N]
- Git tag: [version]

🔄 To start a new milestone:
   /clear
   /gsd:lazy
```

Exit. User manually runs `/clear` and `/gsd:lazy` for fresh context on next milestone.

</process>

<subagent_config>
**Type:** general-purpose (has ALL tools including AskUserQuestion)

**Timeout:** 600000ms (10 minutes) - max allowed

**Each subagent receives:**
- Clear task description
- Path to relevant workflow/skill file
- Expected deliverable
- Instruction to use AskUserQuestion for ALL user interaction

**Blocking behavior:**
- Subagent's AskUserQuestion surfaces to user
- Subagent pauses until user responds
- Answer returns to subagent
- Subagent continues execution
- Returns to orchestrator when complete

**On subagent failure:**
- Log error to console
- Offer: "Retry this step?" or "Skip and continue?"
- If skip, note in STATE.md issues section
</subagent_config>

<success_criteria>
- [ ] Usage summary displayed at start
- [ ] Lifecycle position detected correctly
- [ ] All steps run in isolated subagents
- [ ] All questions surface and block appropriately
- [ ] Milestone created/completed as needed
- [ ] Each phase has: CONTEXT.md, RESEARCH.md, PLAN.md(s), SUMMARY.md(s)
- [ ] All changes committed
- [ ] Git tag created on milestone completion
- [ ] Shows next steps (/clear + /gsd:lazy) at end
</success_criteria>
