  list p=16f870
  #include "p16f870.inc"
  #include "config.inc"

  EXTERN InitLCD

;------------------
SaveContext macro		; ����. W � STATUS
  movwf WBuf
  swapf WBuf,F
  swapf STATUS,W
  movwf StatBuf
  endm
RestContext macro		; �����. W � STATUS
  swapf StatBuf,W
  movwf STATUS
  swapf WBuf,W
  endm

;------------------
SharedData			UDATA_SHR
WBuf				res 1			; ���� W
StatBuf				res 1			; ���� STATUS
TempB				res 1
  GLOBAL TempB
;------------------
START_VEC			CODE h'0000'
  nop
MAIN_VEC			CODE h'0001'
  goto main
INT_VEC				CODE h'0004'
  goto interrupt	; ��������� ����������
;---------------
MAIN_FUNC			CODE
main
  call InitLCD
mainloop
  goto mainloop
;----------------
INT_FUNC			CODE
interrupt			; ���������� ����������
  SaveContext

intexit
  RestContext
  retfie
;----------------
  end