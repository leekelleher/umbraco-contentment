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

# Set various variables / paths

$projectNamespace = 'Umbraco.Community.Contentment';

$rootPath = (Get-Item($MyInvocation.MyCommand.Path)).Directory.Parent.FullName;
$buildPath = Join-Path -Path $rootPath -ChildPath 'build';
$assetsPath = Join-Path -Path $buildPath -ChildPath 'assets';
$srcPath = Join-Path -Path $rootPath -ChildPath 'src';

# Get some package metadata - name, description, links, etc.

$version = Get-Content -Path "${rootPath}\VERSION";
$semver = parseSemVer($version);

# Update the assembly version number

Set-Content -Path "${srcPath}\Umbraco.Community.Contentment\Properties\VersionInfo.cs" -Value @"
using System.Reflection;

[assembly: AssemblyVersion("$($semver.Major).$($semver.Minor)")]
[assembly: AssemblyFileVersion("$($semver.Major).$($semver.Minor).$($semver.Patch)")]
[assembly: AssemblyInformationalVersion("$($semver.VersionString)")]
"@ -Encoding UTF8;


# Build the VS project

# Ensure NuGet.exe
$nuget_exe = "${rootPath}\tools\nuget.exe";
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
& $nuget_exe restore "${srcPath}\${projectNamespace}.sln";

Write-Host 'Compiling Visual Studio solution.';
& $msbuild_exe "${srcPath}\${projectNamespace}.sln" /p:Configuration=Release
if (-NOT $?) {
    throw 'The MSBuild process returned an error code.';
}

# Populate the Umbraco package manifest


$umbracoPackageManifest = Join-Path -Path $buildPath -ChildPath 'manifest-umbraco.xml';


# Populate the NuGet package manifest

$nugetPackageManifest = Join-Path -Path $buildPath -ChildPath 'manifest-nuget.xml';


# Anything else?
