REM     PostBuildEvents.bat is called with the following parameters
REM     %1 is TargetPath
REM     %2 is TargetDir
REM     %3 is ProjectName
REM     %4 is ProjectDir
REM     %5 is ConfigurationName

REM     /S (Copies directories and subdirectories except empty ones)
REM     /Y (Suppresses prompting to confirm you want to overwrite an existing destination file.)

IF NOT %5==Debug EXIT /B 0
IF [%1]==[] EXIT /B 1

SET WEBSITE_PATH=C:\PATH\TO\YOUR\UMBRACO\TEST\INSTALLATION

REM Copy *.dll files to website folder
XCOPY /S /Y "%1" "%WEBSITE_PATH%\bin\"

REM Copy *.pdb files
XCOPY /S /Y "%2%3.pdb" "%WEBSITE_PATH%\bin\"

REM Copy package front-end files assets
XCOPY /S /Y "%4Web\UI\*.*" "%WEBSITE_PATH%"

REM Ensures that the "editors" folder is there.
IF NOT EXIST "%WEBSITE_PATH%\App_Plugins\Contentment\editors\" MKDIR "%WEBSITE_PATH%\App_Plugins\Contentment\editors\"

REM Copy property-editor HTML files
FOR /R "%4DataEditors\" %%f IN (*.html) DO COPY "%%f" "%WEBSITE_PATH%\App_Plugins\Contentment\editors\" /Y

REM Concatenate all the property-editor CSS files into a single file
FOR /R "%4DataEditors\" %%f IN (*.css) DO TYPE "%%f" >> "%WEBSITE_PATH%\App_Plugins\Contentment\contentment.css"

REM Concatenate all the property-editor JS files into a single file
FOR /R "%4DataEditors\" %%f IN (*.js) DO TYPE "%%f" >> "%WEBSITE_PATH%\App_Plugins\Contentment\contentment.js"
