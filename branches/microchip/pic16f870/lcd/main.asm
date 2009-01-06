  list p=16f870
  #include "p16f870.inc"
  #include "config.inc"
  #include "lcd_WH1602D.asm"
  #include "ADC.asm"
;------------------
jump_nz macro aLabel
  btfss STATUS,Z
  goto aLabel
  endm
;------------------
jump_z macro aLabel
  btfsc STATUS,Z
  goto aLabel
  endm;------------------
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
ADC_complete			equ 0
ADC_changed				equ 1
;------------------
SharedData				UDATA_SHR
WBuf					res 1			; сохр W
StatBuf					res 1			; сохр STATUS
PB_buf					res 1
ADC_flag				res 1
PrevADRES				res 2
;  GLOBAL PB_buf

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
  call InitADC
  bsf INTCON,GIE
mainloop
  btfss ADC_flag,ADC_changed
  goto mainnext1
  bcf ADC_flag,ADC_changed
  LCD_PRINT_HEX_2W 5,1,ADRESH,ADRESL,1
mainnext1
  btfss ADC_flag,ADC_complete
  goto mainloop
  bcf ADC_flag,ADC_complete
  PauseBeforeGo
  GoADC
  goto mainloop
;----------------
INT_FUNC			CODE
interrupt			; обработчик прерываний
  SaveContext

  banksel PIE1
  btfss PIE1,ADIE
  goto intnext1
  banksel PIR1
  btfss PIR1,ADIF
  goto intnext1
  bcf PIR1,ADIF
  bsf ADC_flag,ADC_complete  
;  banksel ADRESL
;  movf ADRESL,W				; проверяем предыдущее значение АЦП
;  xorwf PrevADRES,F
;  jump_z nextADC_check
;  movwf PrevADRES
;  bsf ADC_flag,ADC_changed
;  goto intnext1
;nextADC_check
;  banksel ADRESH
;  movf ADRESH,W
;  xorwf PrevADRES+1,F
;  jump_z intnext1
;  movwf PrevADRES+1
  bsf ADC_flag,ADC_changed

intnext1
intexit
  RestContext
  retfie
;----------------
  end