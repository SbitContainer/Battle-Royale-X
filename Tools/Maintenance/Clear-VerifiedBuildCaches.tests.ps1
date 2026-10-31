$ErrorActionPreference='Stop'
. "$PSScriptRoot/Clear-VerifiedBuildCaches.ps1" -Mode FunctionsOnly
$passed=0
foreach($role in @('apps\desktop-app\dist-server','runtime-data\windows-server\backend','runtime-data\windows-server\pyinstaller-work','runtime-data\windows-server\packages')) {
    Assert-AllowedRole ('C:\dev\CNC-code-clean\.codex\tmp\release-test\'+$role) 'historical-server-build' '0.1.54'; $passed++
}
foreach($version in @('0.1.53','0.1.61','0.1.65','0.1.66','')) {
    $rejected=$false
    try { Assert-AllowedRole 'C:\dev\CNC-code-clean\.codex\tmp\release-test\apps\desktop-app\dist-server' 'historical-server-build' $version }
    catch { $rejected=$true }
    if(-not $rejected){throw ('Protected version accepted: '+$version)}; $passed++
}
foreach($path in @('C:\','C:\Users\netor','C:\dev\CNC-code-clean',
    'C:\dev\CNC-code-clean\runtime-data\windows-server\backend',
    'C:\dev\CNC-code-clean\.codex\tmp\release-test',
    'C:\dev\CNC-code-clean\.codex\tmp\release-test\apps\desktop-app\dist-server\..',
    'C:\Users\netor\OneDrive\Desktop\Battle Royale X\Assets')) {
    $rejected=$false
    try { Assert-AllowedRole $path 'historical-server-build' '0.1.54' } catch { $rejected=$true }
    if(-not $rejected){throw ('Unsafe path accepted: '+$path)}; $passed++
}
foreach($path in @('C:\Users\netor\.gradle','C:\Users\netor\.gradle\caches',
    'C:\Users\netor\.gradle\caches\modules-2','C:\Users\netor\.gradle\caches\9.1.0\transforms\..')) {
    $rejected=$false
    try { Assert-AllowedRole $path 'gradle-transform' '' } catch { $rejected=$true }
    if(-not $rejected){throw ('Unsafe cache path accepted: '+$path)}; $passed++
}
Assert-AllowedRole 'C:\Users\netor\.gradle\caches\8.14.3\transforms' 'gradle-transform' ''; $passed++
Assert-AllowedRole 'C:\Users\netor\.gradle\caches\9.1.0\transforms' 'gradle-transform' ''; $passed++
if(Test-VerifiedPublicExpoCert 'C:\Users\netor\.gradle\caches\8.14.3\transforms\other\expo-root.pem' 'gradle-transform'){throw 'Wrong certificate path accepted'}; $passed++
if(Test-VerifiedPublicExpoCert $script:ExpoCacheCert 'historical-server-build'){throw 'Certificate exception allowed outside Gradle'}; $passed++
if(Test-Path -LiteralPath $script:ExpoCacheCert) {
    if(-not (Test-VerifiedPublicExpoCert $script:ExpoCacheCert 'gradle-transform')){throw 'Expected public certificate did not match dependency source'}
    $passed++
}
Write-Output ('CLEANUP_PATH_TESTS pass='+$passed+' fail=0; no deletion executed')
