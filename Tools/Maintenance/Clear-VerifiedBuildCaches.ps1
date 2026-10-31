param(
    [ValidateSet('Plan','Execute','FunctionsOnly')][string]$Mode='Plan',
    [string]$PlanPath,
    [string]$PlanSha
)
$ErrorActionPreference='Stop'
$script:CncRepo='C:\dev\CNC-code-clean'
$script:BrxRepo='C:\Users\netor\OneDrive\Desktop\Battle Royale X'
$script:GradleTargets=@('C:\Users\netor\.gradle\caches\8.14.3\transforms','C:\Users\netor\.gradle\caches\9.1.0\transforms')

function Get-TextDigest([string]$Value) {
    $sha=[Security.Cryptography.SHA256]::Create()
    try { ([BitConverter]::ToString($sha.ComputeHash([Text.Encoding]::UTF8.GetBytes($Value)))).Replace('-','').ToLowerInvariant() }
    finally { $sha.Dispose() }
}
function Get-TrackedDigest([string]$Repo) {
    $diff=@(& git -C $Repo diff --binary HEAD --); if($LASTEXITCODE -ne 0){throw 'Cannot inspect tracked changes'}
    $index=@(& git -C $Repo ls-files --stage); if($LASTEXITCODE -ne 0){throw 'Cannot inspect index'}
    Get-TextDigest (($diff -join "`n")+"`nINDEX`n"+($index -join "`n"))
}
function Assert-AllowedRole([string]$Path,[string]$Kind,[string]$Version) {
    if($Kind -eq 'gradle-transform') {
        if($Path -cnotin $script:GradleTargets){throw 'Not an approved transform directory'}
    } elseif($Kind -eq 'historical-server-build') {
        if($Path -cnotmatch '^C:\\dev\\CNC-code-clean\\\.codex\\tmp\\release-[^\\]+\\(?:apps\\desktop-app\\dist-server|runtime-data\\windows-server\\(?:backend|pyinstaller-work|packages))$') {
            throw 'Not a generated historical build directory'
        }
        if(-not $Version -or [version]$Version -lt [version]'0.1.54' -or [version]$Version -ge [version]'0.1.61') {
            throw 'Historical version is not approved for removal'
        }
    } else { throw 'Unknown cleanup role' }
}
function Get-TargetState($Target,$Processes) {
    Assert-AllowedRole $Target.path $Target.kind $Target.version
    $resolved=(Resolve-Path -LiteralPath $Target.path -ErrorAction Stop).Path
    if($resolved -cne $Target.path){throw 'Resolved target changed'}
    $cursor=Get-Item -LiteralPath $resolved -Force
    if(-not $cursor.PSIsContainer){throw 'Expected a directory'}
    while($null -ne $cursor) {
        if(($cursor.Attributes -band [IO.FileAttributes]::ReparsePoint) -ne 0){throw 'Reparse point in ancestry'}
        $cursor=$cursor.Parent
    }
    foreach($process in $Processes) {
        if(($process.ExecutablePath -and ($process.ExecutablePath -eq $resolved -or $process.ExecutablePath.StartsWith($resolved+'\',[StringComparison]::OrdinalIgnoreCase))) -or
           ($process.CommandLine -and $process.CommandLine.IndexOf($resolved,[StringComparison]::OrdinalIgnoreCase) -ge 0)) { throw 'Target is in use' }
    }
    if($Target.kind -eq 'gradle-transform' -and @($Processes|Where-Object Name -in @('java.exe','javaw.exe','gradle.exe')).Count){throw 'Java/Gradle is running'}
    if($Target.kind -eq 'historical-server-build') {
        $snapshot=[regex]::Match($resolved,'^(.*)\\(?:apps\\desktop-app\\dist-server|runtime-data\\windows-server\\(?:backend|pyinstaller-work|packages))$').Groups[1].Value
        $version=(Get-Content -LiteralPath ($snapshot+'\apps\desktop-app\electron-builder.server.json') -Raw|ConvertFrom-Json).extraMetadata.version
        if($version -cne $Target.version){throw 'Snapshot version changed'}
        $tracked=@(& git -C $script:CncRepo ls-files -- $resolved)
        if($LASTEXITCODE -ne 0 -or $tracked.Count){throw 'Git-tracked content or Git failure'}
    }
    $children=@(Get-ChildItem -LiteralPath $resolved -Recurse -Force -ErrorAction Stop)
    if(@($children|Where-Object {($_.Attributes -band [IO.FileAttributes]::ReparsePoint) -ne 0}).Count){throw 'Nested reparse point'}
    $files=@($children|Where-Object {-not $_.PSIsContainer})
    if(@($files|Where-Object {$_.Extension.ToLowerInvariant() -in @('.db','.sqlite','.sqlite3','.pem','.key','.pfx','.p12','.nc','.cnc','.tap','.industriom-bank') -or $_.Name -eq '.env'}).Count) {
        throw 'Potential source, industrial data or credential content; preserve target'
    }
    $metadata=@($files|Sort-Object FullName|ForEach-Object {$_.FullName+'|'+$_.Length+'|'+$_.LastWriteTimeUtc.Ticks})
    [pscustomobject]@{bytes=[long](($files|Measure-Object Length -Sum).Sum);files=$files.Count;fingerprint=Get-TextDigest ($metadata -join "`n")}
}
function Get-ProtectedFiles {
    $paths=@(
      ($script:CncRepo+'\.codex\tmp\preparation66-20261030-v2\apps\desktop-app\dist-server\INDUSTRIOM-Server-Setup-0.1.66.exe'),
      ($script:CncRepo+'\.codex\tmp\release-keyboard65-20261029\apps\desktop-app\dist-server\INDUSTRIOM-Server-Setup-0.1.65.exe'),
      ($script:CncRepo+'\.codex\tmp\release-keyboard64-20261029\apps\desktop-app\dist-server\INDUSTRIOM-Server-Setup-0.1.64.exe'),
      ($script:CncRepo+'\apps\desktop-app\electron-builder.server.json'),
      ($script:BrxRepo+'\Builds\Releases\17\BattleRoyaleX-17.apk'),
      ($script:BrxRepo+'\Packages\packages-lock.json'),
      'C:\Users\netor\AppData\Local\Programs\INDUSTRIOM Server\resources\app.asar'
    )
    foreach($path in $paths){
        if(-not (Test-Path -LiteralPath $path -PathType Leaf)){throw ('Required protected file missing: '+$path)}
        [pscustomobject]@{path=$path;sha256=(Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash}
    }
}
if($Mode -eq 'FunctionsOnly'){return}
if($Mode -eq 'Plan') {
    $processes=Get-CimInstance Win32_Process
    $targets=[Collections.Generic.List[object]]::new(); $skipped=[Collections.Generic.List[object]]::new()
    foreach($snapshot in Get-ChildItem -LiteralPath ($script:CncRepo+'\.codex\tmp') -Directory) {
        if($snapshot.Name -notmatch '^release-'){continue}
        $config=$snapshot.FullName+'\apps\desktop-app\electron-builder.server.json'
        if(-not (Test-Path -LiteralPath $config)){continue}
        $version=(Get-Content -LiteralPath $config -Raw|ConvertFrom-Json).extraMetadata.version
        if(-not $version -or [version]$version -lt [version]'0.1.54' -or [version]$version -ge [version]'0.1.61'){continue}
        foreach($relative in @('apps\desktop-app\dist-server','runtime-data\windows-server\backend','runtime-data\windows-server\pyinstaller-work','runtime-data\windows-server\packages')) {
            $path=$snapshot.FullName+'\'+$relative
            if(-not (Test-Path -LiteralPath $path)){continue}
            $target=[ordered]@{path=$path;kind='historical-server-build';version=$version}
            try { $state=Get-TargetState $target $processes; if($state.bytes -eq 0){continue}; $target.bytes=$state.bytes; $target.files=$state.files; $target.fingerprint=$state.fingerprint; $targets.Add([pscustomobject]$target) }
            catch { $skipped.Add([pscustomobject]@{path=$path;reason=$_.Exception.Message}) }
        }
    }
    foreach($path in $script:GradleTargets) {
        if(-not (Test-Path -LiteralPath $path)){continue}
        $target=[ordered]@{path=$path;kind='gradle-transform';version=''}
        try { $state=Get-TargetState $target $processes; $target.bytes=$state.bytes; $target.files=$state.files; $target.fingerprint=$state.fingerprint; $targets.Add([pscustomobject]$target) }
        catch { $skipped.Add([pscustomobject]@{path=$path;reason=$_.Exception.Message}) }
    }
    [ordered]@{schemaVersion=1;scope='User-authorized old generated Server54-60 builds and two Gradle transform caches only';
      freeBytesBefore=(Get-PSDrive C).Free;cncTrackedDigest=Get-TrackedDigest $script:CncRepo;
      targets=$targets.ToArray();skipped=$skipped.ToArray();protected=@(Get-ProtectedFiles)} | ConvertTo-Json -Depth 7
    exit 0
}
if(-not $PlanPath -or -not $PlanSha){throw 'Execution requires frozen plan and its SHA256'}
if((Get-FileHash -LiteralPath $PlanPath -Algorithm SHA256).Hash -ine $PlanSha){throw 'Plan hash mismatch'}
$plan=Get-Content -LiteralPath $PlanPath -Raw|ConvertFrom-Json
if($plan.schemaVersion -ne 1 -or -not $plan.targets.Count){throw 'Empty or invalid plan'}
if((Get-TrackedDigest $script:CncRepo) -cne $plan.cncTrackedDigest){throw 'Tracked CNC source/index changed; no deletion'}
if(@($plan.targets.path|Sort-Object -Unique).Count -ne $plan.targets.Count){throw 'Duplicate targets'}
$processes=Get-CimInstance Win32_Process
foreach($protected in $plan.protected){if((Get-FileHash -LiteralPath $protected.path -Algorithm SHA256).Hash -cne $protected.sha256){throw 'Protected file changed'}}
# Validate every resolved target before deleting any directory. No shell handoff or globs.
foreach($target in $plan.targets){
    if(@($plan.targets|Where-Object {$_.path -ne $target.path -and $target.path.StartsWith($_.path+'\',[StringComparison]::OrdinalIgnoreCase)}).Count){throw 'Overlapping targets'}
    $state=Get-TargetState $target $processes
    if($state.fingerprint -cne $target.fingerprint -or $state.bytes -ne $target.bytes -or $state.files -ne $target.files){throw 'Target contents changed; no deletion'}
    foreach($protected in $plan.protected){if($protected.path.StartsWith($target.path+'\',[StringComparison]::OrdinalIgnoreCase)){throw 'Protected overlap'}}
}
$freeBefore=(Get-PSDrive C).Free; $removed=[Collections.Generic.List[object]]::new(); $errors=[Collections.Generic.List[object]]::new()
foreach($target in $plan.targets){
    try {
        # Recheck the frozen directory immediately before its irreversible removal.
        $state=Get-TargetState $target (Get-CimInstance Win32_Process)
        if($state.fingerprint -cne $target.fingerprint){throw 'Changed target'}
        Remove-Item -LiteralPath $target.path -Recurse -Force -ErrorAction Stop
        if(Test-Path -LiteralPath $target.path){throw 'Target still exists'}
        $removed.Add($target)
    } catch { $errors.Add([pscustomobject]@{path=$target.path;reason=$_.Exception.Message}) }
}
$protectedOk=$true
foreach($protected in $plan.protected){if((Get-FileHash -LiteralPath $protected.path -Algorithm SHA256).Hash -cne $protected.sha256){$protectedOk=$false}}
$trackedOk=(Get-TrackedDigest $script:CncRepo) -ceq $plan.cncTrackedDigest
$freeAfter=(Get-PSDrive C).Free
[ordered]@{schemaVersion=1;status=if($errors.Count -or -not $protectedOk -or -not $trackedOk){'PARTIAL_OR_REVIEW_REQUIRED'}else{'REMOVED_AND_VERIFIED'};
 planSha256=$PlanSha;removed=$removed.ToArray();errors=$errors.ToArray();protectedVerified=$protectedOk;cncTrackedSourceUnchanged=$trackedOk;
 freeBefore=$freeBefore;freeAfter=$freeAfter;freeDeltaBytes=$freeAfter-$freeBefore;permanentDeletion=$true;cloudDeletion=$false;
 note='Historical generated binaries require rebuild/download if needed later; sources and current/rollback artifacts preserved. Gradle transforms regenerate.'} | ConvertTo-Json -Depth 8
