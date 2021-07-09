# Copyright © 2019 Lee Kelleher.
# This Source Code Form is subject to the terms of the Mozilla Public
# License, v. 2.0. If a copy of the MPL was not distributed with this
# file, You can obtain one at https://mozilla.org/MPL/2.0/.


# Set various variables / folder paths

$nugetPackageId = 'Our.Umbraco.Community.Contentment';
$projectNamespace = 'Umbraco.Community.Contentment';
$nugetTitle = "Contentment for Umbraco";
$packageDescription = "Contentment, a collection of components for Umbraco.";
$packageUrl = 'https://github.com/leekelleher/umbraco-contentment';
$authorName = 'Lee Kelleher';
$minUmbracoVersion = "9.0.0-rc001";
$copyright = "" + [char]0x00A9 + " " + (Get-Date).year + " $authorName";
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
Write-Host 'Cleaning Visual Studio solution.';
Remove-Item -Recurse -Force "${srcFolder}\${projectNamespace}\obj";
& dotnet clean "${srcFolder}\${projectNamespace}.sln";

Write-Host 'Compiling Visual Studio solution.';
& dotnet build "${srcFolder}\${projectNamespace}.sln" --configuration Release
if (-NOT $?) {
    throw 'The dotnet CLI returned an error code.';
}


# Copy DLL to assets folder
$binFolder = "${assetsFolder}\bin";
if (!(Test-Path -Path $binFolder)) {New-Item -Path $binFolder -Type Directory;}
Copy-Item -Path "${srcFolder}\${projectNamespace}\bin\Release\net5.0\${projectNamespace}.dll" -Destination $binFolder;


# Ensure the artifacts folder
$artifactsFolder = Join-Path -Path $rootFolder -ChildPath "artifacts";
if (!(Test-Path -Path $artifactsFolder)) {New-Item -Path $artifactsFolder -Type Directory;}

# Ensure NuGet.exe
$nuget_exe = "${rootFolder}\tools\nuget.exe";
If (-NOT(Test-Path -Path $nuget_exe)) {
    Write-Host "Retrieving nuget.exe...";
    Invoke-WebRequest "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe" -OutFile $nuget_exe;
}

# Populate the NuGet package manifest
Copy-Item -Path "${rootFolder}\docs\assets\img\logo.png" -Destination "${assetsFolder}\icon.png";
Copy-Item -Path "${buildFolder}\_nuget-post-install.targets" -Destination "${assetsFolder}\${nugetPackageId}.targets";
& $nuget_exe pack "${buildFolder}\manifest-nuget-core.nuspec" -BasePath $assetsFolder -OutputDirectory $artifactsFolder -Version "$version" -Properties "id=$nugetPackageId;version=$version;title=$nugetTitle;authors=$authorName;owners=$authorName;projectUrl=$packageUrl;requireLicenseAcceptance=false;description=$packageDescription;copyright=$copyright;license=MPL-2.0;language=en;tags=$tags;minUmbracoVersion=$minUmbracoVersion;repositoryUrl=$packageUrl;"
& $nuget_exe pack "${buildFolder}\manifest-nuget-web.nuspec" -BasePath $assetsFolder -OutputDirectory $artifactsFolder -Version "$version" -Properties "id=$nugetPackageId;version=$version;title=$nugetTitle;authors=$authorName;owners=$authorName;projectUrl=$packageUrl;requireLicenseAcceptance=false;description=$packageDescription;copyright=$copyright;license=MPL-2.0;language=en;tags=$tags;minUmbracoVersion=$minUmbracoVersion;repositoryUrl=$packageUrl;"
