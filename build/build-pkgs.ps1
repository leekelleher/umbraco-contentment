# Copyright © 2019 Lee Kelleher.
# This Source Code Form is subject to the terms of the Mozilla Public
# License, v. 2.0. If a copy of the MPL was not distributed with this
# file, You can obtain one at https://mozilla.org/MPL/2.0/.


# Set various variables / folder paths

$nugetPackageId = 'Our.Umbraco.Community.Contentment';
$projectNamespace = 'Umbraco.Community.Contentment';
$packageName = 'Contentment';
$nugetTitle = "${packageName} for Umbraco";
$packageDescription = "${packageName}, a collection of components for Umbraco.";
$packageUrl = 'https://github.com/leekelleher/umbraco-contentment';
$iconUrl = 'https://raw.githubusercontent.com/leekelleher/umbraco-contentment/master/docs/assets/img/logo.png';
$licenseName = 'Mozilla Public License Version 2.0';
$licenseUrl = 'https://mozilla.org/MPL/2.0/';
$authorName = 'Lee Kelleher';
$authorUrl = 'https://leekelleher.com/';
$minUmbracoVersion = 8,18,0;
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
$net472Folder = "${assetsFolder}\bin";
if (!(Test-Path -Path $net472Folder)) {New-Item -Path $net472Folder -Type Directory;}
Copy-Item -Path "${srcFolder}\${projectNamespace}\bin\Release\net472\${projectNamespace}.*" -Destination $net472Folder;
$net50Folder = "${assetsFolder}\net5.0";
if (!(Test-Path -Path $net50Folder)) {New-Item -Path $net50Folder -Type Directory;}
Copy-Item -Path "${srcFolder}\${projectNamespace}\bin\Release\net5.0\${projectNamespace}.*" -Destination $net50Folder;
$net60Folder = "${assetsFolder}\net6.0";
if (!(Test-Path -Path $net60Folder)) {New-Item -Path $net60Folder -Type Directory;}
Copy-Item -Path "${srcFolder}\${projectNamespace}\bin\Release\net6.0\${projectNamespace}.*" -Destination $net60Folder;

# Populate the Umbraco package manifest

$umbFolder = Join-Path -Path $buildFolder -ChildPath "__umb";
if (!(Test-Path -Path $umbFolder)) {New-Item -Path $umbFolder -Type Directory;}

$umbracoManifest = Join-Path -Path $buildFolder -ChildPath "manifest-umbraco.xml";
$umbracoPackageXml = [xml](Get-Content $umbracoManifest);
$umbracoPackageXml.umbPackage.info.package.version = "$($version)";
$umbracoPackageXml.umbPackage.info.package.name = $packageName;
$umbracoPackageXml.umbPackage.info.package.iconUrl = $iconUrl;
$umbracoPackageXml.umbPackage.info.package.license.set_InnerText($licenseName);
$umbracoPackageXml.umbPackage.info.package.license.url = $licenseUrl;
$umbracoPackageXml.umbPackage.info.package.url = $packageUrl;
$umbracoPackageXml.umbPackage.info.package.requirements.major = "$($minUmbracoVersion[0])";
$umbracoPackageXml.umbPackage.info.package.requirements.minor = "$($minUmbracoVersion[1])";
$umbracoPackageXml.umbPackage.info.package.requirements.patch = "$($minUmbracoVersion[2])";
$umbracoPackageXml.umbPackage.info.author.name = $authorName;
$umbracoPackageXml.umbPackage.info.author.website = $authorUrl;
$umbracoPackageXml.umbPackage.info.readme."#cdata-section" = $packageDescription;

$filesXml = $umbracoPackageXml.CreateElement("files");

$assetFiles = Get-ChildItem -Path $assetsFolder -File -Recurse | Where { $_.FullName -NotLike "*\net*\*" };
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

$artifactsFolder = Join-Path -Path $rootFolder -ChildPath "artifacts";
if (!(Test-Path -Path $artifactsFolder)) {New-Item -Path $artifactsFolder -Type Directory;}
Compress-Archive -Path "${umbFolder}\*" -DestinationPath "${artifactsFolder}\Contentment_$version.zip" -Force;


# Populate the NuGet package manifest

Copy-Item -Path "${rootFolder}\docs\assets\img\logo.png" -Destination "${assetsFolder}\icon.png";
Copy-Item -Path "${buildFolder}\readme-nuget.md" -Destination "${assetsFolder}\README.md";
Copy-Item -Path "${buildFolder}\_nuget-post-install.targets" -Destination "${assetsFolder}\${nugetPackageId}.targets";
& $nuget_exe pack "${buildFolder}\manifest-nuget-core.nuspec" -BasePath $assetsFolder -OutputDirectory $artifactsFolder -Version "$version" -Properties "id=$nugetPackageId;version=$version;title=$nugetTitle;authors=$authorName;owners=$authorName;projectUrl=$packageUrl;requireLicenseAcceptance=false;description=$packageDescription;copyright=$copyright;license=MPL-2.0;language=en;tags=$tags;repositoryUrl=$packageUrl;"
& $nuget_exe pack "${buildFolder}\manifest-nuget-web.nuspec" -BasePath $assetsFolder -OutputDirectory $artifactsFolder -Version "$version" -Properties "id=$nugetPackageId;version=$version;title=$nugetTitle;authors=$authorName;owners=$authorName;projectUrl=$packageUrl;requireLicenseAcceptance=false;description=$packageDescription;copyright=$copyright;license=MPL-2.0;language=en;tags=$tags;repositoryUrl=$packageUrl;"


# Tidy up folders
Remove-Item -Recurse -Force $umbFolder;

