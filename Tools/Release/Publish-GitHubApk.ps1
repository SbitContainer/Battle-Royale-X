param(
    [Parameter(Mandatory=$true)][string]$ReleaseDirectory,
    [Parameter(Mandatory=$true)][string]$TestsLog,
    [string]$AndroidBuildTools = 'C:\Users\netor\AppData\Local\Android\Sdk\build-tools\36.0.0',
    [string]$JavaHome = 'C:\Program Files\Unity\Hub\Editor\6000.6.1f1\Editor\Data\PlaybackEngines\AndroidPlayer\OpenJDK',
    [switch]$Publish
)
# This tool never builds/installs an APK, changes repo visibility, or replaces an existing release.
Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
$ProgressPreference = 'SilentlyContinue'
$package = 'com.sbitcontainer.battleroyalex.prototype'
$certificate = '9cb3ed07529afefbd8f304a78fe349484ccf445357d7f777ca530a493598dc17'
$repository = 'SbitContainer/Battle-Royale-X'
$root = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
function Fail([string]$message) { throw $message }
function Sha([string]$path) { (Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash.ToLowerInvariant() }
function AssertHash([string]$path, [string]$expected) {
    if ($expected -notmatch '^[a-fA-F0-9]{64}$' -or !(Test-Path -LiteralPath $path -PathType Leaf) -or (Sha $path) -ne $expected.ToLowerInvariant()) { Fail 'Candidate evidence hash mismatch or missing file.' }
}
function WriteNewJson([string]$path, $value) {
    $json = $value | ConvertTo-Json -Depth 12
    if (Test-Path -LiteralPath $path) {
        if ([IO.File]::ReadAllText($path).Trim() -ne $json.Trim()) { Fail 'Existing local metadata differs; no file was overwritten.' }
        return
    }
    $stream = [IO.File]::Open($path, [IO.FileMode]::CreateNew)
    try { $writer = New-Object IO.StreamWriter($stream, (New-Object Text.UTF8Encoding($false))); $writer.Write($json); $writer.Dispose() } finally { $stream.Dispose() }
}
function GetCredential {
    if (![string]::IsNullOrWhiteSpace($env:GH_TOKEN)) { return $env:GH_TOKEN }
    $start = New-Object Diagnostics.ProcessStartInfo
    $start.FileName = 'git'; $start.Arguments = 'credential fill'; $start.WorkingDirectory = $root
    $start.UseShellExecute = $false; $start.CreateNoWindow = $true
    $start.RedirectStandardInput = $true; $start.RedirectStandardOutput = $true; $start.RedirectStandardError = $true
    $start.EnvironmentVariables['GCM_INTERACTIVE'] = 'Never'; $start.EnvironmentVariables['GIT_TERMINAL_PROMPT'] = '0'
    $process = New-Object Diagnostics.Process
    $process.StartInfo = $start
    try {
        [void]$process.Start()
        $stdout = $process.StandardOutput.ReadToEndAsync(); $stderr = $process.StandardError.ReadToEndAsync()
        $process.StandardInput.Write("protocol=https`nhost=github.com`n`n"); $process.StandardInput.Close()
        if (!$process.WaitForExit(15000)) { $process.Kill(); Fail 'Noninteractive credential lookup timed out.' }
        $captured = $stdout.GetAwaiter().GetResult(); [void]$stderr.GetAwaiter().GetResult()
        if ($process.ExitCode -ne 0) { Fail 'GitHub credential unavailable. Configure login separately or provide GH_TOKEN; no login was opened.' }
        $match = [regex]::Match($captured, '(?m)^password=(.+)\r?$')
        if (!$match.Success) { Fail 'GitHub credential unavailable.' }
        return $match.Groups[1].Value.Trim()
    } finally { $process.Dispose() }
}
function Api([string]$method, [string]$url, $body=$null, [string]$file=$null, [string]$download=$null, [switch]$Allow404, [switch]$Anonymous) {
    $request = $null; $response = $null; $stream = $null
    try {
        $uri = [Uri]$url
        if ($uri.Scheme -ne 'https' -or $uri.Host -notin @('api.github.com','uploads.github.com','github.com')) { Fail 'Unapproved release endpoint.' }
        $request = New-Object Net.Http.HttpRequestMessage((New-Object Net.Http.HttpMethod($method)), $uri)
        [void]$request.Headers.TryAddWithoutValidation('User-Agent', 'BattleRoyaleX-Release')
        [void]$request.Headers.TryAddWithoutValidation('X-GitHub-Api-Version', '2022-11-28')
        [void]$request.Headers.TryAddWithoutValidation('Accept', $(if($download){'application/octet-stream'}else{'application/vnd.github+json'}))
        if (!$Anonymous) { $request.Headers.Authorization = New-Object Net.Http.Headers.AuthenticationHeaderValue('Bearer', $script:token) }
        if ($file) {
            $stream = [IO.File]::OpenRead($file)
            $request.Content = New-Object Net.Http.StreamContent($stream)
            $request.Content.Headers.ContentType = New-Object Net.Http.Headers.MediaTypeHeaderValue('application/octet-stream')
        } elseif ($null -ne $body) {
            $request.Content = New-Object Net.Http.StringContent(($body | ConvertTo-Json -Depth 12), [Text.Encoding]::UTF8, 'application/json')
        }
        $response = $script:client.SendAsync($request, [Net.Http.HttpCompletionOption]::ResponseHeadersRead).GetAwaiter().GetResult()
        if ($Allow404 -and [int]$response.StatusCode -eq 404) { return $null }
        if (!$response.IsSuccessStatusCode) { Fail ('GitHub request failed (HTTP ' + [int]$response.StatusCode + '); response and credentials withheld.') }
        if ($download) {
            $inputStream = $response.Content.ReadAsStreamAsync().GetAwaiter().GetResult()
            $output = [IO.File]::Open($download, [IO.FileMode]::CreateNew)
            try { $inputStream.CopyTo($output) } finally { $output.Dispose(); $inputStream.Dispose() }
            return
        }
        return ($response.Content.ReadAsStringAsync().GetAwaiter().GetResult() | ConvertFrom-Json)
    } catch {
        # Do not expose arbitrary HTTP exception bodies/headers or captured credentials.
        if ($_.Exception.Message -match '^(GitHub request failed|Unapproved release endpoint)') { throw $_.Exception.Message }
        Fail 'Release network operation failed; credentials and response body withheld. Check draft state before retrying.'
    } finally { if($response){$response.Dispose()}; if($request){$request.Dispose()}; if($stream){$stream.Dispose()} }
}

$directory = (Resolve-Path -LiteralPath $ReleaseDirectory).Path
$manifestPath = Join-Path $directory 'manifest.json'
$manifest = [IO.File]::ReadAllText($manifestPath) | ConvertFrom-Json
if ($manifest.schemaVersion -ne 1 -or $manifest.status -ne 'BUILT_NOT_PUBLISHED' -or $manifest.packageId -ne $package -or $manifest.versionCode -le 11 -or $manifest.buildErrors -ne 0) { Fail 'Manifest is not an approved publishable Android candidate.' }
$code = [int]$manifest.versionCode
$apkName = "BattleRoyaleX-$code.apk"
$apk = Join-Path $directory $apkName
$expectedRelative = "Builds/Releases/$code/$apkName"
if ($manifest.file -ne $expectedRelative -or [IO.Path]::GetFullPath($apk) -ne [IO.Path]::GetFullPath((Join-Path $root $expectedRelative))) { Fail 'Candidate path does not match the reserved release directory.' }
AssertHash $apk $manifest.sha256
if ((Get-Item -LiteralPath $apk).Length -ne $manifest.sizeBytes) { Fail 'APK byte length differs from manifest.' }
$log = (Resolve-Path -LiteralPath $TestsLog).Path
AssertHash $log $manifest.testsLogSha256
$text = [IO.File]::ReadAllText($log)
$finals = [regex]::Matches($text, '\[BRX LIVE FINAL\] pass=(\d+) fail=(\d+)')
$visuals = [regex]::Matches($text, '\[BRX VISUAL SUMMARY\] pass=(\d+) fail=(\d+)')
if (!$finals.Count -or [int]$finals[$finals.Count-1].Groups[1].Value -lt 1 -or [int]$finals[$finals.Count-1].Groups[2].Value -ne 0 -or !$visuals.Count -or [int]$visuals[$visuals.Count-1].Groups[1].Value -lt 48 -or [int]$visuals[$visuals.Count-1].Groups[2].Value -ne 0) { Fail 'Completed combat and visual matrix gates did not pass.' }
$sourcePath = Join-Path $directory 'source-inputs.json'
AssertHash $sourcePath $manifest.sourceInputsSha256
$source = [IO.File]::ReadAllText($sourcePath) | ConvertFrom-Json
if (@($source.PSObject.Properties.Name).Count -ne 1 -or $source.PSObject.Properties.Name -notcontains 'files' -or @($source.files).Count -lt 1) { Fail 'Source inventory schema is invalid.' }
$seen = @{}
foreach($entry in $source.files) {
    if (@($entry.PSObject.Properties.Name).Count -ne 2 -or $entry.path -notmatch '^(Assets|Packages|ProjectSettings)/[^:]+$' -or $entry.path -match '(^|/)\.\.(/|$)|[\r\n\\]' -or $entry.sha256 -notmatch '^[a-fA-F0-9]{64}$' -or $seen.ContainsKey($entry.path)) { Fail 'Source inventory contains unsafe/duplicate paths or unexpected content.' }
    $seen[$entry.path] = $true
}
$aapt = Join-Path $AndroidBuildTools 'aapt.exe'
$signer = Join-Path $AndroidBuildTools 'apksigner.bat'
if (!(Test-Path -LiteralPath $aapt) -or !(Test-Path -LiteralPath $signer)) { Fail 'aapt/apksigner are required for publication.' }
$badging = (& $aapt dump badging $apk 2>&1 | Out-String)
if ($LASTEXITCODE -ne 0) { Fail 'aapt inspection failed.' }
$identity = [regex]::Match($badging, "package: name='([^']+)' versionCode='([^']+)' versionName='([^']+)'")
if (!$identity.Success -or $identity.Groups[1].Value -ne $package -or [int]$identity.Groups[2].Value -ne $code -or $identity.Groups[3].Value -ne $manifest.versionName) { Fail 'APK internal identity differs from manifest.' }
$previousJava = $env:JAVA_HOME
try { $env:JAVA_HOME = $JavaHome; $verification = (& $signer verify --verbose --print-certs $apk 2>&1 | Out-String); $signerExit = $LASTEXITCODE } finally { $env:JAVA_HOME = $previousJava }
if ($signerExit -ne 0 -or $verification -notmatch 'Number of signers: 1' -or $verification -notmatch ("Signer #1 certificate SHA-256 digest: " + $certificate)) { Fail 'APK signature is invalid or differs from the existing prototype certificate.' }
$tag = "android-lab-$code"
$base = "https://github.com/$repository/releases/download/$tag"
$catalog = [ordered]@{schemaVersion=1; packageName=$package; versionCode=$code; versionName=$manifest.versionName; apkUrl="$base/$apkName"; apkBytes=[long]$manifest.sizeBytes; apkSha256=$manifest.sha256.ToLowerInvariant()}
$catalogPath = Join-Path $directory 'brx-update.json'
WriteNewJson $catalogPath $catalog
if (!$Publish) { Write-Output "VERIFIED_NOT_PUBLISHED code=$code catalog=$catalogPath. No credential lookup/upload/install occurred."; return }
if (Test-Path -LiteralPath (Join-Path $directory 'publication.json')) { Fail 'Publication receipt already exists; refusing duplicate publication.' }
$candidateHashes = @{}
foreach($path in @($apk,$catalogPath,$sourcePath)){ $candidateHashes[[IO.Path]::GetFileName($path)] = Sha $path }
if($candidateHashes[$apkName] -ne $manifest.sha256 -or $candidateHashes['source-inputs.json'] -ne $manifest.sourceInputsSha256){ Fail 'Candidate changed during inspection; publication aborted.' }
Add-Type -AssemblyName System.Net.Http
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
$script:token = GetCredential
$script:client = New-Object Net.Http.HttpClient
$script:client.Timeout = [TimeSpan]::FromMinutes(5)
$api = "https://api.github.com/repos/$repository"
$draft = $null
try {
    $repo = Api 'GET' $api
    if ($repo.full_name -cne $repository -or $repo.private) { Fail 'Expected public repository identity differs; no privacy changes are allowed.' }
    if (Api 'GET' "$api/git/ref/tags/$tag" -Allow404) { Fail 'Remote tag already exists; no replacement is allowed.' }
    for($page=1; $page -le 100; $page++) {
        $releases = @(Api 'GET' "$api/releases?per_page=100&page=$page")
        if (@($releases | Where-Object {$_.tag_name -eq $tag}).Count) { Fail 'Release/draft already exists; no replacement is allowed.' }
        foreach($existing in $releases){
            $existingCode=[regex]::Match($existing.tag_name,'^android-lab-(\d+)$')
            if($existingCode.Success -and [long]$existingCode.Groups[1].Value -ge $code){ Fail 'Channel already reserves this or a newer version; choose a new build code.' }
        }
        if ($releases.Count -lt 100) { break }
        if ($page -eq 100) { Fail 'Release pagination limit reached; cannot prove tag uniqueness.' }
    }
    $notes = "Android test APK. sourceDirty=$($manifest.sourceDirty). HEAD $($manifest.sourceCommit) is provenance context, NOT the exact source commit of a dirty build. source-inputs.json hashes the actual build inputs before temporary version settings; it is not a recoverable source archive. Editor combat/UI and structural visual gates passed; Android touch/performance validation NOT EXECUTED. Debug-signed prototype, not a production release. APK SHA256: $($manifest.sha256)."
    $draft = Api 'POST' "$api/releases" @{tag_name=$tag;target_commitish=$repo.default_branch;name="Battle Royale X Android lab $code";body=$notes;draft=$true;prerelease=$false;make_latest='false';generate_release_notes=$false}
    $uploaded = @()
    foreach($path in @($apk,$catalogPath,$sourcePath)) {
        $name = [IO.Path]::GetFileName($path)
        $expectedSha = $candidateHashes[$name]
        AssertHash $path $expectedSha
        $asset = Api 'POST' ("https://uploads.github.com/repos/$repository/releases/$($draft.id)/assets?name=" + [Uri]::EscapeDataString($name)) -file $path
        AssertHash $path $expectedSha
        if ($asset.name -ne $name -or [long]$asset.size -ne (Get-Item -LiteralPath $path).Length -or $asset.state -ne 'uploaded') { Fail 'Uploaded asset metadata mismatch; draft retained.' }
        if ($asset.PSObject.Properties.Name -contains 'digest' -and $asset.digest -and $asset.digest -ne "sha256:$expectedSha") { Fail 'GitHub asset digest mismatch; draft retained.' }
        $temp = Join-Path $directory ('.verify-' + [Guid]::NewGuid().ToString('N'))
        try { Api 'GET' $asset.url -download $temp; AssertHash $temp $expectedSha } finally { if(Test-Path -LiteralPath $temp){ Remove-Item -LiteralPath $temp } }
        $uploaded += $asset
    }
    # Only this draft, created by this invocation, can transition to public after ALL uploads pass.
    $released = Api 'PATCH' "$api/releases/$($draft.id)" @{draft=$false;make_latest='true'}
    if ($released.draft -or $released.tag_name -ne $tag) { Fail 'GitHub did not confirm publication.' }
    foreach($asset in $uploaded) {
        $temp = Join-Path $directory ('.public-verify-' + [Guid]::NewGuid().ToString('N'))
        try { Api 'GET' "$base/$($asset.name)" -download $temp -Anonymous; AssertHash $temp $candidateHashes[$asset.name] } finally { if(Test-Path -LiteralPath $temp){ Remove-Item -LiteralPath $temp } }
    }
    WriteNewJson (Join-Path $directory 'publication.json') ([ordered]@{schemaVersion=1;status='PUBLISHED_VERIFIED';publishedAtUtc=[DateTime]::UtcNow.ToString('o');releaseUrl=$released.html_url;apkUrl=$catalog.apkUrl;catalogUrl="$base/brx-update.json";apkSha256=$catalog.apkSha256;versionCode=$code;certificateSha256=$certificate;deviceValidation='NOT_EXECUTED'})
    Write-Output "PUBLISHED_VERIFIED $($released.html_url) APK=$($catalog.apkUrl). No device installation occurred."
} catch {
    if($draft) { Write-Warning "Release ID $($draft.id) may exist as draft or published. No release/assets were deleted. Inspect state before retrying." }
    throw
} finally { $script:client.Dispose(); $script:token = $null }
