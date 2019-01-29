; 直接从官网拷贝的字符串查找方法
; https://nsis.sourceforge.io/StrContains
; StrContains
; This function does a case sensitive searches for an occurrence of a substring in a string. 
; It returns the substring if it is found. 
; Otherwise it returns null(""). 
; Written by kenglish_hi
; Adapted from StrReplace written by dandaman32
Var STR_HAYSTACK
Var STR_NEEDLE
Var STR_CONTAINS_VAR_1
Var STR_CONTAINS_VAR_2
Var STR_CONTAINS_VAR_3
Var STR_CONTAINS_VAR_4
Var STR_RETURN_VAR
 
Function StrContains
  Exch $STR_NEEDLE
  Exch 1
  Exch $STR_HAYSTACK
  ; Uncomment to debug
  ;MessageBox MB_OK 'STR_NEEDLE = $STR_NEEDLE STR_HAYSTACK = $STR_HAYSTACK '
    StrCpy $STR_RETURN_VAR ""
    StrCpy $STR_CONTAINS_VAR_1 -1
    StrLen $STR_CONTAINS_VAR_2 $STR_NEEDLE
    StrLen $STR_CONTAINS_VAR_4 $STR_HAYSTACK
    loop:
      IntOp $STR_CONTAINS_VAR_1 $STR_CONTAINS_VAR_1 + 1
      StrCpy $STR_CONTAINS_VAR_3 $STR_HAYSTACK $STR_CONTAINS_VAR_2 $STR_CONTAINS_VAR_1
      StrCmp $STR_CONTAINS_VAR_3 $STR_NEEDLE found
      StrCmp $STR_CONTAINS_VAR_1 $STR_CONTAINS_VAR_4 done
      Goto loop
    found:
      StrCpy $STR_RETURN_VAR $STR_NEEDLE
      Goto done
    done:
   Pop $STR_NEEDLE ;Prevent "invalid opcode" errors and keep the
   Exch $STR_RETURN_VAR  
FunctionEnd
 
!macro _StrContainsConstructor OUT NEEDLE HAYSTACK
  Push `${HAYSTACK}`
  Push `${NEEDLE}`
  Call StrContains
  Pop `${OUT}`
!macroend
 
!define StrContains '!insertmacro "_StrContainsConstructor"'

;引用if操作库(逻辑库)
!include "LogicLib.nsh"

;命名
Name "FPlug"
;输出的文件名字
OutFile "..\FPlug.exe"
;设置exe文件的图标样式
Icon "icon.ico"
;安装的时候是否要求管理员权限
RequestExecutionLevel "admin"
;指定压缩方式
SetCompressor lzma
;显示在底部的文案
BrandingText "FPlug v1.0.2"
;设置安装路径
;$PROGRAMFILES：程序文件目录(通常为 C:\Program Files 但是运行时会检测)
InstallDir "$PROGRAMFILES\Fiddler\Scripts\"
;读取注册表中Fiddler的安装路径(读取失败的情况下会使用上一步的路径)
InstallDirRegKey HKCU "SOFTWARE\Microsoft\Fiddler2\InstallerSettings" "ScriptPath"

;包含的页面
Page directory ;installation directory selection page
Page instfiles ;installation page where the sections are executed

;一个命令区段
Section "Main"
	;判断是否包含必要字段
	${StrContains} $0 "Fiddler" $INSTDIR
	${StrContains} $1 "Scripts" $INSTDIR
	;如果结果为空(即不包含必要字段)
	${If} $0 == ""
	${OrIf} $1 == ""
		MessageBox MB_OK|MB_ICONEXCLAMATION "请选择Fiddler安装目录下的Scripts文件夹"
		;返回目录选择页
		SendMessage $HWNDPARENT 0x408 -1 0
		;不执行后面的代码
		Abort
	${EndIf}
	;$INSTDIR：用户定义的解压路径
	SetOutPath "$INSTDIR"
	;是否开启覆写模式
	SetOverwrite on
	;需要打包进exe的文件
	File "..\FPlug.dll"
	File "..\Newtonsoft.Json.dll"
	;输出到日志中
	DetailPrint "安装路径：$INSTDIR"
	DetailPrint "安装成功！"
	;使用MessageBox弹出一个对话框
	MessageBox MB_OK|MB_ICONEXCLAMATION "安装成功"
SectionEnd

;是否显示安装日志
ShowInstDetails show