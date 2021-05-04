# Copyright © 2019 Lee Kelleher.
# This Source Code Form is subject to the terms of the Mozilla Public
# License, v. 2.0. If a copy of the MPL was not distributed with this
# file, You can obtain one at https://mozilla.org/MPL/2.0/.

param(
    [string]$SolutionDir,
    [string]$TargetDir,
    [string]$ProjectName,
    [string]$ProjectDir,
    [string]$ConfigurationName
);

. "${SolutionDir}_vars.ps1";

Write-Host $ConfigurationName;

if ($ConfigurationName -eq 'Debug') {
    Write-Host $SolutionDir;
    Write-Host $TargetDir;
    Write-Host $ProjectName;
    Write-Host $ProjectDir;
    Write-Host $TargetDevWebsite;
}

$targetFolder = "${SolutionDir}..\build\assets";

# If it already exists, delete it
if (Test-Path -Path $targetFolder) {
    Remove-Item -Recurse -Force $targetFolder;
}

# Copy DLL / PDB
$binFolder = "${targetFolder}\bin";

if (!(Test-Path -Path $binFolder)) {New-Item -Path $binFolder -Type Directory;}
Copy-Item -Path "${TargetDir}${ProjectName}.*" -Destination $binFolder;

# Copy package front-end files assets
$pluginFolder = "${targetFolder}\App_Plugins\Contentment\";
if (!(Test-Path -Path $pluginFolder)) {New-Item -Path $pluginFolder -Type Directory;}
Copy-Item -Path "${ProjectDir}Web\UI\App_Plugins\Contentment\*" -Force -Recurse -Destination "${pluginFolder}";

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
Get-Content -Raw -Path "${ProjectDir}**\**\*.css" | Set-Content -Path $targetCssPath;
& "${SolutionDir}..\tools\AjaxMinifier.exe" $targetCssPath -o $targetCssPath

# JS - Bundle & Minify
$targetJsPath = "${pluginFolder}contentment.js";
Get-Content -Raw -Path "${ProjectDir}**\**\*.js" | Set-Content -Path $targetJsPath;
& "${SolutionDir}..\tools\AjaxMinifier.exe" $targetJsPath -o $targetJsPath

# In debug mode, copy the assets over to the local dev website
if ($ConfigurationName -eq 'Debug' -AND -NOT($TargetDevWebsite -eq '')) {
    Copy-Item -Path "${targetFolder}\*" -Force -Recurse -Destination $TargetDevWebsite;
}