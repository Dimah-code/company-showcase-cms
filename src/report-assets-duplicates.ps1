<#
Report duplicate files (by content hash) between two folders.
Outputs:
 - CSV report with groups: ReportFile (default: asset_duplicates_report.csv)
 - CSV mapping of duplicates -> keep candidate: MappingFile (default: asset_duplicates_mapping.csv)
Parameters:
 -FolderA  : first folder to scan (default "Assets")
 -FolderB  : second folder to scan (default "Admin_assets")
 -HashAlgorithm : SHA256|SHA1|MD5 (default SHA256)
#>

param(
    [string]$FolderA = "Assets",
    [string]$FolderB = "Admin_assets",
    [string]$ReportFile = "asset_duplicates_report.csv",
    [string]$MappingFile = "asset_duplicates_mapping.csv",
    [ValidateSet("SHA256","SHA1","MD5")][string]$HashAlgorithm = "SHA256"
)

function Write-Log($msg) { Write-Host $msg }

# normalize and check
$cwd = (Get-Location).ProviderPath
$pathA = Join-Path $cwd $FolderA
$pathB = Join-Path $cwd $FolderB

if (-not (Test-Path $pathA)) {
    Write-Error "FolderA not found: $pathA"
    exit 1
}
if (-not (Test-Path $pathB)) {
    Write-Error "FolderB not found: $pathB"
    exit 1
}

Write-Log "Scanning folders:"
Write-Log " - $pathA"
Write-Log " - $pathB"
Write-Log "Using hash algorithm: $HashAlgorithm"
Write-Log ""

# collect files
$files = Get-ChildItem -Path $pathA,$pathB -Recurse -File -ErrorAction SilentlyContinue

if ($files.Count -eq 0) {
    Write-Error "No files found in the specified folders."
    exit 1
}

# compute hashes
Write-Log "Computing hashes for $($files.Count) files..."
$hashMap = @{}  # hash -> list of full paths
foreach ($f in $files) {
    try {
        $h = (Get-FileHash -Path $f.FullName -Algorithm $HashAlgorithm).Hash
    } catch {
        Write-Warning "Cannot hash file: $($f.FullName). Skipping."
        continue
    }
    if (-not $hashMap.ContainsKey($h)) { $hashMap[$h] = @() }
    $hashMap[$h] += $f.FullName
}

# analyze duplicates
$report = @()
$mapping = @()
$totalDuplicateFiles = 0
$duplicateGroups = 0

foreach ($h in $hashMap.Keys) {
    $list = $hashMap[$h]
    if ($list.Count -gt 1) {
        $duplicateGroups++
        $totalDuplicateFiles += ($list.Count)
        # choose keep candidate: prefer file under FolderA if exists, else first element
        $keep = $null
        $normalizedList = $list
        foreach ($p in $normalizedList) {
            if ($p.ToLower().Contains((Join-Path $cwd $FolderA).ToLower())) {
                $keep = $p; break
            }
        }
        if (-not $keep) { $keep = $list[0] }

        $report += [PSCustomObject]@{
            Hash = $h
            Count = $list.Count
            KeepCandidate = $keep
            Paths = ($list -join ";")
        }

        foreach ($p in $list) {
            if ($p -ne $keep) {
                $mapping += [PSCustomObject]@{ From = $p; Keep = $keep }
            }
        }
    }
}

# export CSVs
if ($report.Count -gt 0) {
    $report | Export-Csv -Path $ReportFile -NoTypeInformation -Encoding UTF8
    $mapping | Export-Csv -Path $MappingFile -NoTypeInformation -Encoding UTF8
    Write-Log ""
    Write-Log "Report saved to: $ReportFile"
    Write-Log "Mapping saved to: $MappingFile"
} else {
    Write-Log "No duplicate files found by content hash."
}

# summary
Write-Log ""
Write-Log "Summary:"
Write-Log " - Files scanned: $($files.Count)"
Write-Log " - Duplicate groups found: $duplicateGroups"
Write-Log " - Total files in duplicate groups: $totalDuplicateFiles"
Write-Log ""
Write-Log "Next steps:"
Write-Log " - Inspect the CSV files. If mapping looks good, run consolidate-assets script to archive/delete duplicates."
