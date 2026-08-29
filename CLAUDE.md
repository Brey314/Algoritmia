# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project state

Fresh Unity 2D project (Unity 6000.5.10f1, Universal Render Pipeline). `Assets/` currently
contains only the default scene (`Assets/Scenes/SampleScene.unity`) and URP/2D template
settings — no gameplay scripts or custom assemblies exist yet. There is no build/test/lint
CLI here; everything happens through the Unity Editor and its MCP tooling (see below).

## Workflow: use the unity-coding-skills plugin

This repo has the `unity-coding-skills` skill set installed — it defines the actual
development workflow and conventions for this project. Load the relevant skill before
touching Unity code or assets rather than improvising:

- **Writing/editing any C# script**: `unity-coding-skills:code-writing-guide` first.
- **New feature / spec change while in plan mode**: `unity-coding-skills:plan-feature`
  (orchestrates plan → `test-designer` → `failing-test-writer` → refactor/dedup).
- **Bug report**: `unity-coding-skills:fix-bug` (reproduce → diagnose → fix, test-first).
- **Editing `.unity` / `.prefab` files**: `unity-coding-skills:edit-scene`.
- **Editing other Unity YAML assets** (ScriptableObjects, Materials) directly: `unity-coding-skills:unity-yaml-editing-guide`.
- **Writing/reviewing test code**: `unity-coding-skills:test-writing-guide` / `test-designing-guide` / `refine-tests`.
- **Running tests**: `unity-coding-skills:run-tests` (drives the `run_unity_tests` tool — don't invoke it ad hoc).
- **Fixing IDE warnings/inspections**: `unity-coding-skills:resolve-diagnostics`.

These skills assume Unity operations go through the Coplay MCP tools
(`mcp__coplay-mcp__*`: create/edit game objects, scenes, prefabs, materials, animations,
input actions, run tests, read compile errors/logs, etc.) rather than hand-editing Editor
state — use them for scene/prefab/asset manipulation and for running Play Mode / Edit Mode
tests, since there is no separate CLI test runner in this project.

## Packages of note (`Packages/manifest.json`)

- `com.unity.render-pipelines.universal` (URP) + `com.unity.2d.*` — this is a 2D/URP project;
  prefer 2D renderer/lighting assets under `Assets/Settings/`.
- `com.unity.inputsystem` — use the new Input System (`Assets/Settings/InputSystem_Actions.inputactions`),
  not the legacy `Input` class.
- `com.unity.ai.assistant`, `com.unity.ai.inference`, `com.coplaydev.coplay` — Unity AI/Coplay
  tooling; Coplay is the MCP bridge these tools talk through.
- `com.unity.test-framework` — Unity Test Framework (NUnit-based) for Edit/Play Mode tests.
