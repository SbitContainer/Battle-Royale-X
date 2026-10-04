# Offline safety probes. No credentials, network, build, publication or device operations.
Set-StrictMode -Version Latest
$ErrorActionPreference='Stop'
$scriptPath=Join-Path $PSScriptRoot 'Publish-GitHubApk.ps1'
$tokens=$null; $errors=$null
$ast=[Management.Automation.Language.Parser]::ParseFile($scriptPath,[ref]$tokens,[ref]$errors)
if($errors.Count){ throw 'Publisher parse failed.' }
$temporary=Join-Path ([IO.Path]::GetTempPath()) ('brx-publisher-tests-'+[Guid]::NewGuid().ToString('N'))
[void](New-Item -ItemType Directory -Path $temporary)
$pass=0
try {
    $definitions=$ast.FindAll({param($node) $node -is [Management.Automation.Language.FunctionDefinitionAst]},$true)
    foreach($name in @('Fail','Sha','AssertHash','WriteNewJson')){
        $definition=@($definitions|Where-Object {$_.Name -eq $name})
        if($definition.Count -ne 1){throw "Missing function $name"}
        Invoke-Expression $definition[0].Extent.Text
    }
    $metadata=Join-Path $temporary 'metadata.json'
    WriteNewJson $metadata ([ordered]@{test=1})
    $originalHash=Sha $metadata
    WriteNewJson $metadata ([ordered]@{test=1})
    if((Sha $metadata)-ne $originalHash){throw 'Idempotent metadata changed.'}; $pass++
    $rejected=$false
    try{WriteNewJson $metadata ([ordered]@{test=2})}catch{$rejected=$true}
    if(!$rejected -or (Sha $metadata)-ne $originalHash){throw 'Metadata overwrite was allowed.'}; $pass++
    AssertHash $metadata $originalHash; $pass++
    $rejected=$false
    try{AssertHash $metadata ('0'*64)}catch{$rejected=$true}
    if(!$rejected){throw 'Wrong hash was allowed.'}; $pass++
    foreach($candidate in @(
        @{schemaVersion=1;status='BUILT_NOT_PUBLISHED';packageId='com.sbitcontainer.battleroyalex.prototype';versionCode=11;buildErrors=0},
        @{schemaVersion=1;status='BUILDING';packageId='com.sbitcontainer.battleroyalex.prototype';versionCode=12;buildErrors=0},
        @{schemaVersion=1;status='BUILT_NOT_PUBLISHED';packageId='wrong.package';versionCode=12;buildErrors=0},
        @{schemaVersion=1;status='BUILT_NOT_PUBLISHED';packageId='com.sbitcontainer.battleroyalex.prototype';versionCode=12;buildErrors=1}
    )){
        [IO.File]::WriteAllText((Join-Path $temporary 'manifest.json'),($candidate|ConvertTo-Json))
        $rejected=$false
        try{& $scriptPath -ReleaseDirectory $temporary -TestsLog $metadata}catch{$rejected=$true}
        if(!$rejected){throw 'Invalid candidate was not rejected.'}; $pass++
    }
    $source=[IO.File]::ReadAllText($scriptPath)
    if($source.IndexOf('if (!$Publish)') -ge $source.IndexOf('$script:token = GetCredential')){throw 'Credential lookup precedes explicit publish switch.'}; $pass++
    if($source -match "Api 'DELETE'|Write(AllText|AllBytes).*token|Write-(Output|Host).*token"){throw 'Forbidden credential output or destructive release call detected.'}; $pass++
    "PUBLISHER_OFFLINE_TESTS pass=$pass fail=0. Live publication NOT EXECUTED."
}finally{
    # Remove only the explicitly created test directory, validated beneath the system temp root.
    $resolved=[IO.Path]::GetFullPath($temporary)
    $tempRoot=[IO.Path]::GetFullPath([IO.Path]::GetTempPath())
    if(!$resolved.StartsWith($tempRoot,[StringComparison]::OrdinalIgnoreCase) -or [IO.Path]::GetFileName($resolved) -notlike 'brx-publisher-tests-*'){throw 'Unsafe test cleanup target.'}
    Remove-Item -LiteralPath $resolved -Recurse
}
