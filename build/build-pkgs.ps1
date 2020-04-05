# Copyright © 2019 Lee Kelleher.
# This Source Code Form is subject to the terms of the Mozilla Public
# License, v. 2.0. If a copy of the MPL was not distributed with this
# file, You can obtain one at https://mozilla.org/MPL/2.0/.


# https://gist.github.com/jageall/c5119d5ba26fa33602d1
Function parseSemVer($version) {
    $version -match "^(?<major>\d+)(\.(?<minor>\d+))?(\.(?<patch>\d+))?(\-(?<pre>[0-9A-Za-z\-\.]+))?(\+(?<build>[0-9A-Za-z\-\.]+))?$" | Out-Null;
    $major = [int]$matches['major'];
    $minor = [int]$matches['minor'];
    $patch = [int]$matches['patch'];
    $pre = [string]$matches['pre'];
    $build = [string]$matches['build'];

    New-Object PSObject -Property @{
        Major = $major
        Minor = $minor
        Patch = $patch
        Pre = $pre
        Build = $build
        VersionString = $version
    };
}


# Set various variables / folder paths

$projectNamespace = 'Umbraco.Community.Contentment';
$packageName = 'Contentment';
$packageDescription = 'Contentment, a collection of components for Umbraco 8.';
$packageUrl = 'https://github.com/leekelleher/umbraco-contentment';
$iconUrl = 'https://raw.githubusercontent.com/leekelleher/umbraco-contentment/master/docs/assets/img/logo.png';
$iconPath = 'content/App_Plugins/Contentment/contentment.png';
$licenseName = 'Mozilla Public License Version 2.0';
$licenseUrl = 'https://mozilla.org/MPL/2.0/';
$authorName = 'Lee Kelleher';
$authorUrl = 'https://leekelleher.com/';
$minUmbracoVersion = parseSemVer('8.4.0');
$copyright = "Copyright " + [char]0x00A9 + " " + (Get-Date).year + " $authorName";

$rootFolder = (Get-Item($MyInvocation.MyCommand.Path)).Directory.Parent.FullName;
$buildFolder = Join-Path -Path $rootFolder -ChildPath 'build';
$assetsFolder = Join-Path -Path $buildFolder -ChildPath 'assets';
$srcFolder = Join-Path -Path $rootFolder -ChildPath 'src';


# Get some package metadata - name, description, links, etc.

$version = Get-Content -Path "${rootFolder}\VERSION";
$semver = parseSemVer($version);


# Update the assembly version number

Set-Content -Path "${srcFolder}\${projectNamespace}\Properties\VersionInfo.cs" -Value @"
using System.Reflection;

[assembly: AssemblyVersion("$($semver.Major).$($semver.Minor)")]
[assembly: AssemblyFileVersion("$($semver.Major).$($semver.Minor).$($semver.Patch)")]
[assembly: AssemblyInformationalVersion("$($semver.VersionString)")]
"@ -Encoding UTF8;


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
& $msbuild_exe "${srcFolder}\${projectNamespace}.sln" /p:Configuration=Release
if (-NOT $?) {
    throw 'The MSBuild process returned an error code.';
}


# Populate the Umbraco package manifest

$umbFolder = Join-Path -Path $buildFolder -ChildPath '__umb';
if (!(Test-Path -Path $umbFolder)) {New-Item -Path $umbFolder -Type Directory;}

$umbracoManifest = Join-Path -Path $buildFolder -ChildPath 'manifest-umbraco.xml';
$umbracoPackageXml = [xml](Get-Content $umbracoManifest);
$umbracoPackageXml.umbPackage.info.package.version = "$($semver.Major).$($semver.Minor).$($semver.Patch)";
$umbracoPackageXml.umbPackage.info.package.name = $packageName;
$umbracoPackageXml.umbPackage.info.package.iconUrl = $iconUrl;
$umbracoPackageXml.umbPackage.info.package.license.set_InnerText($licenseName);
$umbracoPackageXml.umbPackage.info.package.license.url = $licenseUrl;
$umbracoPackageXml.umbPackage.info.package.url = $packageUrl;
$umbracoPackageXml.umbPackage.info.package.requirements.major = "$($minUmbracoVersion.Major)";
$umbracoPackageXml.umbPackage.info.package.requirements.minor = "$($minUmbracoVersion.Minor)";
$umbracoPackageXml.umbPackage.info.package.requirements.patch = "$($minUmbracoVersion.Patch)";
$umbracoPackageXml.umbPackage.info.author.name = $authorName;
$umbracoPackageXml.umbPackage.info.author.website = $authorUrl;
$umbracoPackageXml.umbPackage.info.readme.'#cdata-section' = $packageDescription;

$filesXml = $umbracoPackageXml.CreateElement('files');

$assetFiles = Get-ChildItem -Path $assetsFolder -File -Recurse;
foreach($assetFile in $assetFiles){

    $hash = Get-FileHash -Path $assetFile.FullName -Algorithm MD5;
    $guid = $hash.Hash.ToLower() + $assetFile.Extension;
    $orgPath = "~" + $assetFile.Directory.FullName.Replace($assetsFolder, "").Replace("\", "/");

    $fileXml = $umbracoPackageXml.CreateElement("file");
    $fileXml.set_InnerXML("<guid>${guid}</guid><orgPath>${orgPath}</orgPath><orgName>$($assetFile.Name)</orgName>");
    $filesXml.AppendChild($fileXml);

    Copy-Item -Path $assetFile.FullName -Destination "${umbFolder}\${guid}";
}

$umbracoPackageXml.umbPackage.ReplaceChild($filesXml, $umbracoPackageXml.SelectSingleNode("/umbPackage/files")) | Out-Null;
$umbracoPackageXml.Save("${umbFolder}\package.xml");

$artifactsFolder = Join-Path -Path $rootFolder -ChildPath 'artifacts';
if (!(Test-Path -Path $artifactsFolder)) {New-Item -Path $artifactsFolder -Type Directory;}
Compress-Archive -Path "${umbFolder}\*" -DestinationPath "${artifactsFolder}\Contentment_$($semver.VersionString).zip" -Force;


# Populate the NuGet package manifest

$nugetPackageManifest = Join-Path -Path $buildFolder -ChildPath 'manifest-nuget.nuspec';
& $nuget_exe pack $nugetPackageManifest -BasePath $assetsFolder -OutputDirectory $artifactsFolder -Version "$($semver.VersionString)" -Properties "id=$projectNamespace;version=$($semver.VersionString);title=$packageName for Umbraco;authors=$authorName;owners=$authorName;projectUrl=$packageUrl;icon=$iconPath;requireLicenseAcceptance=false;description=$packageDescription;copyright=$copyright;license=MPL-2.0;language=en;tags=umbraco;minUmbracoVersion=$($minUmbracoVersion.VersionString);repositoryUrl=$packageUrl;"


# Tidy up folders
Remove-Item -Recurse -Force $umbFolder;

