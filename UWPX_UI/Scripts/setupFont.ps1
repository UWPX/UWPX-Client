$solPath = $args[0]

if (Test-Path "env:windir") {

	$segoeUiPath = "$env:windir\Fonts\seguiemj.ttf"
    if(Test-Path $segoeUiPath) {
		Write-Output "Copying font 'Segoe UI Emoji' (seguiemj.ttf)..."
		cp -force $segoeUiPath $solPath\Assets\Fonts\seguiemj.ttf
		Write-Output "'Segoe UI Emoji' copied."
    }
    else {
	    Write-Warning "'Segoe UI Emoji' not found: $segoeUiPath"
	}
    

    $segoeMDL2AssestsPath = "$env:windir\Fonts\seguiemj.ttf"
	if(Test-Path $segoeMDL2AssestsPath) {
		Write-Output "Copying font 'Segoe MDL2 Assets' (segmdl2.ttf)..."
        cp -force $segoeMDL2AssestsPath $solPath\Assets\Fonts\segmdl2.ttf
        Write-Output "'Segoe MDL2 Assets' copied."
	}
	else {
	    Write-Warning "'Segoe UI Emoji' not found: $segoeUiPath"
	}
    exit 0
}
Write-Output "Skipping font import. No 'windir'"
