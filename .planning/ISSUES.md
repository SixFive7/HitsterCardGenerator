# Issues and Enhancements

This file tracks deviations, bug fixes, and enhancements made during plan execution.

## Phase 02-csv-import / Task 02-01

### Bug Fix: Genre validation not running when other errors exist

**Issue:** CSV parser was only validating genre if there were no other validation errors on the song. This meant that a song with both an invalid year AND an invalid genre would only report the year error.

**Fix:** Modified `CsvParser.cs` to always validate genre, regardless of whether other validation errors exist. This ensures all validation errors are captured and reported to the user.

**Impact:** Improved error reporting - users now see all validation issues for each song, not just the first one encountered.

**Lines changed:**
- `Services/CsvParser.cs` line 65-93: Removed the `song.ValidationErrors.Count == 0` condition from genre validation logic, moved genre normalization to only occur when there are no other errors.
