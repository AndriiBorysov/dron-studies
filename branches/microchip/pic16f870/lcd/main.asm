  list p=16f870
  #include "p16f870.inc"
  #include "config.inc"
  #include "lcd_WH1602D.asm"

SaveContext macro		; сохр. W и STATUS
  movwf WBuf
  swapf WBuf,F
  swapf STATUS,W
  movwf StatBuf
  endm
;------------------
RestContext macro		; восст. W и STATUS
  swapf StatBuf,W
  movwf STATUS
  swapf WBuf,W
  endm

;------------------
SharedData				UDATA_SHR
WBuf					res 1			; сохр W
StatBuf					res 1			; сохр STATUS
PB_buf					res 1
  GLOBAL PB_buf

LocalData				UDATA_SHR h'0020'
DelayI					res 1
DelayJ					res 1
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
;  LCD_PRINT 0,0,d'10','0','1','2','3','4','5','6','7','8','9','A','B','C','D','E','F'
  LCD_PRINT 0,1,d'16',0xb0,0xb1,0xb2,0xb3,0xb4,0xb5,0xb6,0xb7,0xb8,0xb9,0xba,0xbb,0xbc,0xbd,0xbe,0xbf
  LCD_PRINT 0,0,d'16',0xA0,0xa1,0xa2,0xa3,0xa4,0xa5,0xa6,0xa7,0xa8,0xa9,0xaa,0xab,0xac,0xad,0xae,0xaf
  goto $
;----------------
INT_FUNC			CODE
interrupt			; обработчик прерываний
  SaveContext

intexit
  RestContext
  retfie
;----------------
  end