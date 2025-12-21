# Phase 5: Web Foundation - Research

**Researched:** 2025-12-21
**Domain:** Svelte 5 + Vite + .NET Minimal API integration
**Confidence:** HIGH

<research_summary>
## Summary

Researched the modern stack for building an SPA frontend with Svelte/Vite that communicates with a .NET Minimal API backend. The standard approach is **plain Svelte 5 + Vite** (not SvelteKit) since we have a separate .NET backend—SvelteKit's server features are overkill.

Key findings: Tailwind CSS v4 has a dramatically simplified setup with the `@tailwindcss/vite` plugin. For animations, Svelte's **built-in transitions** (fade, fly, scale, slide) are sufficient for a polished UI feel—Motion.dev does NOT officially support Svelte. For rapid UI development with consistent design, **shadcn-svelte** provides copy-paste components that work with plain Svelte + Tailwind.

**Primary recommendation:** Use plain Svelte 5 + Vite + Tailwind v4 + Svelte built-in transitions. Consider shadcn-svelte for component consistency. Connect to .NET via Vite's proxy configuration.

</research_summary>

<standard_stack>
## Standard Stack

### Core
| Library | Version | Purpose | Why Standard |
|---------|---------|---------|--------------|
| svelte | 5.x | UI framework | Latest stable, runes API, excellent DX |
| vite | 6.x | Build tool | Fast HMR, native ESM, perfect Svelte support |
| tailwindcss | 4.x | CSS framework | Utility-first, v4 has simplified Vite setup |
| @tailwindcss/vite | 4.x | Vite plugin | Zero-config Tailwind integration |

### Supporting
| Library | Version | Purpose | When to Use |
|---------|---------|---------|-------------|
| shadcn-svelte | latest | UI components | Polished, accessible components without writing from scratch |
| bits-ui | 1.x | Headless components | Base for shadcn-svelte, accessible primitives |
| svelte-routing | 2.x | Client-side routing | SPA navigation without SvelteKit |

### Alternatives Considered
| Instead of | Could Use | Tradeoff |
|------------|-----------|----------|
| Plain Svelte | SvelteKit | SvelteKit adds SSR/routing complexity we don't need with .NET backend |
| Svelte transitions | Motion.dev | Motion.dev doesn't officially support Svelte |
| shadcn-svelte | DaisyUI | DaisyUI simpler but less customizable, shadcn gives code ownership |

**Installation:**
```bash
# Create Svelte project
npm create vite@latest web -- --template svelte-ts
cd web

# Add Tailwind v4
npm install -D tailwindcss @tailwindcss/vite

# Add routing (for SPA)
npm install svelte-routing

# Optional: shadcn-svelte setup
npx shadcn-svelte@latest init
```

</standard_stack>

<architecture_patterns>
## Architecture Patterns

### Recommended Project Structure
```
HitsterCardGenerator/
├── HitsterCardGenerator/       # .NET project (existing)
│   ├── Program.cs              # Minimal API entry
│   ├── Endpoints/              # API route handlers
│   │   ├── SongsEndpoints.cs
│   │   ├── SpotifyEndpoints.cs
│   │   └── ExportEndpoints.cs
│   ├── Services/               # Business logic (existing)
│   └── Models/                 # Domain models (existing)
│
└── web/                        # Svelte frontend (new)
    ├── src/
    │   ├── lib/
    │   │   ├── components/     # UI components
    │   │   └── stores/         # Svelte stores
    │   ├── routes/             # Page components
    │   ├── App.svelte          # Root component
    │   └── main.ts             # Entry point
    ├── vite.config.ts
    └── package.json
```

### Pattern 1: Vite Proxy to .NET API
**What:** Configure Vite dev server to proxy `/api` requests to .NET backend
**When to use:** Always in development for CORS-free API calls
**Example:**
```typescript
// vite.config.ts
import { defineConfig } from 'vite'
import { svelte } from '@sveltejs/vite-plugin-svelte'
import tailwindcss from '@tailwindcss/vite'

export default defineConfig({
  plugins: [svelte(), tailwindcss()],
  server: {
    proxy: {
      '/api': {
        target: 'http://localhost:5000',
        changeOrigin: true
      }
    }
  }
})
```

### Pattern 2: Svelte 5 Runes for State
**What:** Use $state, $derived, $effect instead of let/reactive statements
**When to use:** All new Svelte 5 code
**Example:**
```svelte
<script lang="ts">
  // Svelte 5 runes syntax
  let count = $state(0)
  let doubled = $derived(count * 2)

  function increment() {
    count++
  }
</script>

<button onclick={increment}>
  Count: {count}, Doubled: {doubled}
</button>
```

### Pattern 3: Tailwind v4 Setup
**What:** Minimal Tailwind configuration with Vite plugin
**When to use:** Always with Tailwind v4
**Example:**
```css
/* src/app.css */
@import "tailwindcss";

/* Custom theme variables (optional) */
@theme {
  --color-primary: #1DB954;  /* Spotify green */
  --color-accent: #FF6B6B;
}
```

### Anti-Patterns to Avoid
- **Using SvelteKit for SPA with separate backend:** Adds complexity, routing conflicts, unnecessary SSR
- **Installing postcss/autoprefixer for Tailwind v4:** Not needed with @tailwindcss/vite plugin
- **Using @tailwind directives:** That's v3 syntax, v4 uses @import "tailwindcss"
- **Configuring CORS on .NET for dev:** Use Vite proxy instead, simpler

</architecture_patterns>

<dont_hand_roll>
## Don't Hand-Roll

| Problem | Don't Build | Use Instead | Why |
|---------|-------------|-------------|-----|
| Animations | Custom CSS keyframes | Svelte transitions | Built-in fly, fade, scale, slide handle enter/exit beautifully |
| UI Components | Custom buttons, modals | shadcn-svelte | Accessible, consistent, saves hours of work |
| Form handling | Manual validation | SuperForms or HTML5 | Edge cases in validation are endless |
| State management | Custom pub/sub | Svelte stores + runes | Built into framework, reactive by default |
| CORS handling | .NET CORS config | Vite proxy | Dev proxy is simpler, no config leaking to prod |

**Key insight:** Svelte 5's built-in capabilities handle most needs. The framework is batteries-included for transitions, reactivity, and component patterns. External libraries are rarely needed for core functionality.

</dont_hand_roll>

<common_pitfalls>
## Common Pitfalls

### Pitfall 1: SvelteKit Overkill
**What goes wrong:** Using SvelteKit for a simple SPA with separate API leads to routing conflicts, SSR confusion
**Why it happens:** SvelteKit is the "default" recommendation, but assumes you want its server features
**How to avoid:** Use plain Svelte + Vite when backend is separate (.NET, Go, etc.)
**Warning signs:** Fighting with +page.server.ts, load functions, adapter confusion

### Pitfall 2: Tailwind v3 vs v4 Confusion
**What goes wrong:** Mixing @tailwind directives (v3) with @import (v4), or installing unneeded postcss
**Why it happens:** Most tutorials/examples are still v3
**How to avoid:** For Vite: use @tailwindcss/vite plugin, @import "tailwindcss", no postcss config needed
**Warning signs:** "Unknown at-rule @tailwind" errors, bloated devDependencies

### Pitfall 3: CORS Dance in Development
**What goes wrong:** API calls fail with CORS errors, leading to .NET CORS configuration that then causes issues
**Why it happens:** Svelte dev server and .NET run on different ports
**How to avoid:** Configure Vite proxy for `/api/*` to forward to .NET backend
**Warning signs:** Adding CORS middleware to .NET for localhost origins

### Pitfall 4: Motion.dev Disappointment
**What goes wrong:** Trying to use Motion.dev (Framer Motion) with Svelte, finding no official support
**Why it happens:** Motion.dev is popular and often recommended, but only officially supports React/Vue/JS
**How to avoid:** Use Svelte's excellent built-in transitions (fade, fly, scale, slide)
**Warning signs:** Unmaintained community wrappers, missing features, runtime errors

### Pitfall 5: HMR Issues with .NET Integration
**What goes wrong:** Hot module replacement doesn't work, requiring full page reloads
**Why it happens:** Complex proxy setups or SpaServices misconfiguration
**How to avoid:** Run Vite and .NET as separate processes, use simple proxy config
**Warning signs:** Changes not reflecting, WebSocket connection errors

</common_pitfalls>

<code_examples>
## Code Examples

### Basic Svelte 5 Component with Transitions
```svelte
<!-- Source: Official Svelte docs -->
<script lang="ts">
  import { fade, fly } from 'svelte/transition'

  let visible = $state(false)
</script>

<button onclick={() => visible = !visible}>
  Toggle
</button>

{#if visible}
  <div
    in:fly={{ y: 20, duration: 300 }}
    out:fade={{ duration: 200 }}
    class="p-4 bg-primary rounded-lg"
  >
    Hello with smooth animation!
  </div>
{/if}
```

### Vite Config with Proxy and Tailwind
```typescript
// Source: Vite docs + Tailwind v4 docs
import { defineConfig } from 'vite'
import { svelte } from '@sveltejs/vite-plugin-svelte'
import tailwindcss from '@tailwindcss/vite'

export default defineConfig({
  plugins: [svelte(), tailwindcss()],
  server: {
    port: 5173,
    proxy: {
      '/api': {
        target: 'http://localhost:5000',
        changeOrigin: true
      }
    }
  }
})
```

### .NET Minimal API Endpoint Structure
```csharp
// Source: Microsoft Minimal API docs
// Endpoints/SongsEndpoints.cs
public static class SongsEndpoints
{
    public static void MapSongsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/songs");

        group.MapPost("/upload", HandleUpload);
        group.MapGet("/", GetAllSongs);
    }

    private static async Task<IResult> HandleUpload(
        IFormFile file,
        CsvParser parser)
    {
        // Implementation
        return Results.Ok(songs);
    }
}

// Program.cs - keep it clean
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapSongsEndpoints();
app.MapSpotifyEndpoints();
app.MapExportEndpoints();

app.Run();
```

### Fetching from .NET API
```svelte
<script lang="ts">
  let songs = $state<Song[]>([])
  let loading = $state(false)

  async function uploadCsv(file: File) {
    loading = true
    const formData = new FormData()
    formData.append('file', file)

    const response = await fetch('/api/songs/upload', {
      method: 'POST',
      body: formData
    })

    songs = await response.json()
    loading = false
  }
</script>
```

</code_examples>

<sota_updates>
## State of the Art (2024-2025)

| Old Approach | Current Approach | When Changed | Impact |
|--------------|------------------|--------------|--------|
| Tailwind v3 (@tailwind) | Tailwind v4 (@import) | Dec 2024 | Simpler setup, no postcss config needed |
| Svelte 4 (reactive $:) | Svelte 5 (runes $state) | Oct 2024 | More explicit reactivity, better TypeScript |
| SvelteKit for everything | Plain Svelte for SPAs | 2024 | Right tool for job, less complexity |
| Framer Motion | Svelte built-in transitions | Always | Native solution beats wrappers |

**New tools/patterns to consider:**
- **shadcn-svelte:** Now supports Svelte 5 and Tailwind v4, excellent for rapid UI
- **Bits UI:** Headless, accessible component primitives for Svelte
- **Svelte 5 runes:** $state, $derived, $effect are the new standard

**Deprecated/outdated:**
- **Svelte 4 reactive syntax:** Still works but legacy mode, migrate to runes
- **@tailwind directives:** v3 only, v4 uses @import
- **postcss-import for Tailwind:** Not needed with @tailwindcss/vite

</sota_updates>

<open_questions>
## Open Questions

1. **shadcn-svelte vs rolling own components**
   - What we know: shadcn-svelte provides polished, accessible components
   - What's unclear: User preference on component library vs custom
   - Recommendation: Start with shadcn-svelte for speed, customize as needed

2. **Routing approach**
   - What we know: svelte-routing works for SPA, but simple apps may not need it
   - What's unclear: How many "pages" the wizard flow needs
   - Recommendation: Defer decision to planning phase, may use conditional rendering instead

3. **Development workflow**
   - What we know: Two processes (npm run dev + dotnet watch) or integrated with SpaServices
   - What's unclear: User preference on workflow complexity
   - Recommendation: Start with two processes for simplicity, integrate later if needed

</open_questions>

<sources>
## Sources

### Primary (HIGH confidence)
- [Svelte Getting Started](https://svelte.dev/docs/svelte/getting-started) - Project creation, SvelteKit vs plain Svelte
- [Tailwind CSS Vite Installation](https://tailwindcss.com/docs/installation/using-vite) - v4 setup with @tailwindcss/vite
- [Svelte Transitions](https://svelte.dev/docs/svelte/transition) - Built-in fade, fly, scale, slide
- [Svelte 5 Migration Guide](https://svelte.dev/docs/svelte/v5-migration-guide) - Runes, $state, $derived
- [shadcn-svelte Vite Installation](https://www.shadcn-svelte.com/docs/installation/vite) - Works with plain Svelte

### Secondary (MEDIUM confidence)
- [SvelteKit Single-Page Apps](https://svelte.dev/docs/kit/single-page-apps) - SPA mode documentation
- [Vite Server Proxy](https://vite.dev/config/server-options.html#server-proxy) - Proxy configuration
- [.NET Minimal APIs Overview](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/overview) - Endpoint patterns

### Tertiary (LOW confidence - needs validation during implementation)
- GitHub discussions on .NET + Vite integration patterns
- Community patterns for monorepo structure

</sources>

<metadata>
## Metadata

**Research scope:**
- Core technology: Svelte 5 + Vite
- Ecosystem: Tailwind v4, shadcn-svelte, svelte-routing
- Patterns: Vite proxy, runes, transitions
- Pitfalls: SvelteKit overkill, Tailwind version confusion, CORS, Motion.dev

**Confidence breakdown:**
- Standard stack: HIGH - verified with official docs
- Architecture: HIGH - follows official recommendations
- Pitfalls: HIGH - documented in GitHub issues and community
- Code examples: HIGH - from official documentation

**Research date:** 2025-12-21
**Valid until:** 2026-01-21 (30 days - Svelte/Tailwind ecosystems recently stabilized)

</metadata>

---

*Phase: 05-web-foundation*
*Research completed: 2025-12-21*
*Ready for planning: yes*
