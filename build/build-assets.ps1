# Copyright © 2019 Lee Kelleher.
# This Source Code Form is subject to the terms of the Mozilla Public
# License, v. 2.0. If a copy of the MPL was not distributed with this
# file, You can obtain one at https://mozilla.org/MPL/2.0/.

param(
    [string]$TargetFramework,
    [string]$TargetDir,
    [string]$ProjectName,
    [string]$ProjectDir,
    [string]$ConfigurationName
);

$rootDir = "${ProjectDir}..\..";
. "${rootDir}\src\_vars.ps1";

Write-Host "ConfigurationName = $ConfigurationName";

if ($ConfigurationName -eq 'Debug') {
    Write-Host "TargetFramework = $TargetFramework";
    Write-Host "TargetDir = $TargetDir";
    Write-Host "ProjectName = $ProjectName";
    Write-Host "ProjectDir = $ProjectDir";
    Write-Host "TargetDevWebsite = $TargetDevWebsite";
}

$targetFolder = "${rootDir}\build\assets";

# If it already exists, delete it
if (Test-Path -Path $targetFolder) {
    Remove-Item -Recurse -Force $targetFolder;
}

# Copy DLL / PDB
if (-NOT($ConfigurationName -eq 'Debug')) {
    $binFolder = "${targetFolder}\bin";
    if (!(Test-Path -Path $binFolder)) {New-Item -Path $binFolder -Type Directory;}
    Copy-Item -Path "${ProjectDir}\bin\${ConfigurationName}\net472\${ProjectName}.*" -Destination $binFolder;
    $net50Folder = "${targetFolder}\net50";
    if (!(Test-Path -Path $net50Folder)) {New-Item -Path $net50Folder -Type Directory;}
    Copy-Item -Path "${ProjectDir}\bin\${ConfigurationName}\net50\${ProjectName}.*" -Destination $net50Folder;
}

# Copy package front-end files assets
$pluginFolder = "${targetFolder}\App_Plugins\Contentment\";
if (!(Test-Path -Path $pluginFolder)) {New-Item -Path $pluginFolder -Type Directory;}
Copy-Item -Path "${ProjectDir}Web\UI\App_Plugins\Contentment\*" -Force -Recurse -Destination "${pluginFolder}";

# // TODO: Update version number in package.manifest file. [LK]

# HTML (Property Editors) - Copy and Minify (or just remove comments)
$htmlFiles = Get-ChildItem -Path "${ProjectDir}DataEditors" -Recurse -Force -Include *.html;
foreach($htmlFile in $htmlFiles){
    $contents = Get-Content -Raw -Path $htmlFile.FullName;
    $minifiedHtml = [Regex]::Replace($contents, "^<!--.*?-->", "", "Singleline");
    $minifiedHtml = [Regex]::Replace($minifiedHtml, "^`r`n`r`n", "", "Singleline");
    $minifiedHtml = [Regex]::Replace($minifiedHtml, "`r`n$", "", "Singleline");
    [IO.File]::WriteAllLines("${pluginFolder}\editors\$($htmlFile.Name)", $minifiedHtml);
}

# Razor Templates - Copy
$razorFiles = Get-ChildItem -Path "${ProjectDir}DataEditors" -Recurse -Force -Include *.cshtml;
foreach($razorFile in $razorFiles){
    $contents = Get-Content -Raw -Path $razorFile.FullName;
    [IO.File]::WriteAllLines("${pluginFolder}\render\$($razorFile.Name)", $contents);
}

# CSS - Bundle & Minify
$targetCssPath = "${pluginFolder}contentment.css";
Get-Content -Raw -Path "${ProjectDir}**\**\*.css" | Set-Content -Encoding UTF8 -Path $targetCssPath;
& "${rootDir}\tools\AjaxMinifier.exe" $targetCssPath -o $targetCssPath

# JS - Bundle & Minify
$targetJsPath = "${pluginFolder}contentment.js";
Get-Content -Raw -Path "${ProjectDir}**\**\*.js" | Set-Content -Encoding UTF8 -Path $targetJsPath;
& "${rootDir}\tools\AjaxMinifier.exe" $targetJsPath -o $targetJsPath

# In debug mode, copy the assets over to the local dev website
if ($ConfigurationName -eq 'Debug' -AND -NOT($TargetDevWebsite -eq '')) {
    Copy-Item -Path "${targetFolder}\*" -Force -Recurse -Destination $TargetDevWebsite;
}