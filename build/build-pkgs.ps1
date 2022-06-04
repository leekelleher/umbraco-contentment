# Copyright © 2019 Lee Kelleher.
# This Source Code Form is subject to the terms of the Mozilla Public
# License, v. 2.0. If a copy of the MPL was not distributed with this
# file, You can obtain one at https://mozilla.org/MPL/2.0/.


# Set various variables / folder paths

$nugetPackageId = 'Our.Umbraco.Community.Contentment';
$nugetTitle = "${packageName} for Umbraco";
$packageDescription = "${packageName}, a collection of components for Umbraco.";
$packageUrl = 'https://github.com/leekelleher/umbraco-contentment';
$authorName = 'Lee Kelleher';
$copyright = "Copyright " + [char]0x00A9 + " " + (Get-Date).year + " $authorName";
$tags = "umbraco";

$rootFolder = (Get-Item($MyInvocation.MyCommand.Path)).Directory.Parent.FullName;
$buildFolder = Join-Path -Path $rootFolder -ChildPath 'build';
$assetsFolder = Join-Path -Path $buildFolder -ChildPath 'assets';
$srcFolder = Join-Path -Path $rootFolder -ChildPath 'src';


# Get package version number
$csprojXml = [xml](Get-Content -Path "${srcFolder}\${projectNamespace}\${projectNamespace}.csproj");
$version = $csprojXml.Project.PropertyGroup.Version;
Write-Host "Package version: $version";

# Build the VS project

# Ensure NuGet.exe
$nuget_exe = "${rootFolder}\tools\nuget.exe";
If (-NOT(Test-Path -Path $nuget_exe)) {
    Write-Host "Retrieving nuget.exe...";
    Invoke-WebRequest "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe" -OutFile $nuget_exe;
}

# vswhere.exe is part of VS2017 (v15.2)+
$vswhere_exe = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe";

$msbuild_exe = & "$vswhere_exe" -Latest -Requires Microsoft.Component.MSBuild -Find MSBuild\**\Bin\MSBuild.exe | Select-Object -First 1
if (-NOT(Test-Path $msbuild_exe)) {
    throw 'MSBuild not found!';
}

Write-Host 'Restoring NuGet packages...';
& $nuget_exe restore "${srcFolder}\${projectNamespace}.sln";

Write-Host 'Compiling Visual Studio solution.';
& $msbuild_exe "${srcFolder}\${projectNamespace}\${projectNamespace}.csproj" /p:Configuration=Release
if (-NOT $?) {
    throw 'The MSBuild process returned an error code.';
}


# Copy the assemblies (DLL / PDB)
$net60Folder = "${assetsFolder}\net6.0";
if (!(Test-Path -Path $net60Folder)) {New-Item -Path $net60Folder -Type Directory;}
Copy-Item -Path "${srcFolder}\${projectNamespace}\bin\Release\net6.0\${projectNamespace}.*" -Destination $net60Folder;


# Populate the NuGet package manifest
Copy-Item -Path "${rootFolder}\docs\assets\img\logo.png" -Destination "${assetsFolder}\icon.png";
Copy-Item -Path "${buildFolder}\readme-nuget.md" -Destination "${assetsFolder}\README.md";
Copy-Item -Path "${buildFolder}\_nuget-post-install.targets" -Destination "${assetsFolder}\${nugetPackageId}.targets";
& $nuget_exe pack "${buildFolder}\manifest-nuget-core.nuspec" -BasePath $assetsFolder -OutputDirectory $artifactsFolder -Version "$version" -Properties "id=$nugetPackageId;version=$version;title=$nugetTitle;authors=$authorName;owners=$authorName;projectUrl=$packageUrl;requireLicenseAcceptance=false;description=$packageDescription;copyright=$copyright;license=MPL-2.0;language=en;tags=$tags;repositoryUrl=$packageUrl;"
& $nuget_exe pack "${buildFolder}\manifest-nuget-web.nuspec" -BasePath $assetsFolder -OutputDirectory $artifactsFolder -Version "$version" -Properties "id=$nugetPackageId;version=$version;title=$nugetTitle;authors=$authorName;owners=$authorName;projectUrl=$packageUrl;requireLicenseAcceptance=false;description=$packageDescription;copyright=$copyright;license=MPL-2.0;language=en;tags=$tags;repositoryUrl=$packageUrl;"


# Tidy up folders
Remove-Item -Recurse -Force $umbFolder;

