---
name: generate-documentation
description: Generate or update Docusaurus documentation for RA.Utilities NuGet packages — understands source diffs, writes changelogs, creates migration guides, and follows the project's 9-section documentation template.
---

# Documentation Generator

Generate or update Docusaurus documentation for any RA.Utilities NuGet package following the standard 9-section template and workflow used across the repository.

## When to Use

Invoke this skill whenever the user asks to:
- Write or generate documentation for a package
- Update documentation to reflect unsaved/uncommitted source changes
- Create changelog entries for a new version
- "Document the changes" or "write docs for X package"

## Workflow

### Phase 1: Understand the Changes

1. **Read git diff** for the package's source files to understand exactly what changed:
   ```bash
   git diff -- <package-path>/*.cs <package-path>/*.csproj
   ```
2. **Read the modified source files** to understand the current API surface:
   - All `.cs` files in the package directory
   - The `.csproj` file (for version, description, dependencies)
3. **Identify**: new types/members, removed types/members, renamed types/members, constructor signature changes, type changes (e.g., `string` → `int`, `enum` → `record`), XML doc improvements, version bumps.

### Phase 2: Review Existing Documentation

1. **Read all existing doc files** at `documentation/docs/<layer>/<PackageName>/`:
   - `index.mdx` — the landing page
   - All `.md` files — API reference pages
2. **Read sibling package docs** (e.g., `Core.Exceptions/index.mdx`) to understand the project's documentation conventions.
3. **Identify bugs** in existing docs:
   - Wrong class/method names that don't match source
   - Claims of features that don't exist (e.g., implicit conversion that isn't defined)
   - Missing or outdated content
   - Broken code examples

### Phase 3: Write/Update Documentation

Apply the **9-section template** below. For each section, decide whether it goes on the `index.mdx` landing page or as a separate file, based on the package's complexity. Simple packages can consolidate sections 1–4, 6, and 9 onto `index.mdx`. Complex packages may need dedicated pages.

#### Section 1 — Landing / Overview (`index.mdx`)
```markdown
---
title: <PackageName>
sidebar_position: <N>
sidebar_class_name: nav_session <snake_case_package>
---

import DocCardList from '@theme/DocCardList';
import LogoSvg from '<relative-path-to-svg>';

<p align="center"><LogoSvg width={'12rem'} height={'12rem'} /></p>

# <PackageName>

[![NuGet version](...)](...)
[![NuGet Downloads](...)](...)
[![Codecov](...)](...)
[![Publish NuGet](...)](...)
[![GitHub license](...)](...)

<1-2 sentence description of what the package does>
<1-2 sentences on why someone would use it>

## 🎯 Purpose

<Bulleted key benefits — avoid typos, improve readability, standardize X, decouple Y>

### ✨ Key Features

- **Feature 1** — short description
- **Feature 2** — short description
- Zero Dependencies / No Configuration / etc.
```

#### Section 2 — Getting Started (`index.mdx`)
```markdown
## 🛠️ Getting Started

### Prerequisites
- **.NET 10.0** or later

### Installation

**.NET CLI**
```bash
dotnet add package <PackageName>
```

**Package Manager Console**
```powershell
Install-Package <PackageName>
```

**PackageReference (csproj)**
```xml
<PackageReference Include="<PackageName>" Version="<X.Y.Z>" />
```

### Hello World
<Simplest possible working example — 5-10 lines of code>
<Note about any required setup (DI, config), or state "No setup required">
```

#### Section 3 — Core Concepts (`index.mdx` or separate page)
```markdown
## 🧠 Core Concepts

<Table of main abstractions: class/interface name, role, example>

### How they work together
<Architecture diagram (ASCII art or mermaid if supported), or a narrative flow>

### Terminology
- **Term 1**: definition
- **Term 2**: definition
```

#### Section 4 — How-To Guides (`index.mdx` or separate page)
```markdown
## 📖 How-To Guides

### <Task-oriented title: "Return a 404 with consistent messaging">

```csharp showLineNumbers
// Real, runnable code snippet
```

### <Next task>

<Cross-link to detailed API reference pages where appropriate>
```

#### Section 5 — API Reference (existing `.md` files)
- Keep separate from guides — reference vs. narrative are different reading modes
- Each class/interface gets its own `.md` file
- Include: namespace, purpose, table of members/constants, usage example
- Match the existing per-file pattern: no YAML frontmatter (except `title:` if needed), descriptive prose, constant/value tables, code examples

#### Section 6 — Configuration Reference (`index.mdx`)
```markdown
## ⚙️ Configuration

<All options/settings in one table: name, type, default, description>
<If zero configuration, state it explicitly: "This package has no configuration options.">
```

#### Section 7 — Migration Guides / Changelog (`migration-guides.mdx` or `changeLogs/<date>-<PackageName>.md`)
```markdown
---
title: Migration Guides
sidebar_class_name: nav_session <snake_case_package>
---

# Migration Guides

## v<old> → v<new>

### Breaking Change: <title>

### Why this change?
<1-2 sentences>

### What changed
<Before/after table>

### Migration Steps
#### 1. <Step title>
**Before:**
```csharp
// old code
```
**After:**
```csharp
// new code
```

## Full Changelog
<Link to releasenotes.md or changelog file>
```

#### Section 8 — Troubleshooting / FAQ (`troubleshooting.mdx`)
```markdown
---
title: Troubleshooting & FAQ
sidebar_class_name: nav_session <snake_case_package>
---

# Troubleshooting & FAQ

## Common Issues

### <Issue title>
**Cause**: ...
**Fix**: ...

### Can I <do X>?
**Answer** with code examples showing correct approach.

## Known Limitations
- **Limitation**: description and workaround

## Performance Tips
- **Tip**: brief guidance
```

#### Section 9 — Contributing / Support (`index.mdx` — at bottom)
```markdown
## 🙌 Contributing & Support

### Found a bug or have a suggestion?
- **GitHub Issues**: [link]
- **Discussions**: [link]

### Contributing
1. Fork → 2. Branch → 3. Change → 4. PR

### Links
| Resource | URL |
|---|---|
| **NuGet** | [link] |
| **Source** | [GitHub — RedonAlla/RA.Utilities](https://github.com/RedonAlla/RA.Utilities) |
| **License** | [MIT](https://github.com/RedonAlla/RA.Utilities?tab=MIT-1-ov-file) |
| **Build Status** | [GitHub Actions](...) |
```

### Phase 4: Fix Existing Documentation Bugs

Before writing new content, fix bugs found in Phase 2:
- Wrong class/method names → correct them
- False claims (e.g., implicit conversion that doesn't exist) → remove them
- Broken code examples → fix to match current API
- Missing members in tables → add them
- Members in tables that don't exist in source → remove them

### Phase 5: Create/Update Changelog

1. Check if a changelog file exists at `documentation/changeLogs/<date>-<PackageName>.md`
2. Add new version entry at the top, following the existing format:
   ```markdown
   ## Version X.Y.Z
   ![Date Badge](https://img.shields.io/badge/Publish-<DD>%20<Month>%20<YYYY>-lightblue?logo=fastly&logoColor=white)
   [![NuGet version](https://img.shields.io/badge/NuGet-vX.Y.Z-blue?logo=nuget)](<nuget-url>)

   <One-line summary>

   <!-- truncate -->

   ### ⚠️ Breaking Changes
   * **<Title>**: <description>. **Migration**: <steps>.

   ### ✨ New Features
   * **<Title>**: <description>

   ### 📝 Improvements
   * **<Title>**: <description>
   ```
3. The `<!-- truncate -->` marker goes after the summary paragraph, before the first detail section — this is the Docusaurus blog preview cutoff.

### Phase 6: Verify

1. **Cross-reference code examples** with actual source APIs to ensure they compile
2. **Check class/method names** used in docs match actual source names
3. **Verify table completeness** — every public constant/member in source should appear in docs
4. **Check frontmatter** — new `.mdx` pages need `title:` and `sidebar_class_name:`
5. **Links** — ensure cross-references between pages use correct relative paths (e.g., `./BaseResponseCode`, `../docs/core/RA.Utilities.Core.Constants/migration-guides`)

## Conventions

### File Naming
- `index.mdx` — landing page (Docusaurus convention)
- `<ClassName>.md` — API reference pages
- `migration-guides.mdx` — breaking changes and version migration
- `troubleshooting.mdx` — FAQ and common issues
- `<YYYY-MM-DD>-<PackageShortName>.md` — changelog files in `documentation/changeLogs/`

### Frontmatter
```yaml
# For index.mdx:
---
title: RA.Utilities.Core.Constants
sidebar_position: 2
sidebar_class_name: nav_session ra_utilities_core_constants
---

# For sub-pages (.mdx):
---
title: Migration Guides
sidebar_class_name: nav_session ra_utilities_core_constants
---

# For API reference (.md):
---
title: ResponseType
---
```

### Code Blocks
- Use ` ```csharp showLineNumbers ` for code examples
- Use `// highlight-next-line` for important lines
- Use ` ```json showLineNumbers ` for JSON examples
- Use ` ```bash ` for CLI commands
- Use ` ```powershell ` for Package Manager Console commands

### Badge Pattern
```
[![NuGet version](https://img.shields.io/nuget/v/<PACKAGE>?logo=nuget&label=NuGet)](<URL>)
[![NuGet Downloads](https://img.shields.io/nuget/dt/<PACKAGE>.svg?logo=nuget)](<URL>)
```

### SVG Logo
Each package has an SVG in `Assets/Images/<name>.svg`. Reference it with:
```jsx
import LogoSvg from '<relative-path-to-Assets/Images/name.svg>';
```

### Directory Structure
```
documentation/
├── docs/
│   └── <layer>/                    # core, api, data, application, logging, infrastructure
│       └── <PackageName>/
│           ├── index.mdx           # Sections 1–4, 6, 9
│           ├── <ClassName>.md      # Section 5 — one per class
│           ├── migration-guides.mdx # Section 7
│           └── troubleshooting.mdx  # Section 8
└── changeLogs/
    └── <YYYY-MM-DD>-<ShortName>.md # Changelog entries
```
