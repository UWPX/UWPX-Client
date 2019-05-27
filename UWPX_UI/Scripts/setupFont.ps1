$solPath = $args[0]

Write-Output "Copying font 'Segoe UI Emoji' (seguiemj.ttf)..."
cp -force C:\Windows\Fonts\seguiemj.ttf $solPath\Assets\Fonts\seguiemj.ttf
Write-Output "Font copied"