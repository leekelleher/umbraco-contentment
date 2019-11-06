# Copyright © 2019 Lee Kelleher.
# This Source Code Form is subject to the terms of the Mozilla Public
# License, v. 2.0. If a copy of the MPL was not distributed with this
# file, You can obtain one at https://mozilla.org/MPL/2.0/.

param(
    [string]$SolutionDir,
    [string]$TargetDir,
    [string]$ProjectName,
    [string]$ProjectDir,
    [string]$TargetDevWebsite,
    [string]$ConfigurationName
);

Write-Host $ConfigurationName;

if ($ConfigurationName -eq 'Debug') {
  Write-Host $SolutionDir;
  Write-Host $TargetDir;
  Write-Host $ProjectName;
  Write-Host $ProjectDir;
  Write-Host $TargetDevWebsite;
}

$targetFolder = "${SolutionDir}..\build\assets";

# Copy DLL / PDB
$binFolder = "${targetFolder}\bin";
if (!(Test-Path -Path $binFolder)) {New-Item -Path $binFolder -Type Directory;}
Copy-Item -Path "${TargetDir}${ProjectName}.*" -Destination $binFolder;

# Copy package front-end files assets
$pluginFolder = "${targetFolder}\App_Plugins\Contentment\";
if (!(Test-Path -Path $pluginFolder)) {New-Item -Path $pluginFolder -Type Directory;}
Copy-Item -Path "${ProjectDir}Web\UI\App_Plugins\Contentment\*" -Force -Recurse -Destination "${pluginFolder}";

# Load WebMarkupMin (for minification)
[Reflection.Assembly]::LoadFile("${SolutionDir}..\tools\lib\AdvancedStringBuilder.dll");
[Reflection.Assembly]::LoadFile("${SolutionDir}..\tools\lib\WebMarkupMin.Core.dll");
$htmlMinifier = [WebMarkupMin.Core.HtmlMinifier]::new();

# HTML - Copy and Minify (or just remove comments)
$htmlFiles = Get-ChildItem -Path "${ProjectDir}DataEditors" -Recurse -Force -Include *.html;
foreach($htmlFile in $htmlFiles){
  $contents = Get-Content -Path $htmlFile.FullName;
  $minifiedHtml = $htmlMinifier.Minify($contents).MinifiedContent;
  Set-Content -Path "${pluginFolder}\editors\$($htmlFile.Name)" -Value $minifiedHtml;
}

# CSS - Bundle & Minify
$targetCssPath = "${pluginFolder}contentment.css";
Get-Content -Path "${ProjectDir}DataEditors\**\*.css" | Set-Content -Path $targetCssPath;
& "${SolutionDir}..\tools\AjaxMinifier.exe" $targetCssPath -o $targetCssPath

# JS - Bundle & Minify
$targetJsPath = "${pluginFolder}contentment.js";
Get-Content -Path "${ProjectDir}DataEditors\**\*.js" | Set-Content -Path $targetJsPath;
& "${SolutionDir}..\tools\AjaxMinifier.exe" $targetJsPath -o $targetJsPath

# In debug mode, copy the assets over to the local dev website
if ($ConfigurationName -eq 'Debug' -AND -NOT($TargetDevWebsite -eq '')) {
  Copy-Item -Path "${targetFolder}\*" -Force -Recurse -Destination $TargetDevWebsite;
}