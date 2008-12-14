  list p=16f870
  #include "p16f870.inc"
  __config h'3F3A'

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
WBuf equ 0x70			; сохр W
StatBuf equ 0x71		; сохр STATUS

DelayI equ 0x72
DelayJ equ 0x73
TempC equ 0x74
TempB equ 0x75

Temp equ 0x76			; хлам
DataOUT equ 0x77
;------------------
  org h'00'
  nop
  org h'01'
  goto main

  org h'04'
  goto interrupt	; обработка прерываний
;---------------
main
  call InitLCD
mainloop
  goto mainloop
;----------------
interrupt			; обработчик прерываний
  SaveContext
intexit
  RestContext
  retfie
;----------------
; --------------------------------
  #include "lcd_WH1602D.asm"	; определены функции для работы с WH1602D
; --------------------------------
  end