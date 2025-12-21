# Phase 9: PDF Export - Context

**Gathered:** 2025-12-21
**Status:** Ready for planning

<vision>
## How This Should Work

One-click download. You finish previewing your cards, move to the Export step, see a summary of what you're getting, and click download. The PDF generates (with a progress indicator), then the browser downloads it automatically.

The export step is the final destination in the wizard flow — after card preview, you land here. It shows a summary view with stats (card count, genres, etc.) plus the download button. Simple and complete.

After download: stay on the page with a success confirmation, and offer a "start new batch" option to upload another CSV and begin again.

The filename should be smart — reflecting the content (date, song count, etc.) so you know what you downloaded.

</vision>

<essential>
## What Must Be Nailed

- **Speed AND print quality** — Fast generation (click and get it within seconds), but the PDF must look perfect when printed (exact 85x55mm sizing, clean QR codes)
- **One-click simplicity** — No configuration maze. One button, one PDF.
- **Genre colors flow through** — The color palette customizations from card preview apply to the PDF cards

</essential>

<boundaries>
## What's Out of Scope

- Multiple export formats — PDF only, no PNG or individual card images
- Print settings UI — No page size options, margins, paper orientation controls
- Card selection at export — All matched cards go in, no filtering at this step (curation was for preview only)
- Card size options — 85x55mm is fixed
- Cards per page options — Layout is fixed

</boundaries>

<specifics>
## Specific Ideas

- **Cutting lines toggle** — User can choose between:
  - Edge-only lines (for ruler-guided cutting)
  - Complete lines (surrounding each card)
- Toggle lives on the export page, easy to access near the download button
- Preference is remembered (localStorage) so they don't have to pick each time
- Summary view shows: card count, genres represented, maybe other relevant stats
- Success confirmation after download starts
- "Start new batch" option to begin again with fresh CSV

</specifics>

<notes>
## Additional Context

This is the final phase of v2.0 — the culmination of the web interface milestone. The PDF generation itself already works from v1.0 (QuestPDF), so this phase is about:
1. Wiring the existing PDF generation to the web backend
2. Creating the export step UI in the frontend
3. Adding the cutting lines option
4. Ensuring genre colors from preview carry through to the PDF

The user values practical print quality — the cutting lines option shows they're thinking about the actual physical process of making the cards.

</notes>

---

*Phase: 09-pdf-export*
*Context gathered: 2025-12-21*
