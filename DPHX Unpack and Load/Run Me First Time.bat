@echo off
setlocal ENABLEEXTENSIONS
title DPHX Unpack ^& Load - First-Time Starter

REM Change to the folder where this .bat lives
cd /d "%~dp0"

REM ---- Filenames (must be in this folder) ----
set "LISTENER=WSGListener.exe"
set "APP=DPHX Unpack and Load.exe"

REM ---- Quick validation ----
if not exist "%LISTENER%" (
  echo [ERROR] "%LISTENER%" not found in: "%cd%"
  echo Make sure you unzipped everything in the same folder as this batch file.
  pause
  exit /b 1
)
if not exist "%APP%" (
  echo [ERROR] "%APP%" not found in: "%cd%"
  echo Make sure you unzipped everything in the same folder as this batch file.
  pause
  exit /b 1
)

REM ---- Start listener if not already running ----
echo Ensuring %LISTENER% is running...
tasklist /FI "IMAGENAME eq %LISTENER%" | find /I "%LISTENER%" >nul
if errorlevel 1 (
  echo Starting %LISTENER%...
  start "" "%cd%\%LISTENER%"
) else (
  echo %LISTENER% is already running.
)

REM ---- Verify listener is actually up (retry up to ~10s) ----
set "OK="
for /l %%I in (1,1,10) do (
  tasklist /FI "IMAGENAME eq %LISTENER%" | find /I "%LISTENER%" >nul && (
    set "OK=1"
    goto :listener_ok
  )
  >nul timeout /t 1 /nobreak
)
:listener_ok

if not defined OK (
  echo [ERROR] Could not confirm %LISTENER% is running.
  echo Please check antivirus prompts or Windows SmartScreen, then run this again.
  echo.
  pause
  exit /b 2
)

echo %LISTENER% confirmed running.
echo.

REM ---- Launch main app ----
echo Launching "%APP%"...
start "" "%cd%\%APP%"
echo.

echo Setup complete! The tool should now be running.
echo Press ENTER to close this window.
pause >nul
exit /b 0
