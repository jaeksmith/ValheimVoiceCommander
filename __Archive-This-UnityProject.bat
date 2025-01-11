@echo off
SET scriptPath=\L\Scripts\Archive-UnityProject.ps1
SET projectPath=%cd%

PowerShell -NoProfile -ExecutionPolicy Bypass -Command "& '%scriptPath%' -projectPath '%projectPath%'"
pause
