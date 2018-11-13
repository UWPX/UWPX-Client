$project="..\AES_GCM\AES_GCM.vcxproj"
$parasBuild="/p:configuration=debug /p:Platform=ARM"
$parasClean="/t:clean"
$msBuild="C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe"

# First clean the project:
#& $msBuild $project $parasClean

# Then Build the project:
& $msBuild $project /p:configuration=debug /p:Platform=ARM
& $msBuild $project /p:configuration=debug /p:Platform=x86
& $msBuild $project /p:configuration=debug /p:Platform=x64