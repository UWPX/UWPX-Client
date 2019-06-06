$solPath = $args[0]

if (Test-Path "env:RunningCI") {
    Write-Output "Skipping font import."
    exit 0
}

Write-Output "Copying font 'Segoe UI Emoji' (seguiemj.ttf)..."
cp -force C:\Windows\Fonts\seguiemj.ttf $solPath\Assets\Fonts\seguiemj.ttf
Write-Output "Font copied"

Write-Output "Copying font 'Segoe MDL2 Assets' (segmdl2.ttf)..."
cp -force C:\Windows\Fonts\segmdl2.ttf $solPath\Assets\Fonts\segmdl2.ttf
Write-Output "Font copied"