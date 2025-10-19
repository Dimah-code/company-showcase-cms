<#
Consolidate duplicate files according to a mapping CSV (created by report script).
By default this script will MOVE duplicate files to an archive folder (safe).
To perform actual deletion, use -Delete -Execute (risky).
Parameters:
 -MappingFile  : mapping CSV produced by report script (default asset_duplicates_mapping.csv)
 -ArchiveFolder: where to move duplicates (default Assets_archive)
 -Execute      : if present, perform move/delete. Otherwise dry-run.
 -Delete       : if present with -Execute, REMOVE files instead of archiving.
#>

param(
    [string]$MappingFile = "asset_duplicates_mapping.csv",
    [string]$ArchiveFolder = "Assets_archive",
    [switch]$Execute,
    [switch]$Delete
)

function Write-Log($m){ Write-Host $m }

$cwd = (Get-Location).ProviderPath
$mappingPath = Join-Path $cwd $MappingFile

if (-not (Test-Path $mappingPath)) {
    Write-Error "Mapping file not found: $mappingPath`nRun report-assets-duplicates.ps1 first."
    exit 1
}

$mappings = Import-Csv -Path $mappingPath

if ($mappings.Count -eq 0) {
    Write-Log "Mapping file is empty. Nothing to do."
    exit 0
}

Write-Log "Mappings loaded: $($mappings.Count) entries."
Write-Log ""

# show planned actions
$plan = @()
foreach ($row in $mappings) {
    $from = $row.From
    $keep = $row.Keep
    $action = $Delete.IsPresent ? "DELETE" : "ARCHIVE"
    $archiveTarget = Join-Path (Join-Path $cwd $ArchiveFolder) (Split-Path -Path $from -NoQualifier)
    $plan += [PSCustomObject]@{
        From = $from
        Keep = $keep
        Action = $action
        ArchiveTarget = $archiveTarget
    }
}

Write-Log "Planned actions (first 50 shown):"
$plan | Select-Object -First 50 | Format-Table -AutoSize

if (-not $Execute.IsPresent) {
    Write-Log ""
    Write-Log "DRY-RUN: no files will be moved or deleted. To apply changes, re-run with -Execute."
    exit 0
}

# Execute actions
# ensure archive root exists if archiving
if (-not $Delete.IsPresent) {
    $archiveRoot = Join-Path $cwd $ArchiveFolder
    if (-not (Test-Path $archiveRoot)) {
        New-Item -ItemType Directory -Path $archiveRoot | Out-Null
    }
}

$errors = @()
foreach ($p in $plan) {
    $from = $p.From
    if (-not (Test-Path $from)) {
        Write-Warning "File not found (skipping): $from"
        continue
    }

    if ($Delete.IsPresent) {
        try {
            Remove-Item -Path $from -Force
            Write-Log "Deleted: $from"
        } catch {
            Write-Warning "Failed to delete: $from - $_"
            $errors += $from
        }
    } else {
        # compute target preserving relative path structure
        $relative = $from.Substring($cwd.Length).TrimStart('\','/')
        $target = Join-Path (Join-Path $cwd $ArchiveFolder) $relative
        $targetDir = Split-Path -Path $target -Parent
        if (-not (Test-Path $targetDir)) { New-Item -ItemType Directory -Path $targetDir -Force | Out-Null }
        try {
            Move-Item -Path $from -Destination $target -Force
            Write-Log "Moved: $from -> $target"
        } catch {
            Write-Warning "Failed to move: $from - $_"
            $errors += $from
        }
    }
}

Write-Log ""
if ($errors.Count -gt 0) {
    Write-Warning "Completed with errors. See warnings above."
} else {
    Write-Log "All operations completed successfully."
}
