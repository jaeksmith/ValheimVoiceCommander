SendMode("Input") ; Use SendInput for fast and reliable key sending
SetTitleMatchMode(2) ; Match window titles containing "Valheim"

; Activate Valheim window and send 'm'
; if WinActivate("Valheim")
SetTitleMatchMode(2) ; Partial matching for window titles
WinActivate("ahk_exe valheim.exe")
Sleep(100) ; Wait 100 ms
if WinActive("ahk_exe valheim.exe")
{
    WinGetPos(&X, &Y, &Width, &Height, "ahk_exe valheim.exe")
    MouseClick("Right", X + Width // 2, Y + Height // 2)

    Sleep(100) ; Small delay after the click

    Send("{f down}")
	Sleep(50) ; Hold the key for 50ms
	Send("{f up}")
}
else
{
    MsgBox("Could not activate the Valheim window.", "Error", 48)
}