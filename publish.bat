@echo off
echo ========================================
echo   Building Sorting Visualizer
echo   Single File Executable
echo ========================================
echo.

echo Cleaning previous builds...
dotnet clean -c Release

echo.
echo Publishing as Single File...
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o ./publish

echo.
echo ========================================
echo   Build Complete!
echo ========================================
echo.
echo Output file: publish\SortingVisualizer.exe
echo.
echo You can now share this single .exe file
echo with your teammates. No installation required!
echo.
pause
