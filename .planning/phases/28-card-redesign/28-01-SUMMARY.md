# Summary 28-01: Backend Card Redesign

## Execution Results

**Status:** COMPLETE
**Duration:** ~10 min
**Date:** 2025-12-26

## Tasks Completed

### Task 1: Update CardData model with album properties
- Added `AlbumImageUrl` and `AlbumName` properties to CardData record
- File: `Models/CardData.cs`

### Task 2: Update CardPreviewRequest with album properties
- Added `AlbumImageUrl` and `AlbumName` properties to request model
- Updated both front and back endpoint handlers to pass album data to CardData
- File: `Endpoints/CardPreviewEndpoints.cs`

### Task 3: Update CardPreviewCache with album in cache key
- Updated `BackCardKey` to include album image URL hash for proper cache invalidation
- File: `Services/CardPreviewCache.cs`

### Task 4: Redesign CardDesigner front card layout
- Reduced QR code size from 45mm to 38mm to make room for genre text
- Added genre text centered below QR code
- Implemented contrast-aware text color (white on dark backgrounds, dark on light)
- File: `Services/CardDesigner.cs`

### Task 5: Create helper method for fetching album art
- Added `FetchAlbumImage` method with HTTP client
- Implemented `ConcurrentDictionary` cache for session-level caching
- Graceful error handling (returns null on failure)
- File: `Services/CardDesigner.cs`

### Task 6: Add brightness/contrast helper method
- Added `IsDarkColor` method using luminance formula
- Formula: `0.299*R + 0.587*G + 0.114*B`
- Returns true if luminance < 128
- File: `Services/CardDesigner.cs`

### Task 7: Redesign CardDesigner back card layout
- Top bar: Year | Genre with semi-transparent black overlay (#00000080)
- Center: Album art (35mm x 35mm) fetched from URL
- Bottom bar: Artist - Title - Album with semi-transparent black overlay
- All text in white for readability on dark overlay
- File: `Services/CardDesigner.cs`

### Task 8: Update ExportCard and ExportRequest with album data
- Added `AlbumImageUrl` and `AlbumName` properties to ExportCard record
- File: `Models/ExportRequest.cs`

### Task 9: Update ExportEndpoints to pass album data
- Updated card generation loop to include album data from export request
- File: `Endpoints/ExportEndpoints.cs`

### Task 10: Update PdfExporter render methods
- Added same helper methods (`IsDarkColor`, `FetchAlbumImage`)
- Updated `RenderFrontCard` to match new CardDesigner layout
- Updated `RenderBackCard` to match new CardDesigner layout
- Ensures PDF export produces identical cards to preview
- File: `Services/PdfExporter.cs`

## New Card Layout

### Front Card
```
+----------------------------------+
|        [GENRE BACKGROUND]        |
|                                  |
|          +--------+              |
|          |   QR   |              |
|          |  CODE  |              |
|          +--------+              |
|                                  |
|           "Genre"                |
+----------------------------------+
```

### Back Card
```
+----------------------------------+
| [  Year | Genre  ]    <- top bar |
|                                  |
|        +------------+            |
|        |            |            |
|        | Album Art  |            |
|        |            |            |
|        +------------+            |
|                                  |
| [Artist - Title - Album] <- btm |
+----------------------------------+
```

## Verification

- [x] All 10 tasks completed
- [x] `dotnet build` succeeds with 0 errors, 0 warnings
- [x] Album properties added to all relevant models
- [x] CardDesigner implements new layouts
- [x] PdfExporter matches CardDesigner layouts
- [x] Album art fetching with caching implemented
- [x] Contrast-aware text colors implemented

## Files Modified

1. `Models/CardData.cs` - Added album properties
2. `Endpoints/CardPreviewEndpoints.cs` - Added album properties to request, pass to CardData
3. `Services/CardPreviewCache.cs` - Added album URL to cache key
4. `Services/CardDesigner.cs` - New layouts, album fetching, brightness helper
5. `Models/ExportRequest.cs` - Added album properties to ExportCard
6. `Endpoints/ExportEndpoints.cs` - Pass album data to CardData
7. `Services/PdfExporter.cs` - New layouts matching CardDesigner

## Next Steps

Execute Plan 28-02 for frontend integration and visual verification.
