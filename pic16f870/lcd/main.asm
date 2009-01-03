  list p=16f870
  #include "p16f870.inc"
  #include "config.inc"

  EXTERN InitLCD

;------------------
SaveContext macro		; сохр. W и STATUS
  movwf WBuf
  swapf WBuf,F
  swapf STATUS,W
  movwf StatBuf
  endm
RestContext macro		; восст. W и STATUS
  swapf StatBuf,W
  movwf STATUS
  swapf WBuf,W
  endm

;------------------
SharedData			UDATA_SHR
WBuf				res 1			; сохр W
StatBuf				res 1			; сохр STATUS
TempB				res 1
  GLOBAL TempB
;------------------
START_VEC			CODE h'0000'
  nop
MAIN_VEC			CODE h'0001'
  goto main
INT_VEC				CODE h'0004'
  goto interrupt	; обработка прерываний
;---------------
MAIN_FUNC			CODE
main
  call InitLCD
mainloop
  goto mainloop
;----------------
INT_FUNC			CODE
interrupt			; обработчик прерываний
  SaveContext

intexit
  RestContext
  retfie
;----------------
  end