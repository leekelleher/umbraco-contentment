# Copyright © 2019 Lee Kelleher.
# This Source Code Form is subject to the terms of the Mozilla Public
# License, v. 2.0. If a copy of the MPL was not distributed with this
# file, You can obtain one at https://mozilla.org/MPL/2.0/.

param(
    [string]$TargetDir,
    [string]$ProjectName,
    [string]$ProjectDir,
    [string]$ConfigurationName
);

$rootDir = "${ProjectDir}..\..";
. "${rootDir}\src\_vars.ps1";

Write-Host $ConfigurationName;

if ($ConfigurationName -eq 'Debug') {
    Write-Host $TargetDir;
    Write-Host $ProjectName;
    Write-Host $ProjectDir;
    Write-Host $TargetDevWebsite;
}

$targetFolder = "${rootDir}\build\assets";

# If it already exists, delete it
if (Test-Path -Path $targetFolder) {
    Remove-Item -Recurse -Force $targetFolder;
}

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

# CSS (Property Editors) - Copy
$cssFiles = Get-ChildItem -Path "${ProjectDir}DataEditors" -Recurse -Force -Include *.css;
foreach($cssFile in $cssFiles){
    $contents = Get-Content -Raw -Path $cssFile.FullName;
    [IO.File]::WriteAllLines("${pluginFolder}\editors\$($cssFile.Name)", $contents);
}

# JS (Property Editors) - Copy
$jsFiles = Get-ChildItem -Path "${ProjectDir}DataEditors" -Recurse -Force -Include *.js;
foreach($jsFile in $jsFiles){
    $contents = Get-Content -Raw -Path $jsFile.FullName;
    [IO.File]::WriteAllLines("${pluginFolder}\editors\$($jsFile.Name)", $contents);
}

# In debug mode, copy the assets over to the local dev website
if ($ConfigurationName -eq 'Debug' -AND -NOT($TargetDevWebsite -eq '')) {
    Copy-Item -Path "${targetFolder}\*" -Force -Recurse -Destination $TargetDevWebsite;
}