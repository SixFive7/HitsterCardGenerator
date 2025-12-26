# Phase 26-01 Summary: Card Preview API Endpoints

## What Was Done

Added server-side card preview image generation using QuestPDF, enabling pixel-perfect card previews through API endpoints.

### Files Created

- **Endpoints/CardPreviewEndpoints.cs** - Two POST endpoints for generating card preview images:
  - `POST /api/card-preview/front` - Returns front card PNG with QR code
  - `POST /api/card-preview/back` - Returns back card PNG with year, artist, title, genre
  - Includes `CardPreviewRequest` record with all card properties (trackId, title, artist, year, genre, backgroundColor)

- **Services/CardPreviewCache.cs** - In-memory caching service:
  - Uses .NET IMemoryCache for efficient image caching
  - Cache key format: `card_{side}_{trackId}_{backgroundColor}`
  - 10-minute sliding expiration, 1-hour absolute expiration
  - Prevents redundant QuestPDF rendering for repeated requests

### Files Modified

- **Program.cs** - Added service registrations and endpoint mapping:
  - `builder.Services.AddMemoryCache()`
  - `builder.Services.AddSingleton<CardPreviewCache>()`
  - `app.MapCardPreviewEndpoints()`

## Verification Results

- Build succeeds with no errors or warnings
- Front card endpoint returns valid PNG (17KB, valid PNG signature)
- Back card endpoint returns valid PNG (21KB, valid PNG signature)
- Endpoints registered in OpenAPI at `/openapi/v1.json`
- Caching verified: second request 5x faster than first (8ms vs 42ms)

## Technical Notes

- Uses existing `CardDesigner.GenerateFrontCardImage()` and `GenerateBackCardImage()` methods
- Uses existing `QrCodeService.GenerateQrCode()` for QR code generation
- Images generated at QuestPDF default 288 DPI
- Cache key includes backgroundColor to invalidate on color changes

## Next Steps

Phase 26-02 will integrate these endpoints with the Svelte frontend to replace the current HTML/CSS card rendering.
