function Update-SourceVersion
{
    Param ([string]$Version)
    $NewVersion = 'AssemblyVersion("' + $Version + '")';
    $NewFileVersion = 'AssemblyFileVersion("' + $Version + '")';
    foreach ($o in $input){
        Write-Output $o.FullName
        $TmpFile = $o.FullName + ".tmp"

        Get-Content $o.FullName |
            %{$_ -replace 'AssemblyVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)', $NewVersion } |
            %{$_ -replace 'AssemblyFileVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)', $NewFileVersion } > $TmpFile

        Move-Item $TmpFile $o.FullName -force
    }
}

function Update-AllAssemblyInfoFiles ( $version )
{
    foreach ($file in "Version.cs" )
    {
        Get-ChildItem -Recurse |? {$_.Name -eq $file} | Update-SourceVersion $version ;
    }
}

function Build-All()
{
    $p = Resolve-Path ..\src\ProtoServiceGenerator.sln
    cmd.exe /c 'C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\msbuild.exe' $p /t:Clean
    cmd.exe /c 'C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\msbuild.exe' $p /t:restore
    cmd.exe /c 'C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\msbuild.exe' $p /t:rebuild /m /p:Configuration=Release /v:m
}

function Update-All-Nuspec($ver)
{
    foreach ($file in @(".\Proto.Service.Interface.Generator.nuspec",".\Proto.Service.ProtoEndPoint.Generator.nuspec",".\Proto.Service.AspNetController.Generator.nuspec"))
    {
        UpdateNuspec $file $ver
    }
}

function UpdateNuspec ([String]$f, [String]$vr)
{
    echo $vr
    echo $f
    $NuspecFilePath = Resolve-Path $f
    echo $NuspecFilePath
    [xml]$fileContents = Get-Content -Path $NuspecFilePath
    echo $fileContents.package.metadata.version
    $fileContents.package.metadata.version = $vr
    $fileContents.Save($NuspecFilePath)
    cmd.exe /c nuget.exe pack $NuspecFilePath
}

#funciton Publish-Nuget()
#{
    #echo "Publishing nuget"
    #Param ([string]$Version)
    # $NupkgFilePath = Resolve-Path .\$NuspecFileName.$Version.nupkg
    # echo $NupkgFilePath
    # rem nuget setApiKey yourApiKey
    # cmd.exe /c nuget push $NupkgFilePath -Source https://api.nuget.org/v3/index.json
#}

function GitPushAndTag()
{
    Param ([string]$Version)
    $Branch = git rev-parse --abbrev-ref HEAD
    echo $Branch
    echo $Version
    $versionFile = Resolve-Path .\Version.cs
    $oldGitUserName = git config user.name
    $oldGitUserEmail = git config user.email
    git config user.name gulshan
    git config user.email gulshanjitm@gmail.com
    git add $versionFile
    git add Resolve-Path .\Proto.Service.Interface.Generator.nuspec
    git add Resolve-Path .\Proto.Service.ProtoEndPoint.Generator.nuspec
    git add Resolve-Path .\Proto.Service.AspNetController.Generator.nuspec
    git add Resolve-Path ..\IntefaceGeneratorReleases.md
    git add Resolve-Path ..\EndPointGeneratorReleases.md
    git add Resolve-Path ..\ControllerGeneratorReleases.md
    git commit -m "Updated version number to $Version"
    git push origin $Branch

    git tag -a Generator_$Version -m "Tag Created for the version $Version"
    git push origin Generator_$Version
    git config user.name $oldGitUserName
    git config user.email $oldGitUserEmail 
}

Write-Host "Build Library" -ForegroundColor Green
$v = READ-HOST -Prompt "Enter Version of build:"

$r = [System.Text.RegularExpressions.Regex]::Match($v, "^[0-9]+(\.[0-9]+){1,3}$");

if($r.Success){
    Update-AllAssemblyInfoFiles $v
    Build-All
    Update-All-Nuspec $v
    # Publish-Nuget $v
    # GitPushAndTag $v
}
else{
    Write-Host "Bad input!" -ForegroundColor Red
}
