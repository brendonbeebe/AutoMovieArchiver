!define PRODUCT_NAME "Auto Movie Archive" 
!define PRODUCT_VERSION "0.1" 
!define PRODUCT_PUBLISHER "TJ Young" 
!define PRODUCT_WEB_SITE "http://brendonbeebe.github.com/AutoMovieArchive/"
!define PRODUCT_ID "AMA"
!define RUN_FILE "HelloWorld"

; MUI 1.67 compatible ------
!include "MUI.nsh"

; MUI Settings
!define MUI_ICON "dvd.ico"
!define MUI_UNICON "dvd.ico"
!define MUI_ABORTWARNING
!insertmacro MUI_PAGE_WELCOME
!define MUI_LICENSEPAGE_RADIOBUTTONS
!insertmacro MUI_PAGE_LICENSE "License.txt"
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH
!insertmacro MUI_LANGUAGE "English"

Name "${PRODUCT_NAME} ${PRODUCT_VERSION}"
OutFile "setup.exe"
InstallDir "$PROGRAMFILES\AMA"


Section ""
	SetOutPath "$INSTDIR"
	SetOverwrite ifnewer
	File /r "pkgs"
	ReadRegStr $0 HKLM "SOFTWARE\VideoLAN\VLC" "Version"
	${if} $0 == "2.0.5"
		goto endvlc
	${EndIf}
	ExecWait '"$INSTDIR\pkgs\vlc.exe"'
	endvlc:
	;install python
	ExecWait '"msiexec" /i "$INSTDIR\pkgs\python.msi"  /passive'
	ReadRegStr $0 HKLM "SYSTEM\CurrentControlSet\Control\Session Manager\Environment" "path"
	WriteRegStr HKLM "SYSTEM\CurrentControlSet\Control\Session Manager\Environment" "path" "$0;c:\python33"
	;install IronPython
	ExecWait '"msiexec" /i "$INSTDIR\pkgs\IronPython.msi"  /passive'
	ReadRegStr $0 HKLM "SYSTEM\CurrentControlSet\Control\Session Manager\Environment" "path"
	WriteRegStr HKLM "SYSTEM\CurrentControlSet\Control\Session Manager\Environment" "path" "$0;C:\Program Files\IronPython 2.7"
	;install MakeMKV
	ExecWait '"$INSTDIR\pkgs\MakeMKV.exe"'
	;create desktop shortcut
	CreateShortCut "$DESKTOP\${PRODUCT_ID}.lnk" "$INSTDIR\pkgs\${RUN_FILE}.py" ""
 
	;create start-menu items
	CreateDirectory "$SMPROGRAMS\${PRODUCT_ID}"
	CreateShortCut "$SMPROGRAMS\${PRODUCT_ID}\Uninstall.lnk" "$INSTDIR\Uninstall.exe" "" "$INSTDIR\Uninstall.exe" 0
	CreateShortCut "$SMPROGRAMS\${PRODUCT_ID}\${PRODUCT_ID}.lnk" "$INSTDIR\pkgs\${RUN_FILE}.py" "" "$INSTDIR\pkgs\${RUN_FILE}.py" 0
 
	;write uninstall information to the registry
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_ID}" "DisplayName" "${PRODUCT_ID} (remove only)"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_ID}" "UninstallString" "$INSTDIR\Uninstall.exe"
 
	WriteUninstaller "$INSTDIR\Uninstall.exe"
SectionEnd


;--------------------------------    
;Uninstaller Section  
Section "Uninstall"
	;Delete Files 
	RMDir /r "$INSTDIR\*.*"    
 
	;Remove the installation directory
	RMDir "$INSTDIR"
 
	;Delete Start Menu Shortcuts
	Delete "$DESKTOP\${PRODUCT_ID}.lnk"
	Delete "$SMPROGRAMS\${PRODUCT_ID}\*.*"
	RMDir  "$SMPROGRAMS\${PRODUCT_ID}"
 
	;Delete Uninstaller And Unistall Registry Entries
	DeleteRegKey HKEY_LOCAL_MACHINE "SOFTWARE\${PRODUCT_ID}"
	DeleteRegKey HKEY_LOCAL_MACHINE "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_ID}"  
SectionEnd