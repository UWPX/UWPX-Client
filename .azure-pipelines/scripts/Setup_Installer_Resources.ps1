param([String] $src, [String] $dst)

function GetAppxBundle {
    Get-ChildItem -File $args[0] -Filter "*.appxbundle" |
    ForEach-Object {
        return $_
    }
    return $null
}

function GetCert {
    Get-ChildItem -File $args[0] -Filter "*.cer" |
    ForEach-Object {
        return $_
    }
    return $null
}

function GetDependencies {
    Get-ChildItem -Directory $args[0] -Filter "Dependencies" |
    ForEach-Object {
        return $_
    }
    return $null
}

function GetPotentialAppxDirectoris {
    $dirs = @()
    Get-ChildItem -Directory "$src" |
    ForEach-Object {
        if ($_.BaseName -match "([a-zA-Z]+)_(\d+\.\d+\.\d+\.\d+)_([a-zA-Z]+)(_([a-zA-Z]+))?") {
            $dirs += $_
        }
    }
    return $dirs
}

function DeleteOldFiles {
    Write-Output "Removing old files..."
    if(Test-Path "Dependencies") {
        Write-Output "Removing Dependencies directory..."
        Remove-Item -Recurse "Dependencies"
    }
    $tmp = GetAppxBundle $dst
    if($tmp -and (Test-Path $tmp.FullName)) {
        Write-Output "Removing appxbundle..."
        Remove-Item $tmp.FullName
    }
    $tmp = GetCert $dst
    if($tmp -and (Test-Path $tmp.FullName)) {
        Write-Output "Removing cert..."
        Remove-Item $tmp.FullName
    }

    if(Test-Path "ReleaseInfo.json") {
        Write-Output "Removing ReleaseInfo.json..."
        Remove-Item "ReleaseInfo.json"
    }
}

function CopyNewFiles {
    Write-Output "Copying new files..."
    Copy-Item $args[0].FullName $dst
    Copy-Item $args[1].FullName $dst
    Copy-Item -Recurse $args[2].FullName $dst
}

function GetVersion {
    $args[0].BaseName | Select-String -Pattern "([a-zA-Z]+)_(\d+\.\d+\.\d+\.\d+)_([a-zA-Z]+)(_([a-zA-Z0-9]+))*" |
    ForEach-Object {
        return "v." + $_.Matches.Groups[2].value
    }
}

function GenReleaseInfo {
    Write-Output "Generating ReleaseInfo.json..."
    New-Item "ReleaseInfo.json"

    $appx = $args[0].BaseName + ".appxbundle"
    $cer = $args[1].BaseName + ".cer"
    $dep = $args[2].BaseName
    $version = GetVersion $args[0]
    $date = Get-Date -Format "dd.MM.yyyy"
    $changes = "https://github.com/UWPX/UWPX-Client/releases/tag/" + $version

    $content = '{
        "certPath": "' + $cer + '",
        "appxBundlePath": "' + $appx + '",
        "dependeciesPath": "' + $dep + '",
        "version": "' + $version + '",
        "releaseDate": "' + $date + '",
        "changelogUrl": "' + $changes + '",
        "appFamilyName": "790FabianSauter.UWPXAlpha"
      }'

    Set-Content "ReleaseInfo.json" $content
}

$appxBundle = $null
$cert = $null
$dependencies = $null

$dirs = GetPotentialAppxDirectoris
foreach ($item in $dirs) {
    $appxBundle = GetAppxBundle $item.FullName
    $cert = GetCert $item.FullName
    $dependencies = GetDependencies $item.FullName
    
    if($appxBundle -and $cert -and $dependencies) {
        break
    }
}
if (-not $appxBundle) {
    Write-Output ".appxbundle not found in: $src"
    exit 10
}

if (-not $cert) {
    Write-Output ".cer not found in: $src"
    exit 11
}

if (-not $dependencies) {
    Write-Output "Dependencies directory not found in: $src"
    exit 12
}

DeleteOldFiles
CopyNewFiles $appxBundle $cert $dependencies
GenReleaseInfo $appxBundle $cert $dependencies

Write-Output "Done"
