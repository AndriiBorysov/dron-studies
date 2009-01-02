ScanKeys
; SCAN--
  ; получаем значения с ног
  clrf TempKEY
  banksel PORTB
  movlw b'11111011'
  movwf BMask
  movwf PORTB		; B2, w = 1111 1011
  nop
  movf PORTB,W
  movf PORTB,W
  movwf Temp
  btfss Temp,4
  bsf TempKEY,KEY_2
  btfss Temp,5
  bsf TempKEY,KEY_5

  rrf BMask,F
  movf BMask,W
  movwf PORTB		; B1, w = 1111 1101
  nop
  movf PORTB,W
  movf PORTB,W
  movwf Temp
  btfss Temp,4
  bsf TempKEY,KEY_3
  btfss Temp,5
  bsf TempKEY,KEY_4

  rrf BMask,F
  movf BMask,W
  movwf PORTB		; B0, w = 1111 1110
  nop
  movf PORTB,W
  movf PORTB,W
  movwf Temp
  btfss Temp,4
  bsf TempKEY,KEY_1
  btfss Temp,5
  bsf TempKEY,KEY_6
  ; в TempKEY текущее состояние кнопок
; SCAN--
  ; обработка флагов и кнопок
  movf KEYState,W	; сохраняем KEYState для последующего использования
  movwf PrevKS		;         /

  ; KEYState = (TempKEY & DeBNC) | (KEYState & ~DeBNC)
  movf DeBNC,W
  xorlw 0xFF		; ~DeBNC
  andwf PrevKS,W		; KEYState & ~DeBNC
  movwf Temp
  movf DeBNC,W		; DeBNC
  andwf TempKEY,W	; TempKEY & DeBNC
  iorwf Temp,W		; (TempKEY & DeBNC) | (KEY & ~DeBNC)
  movwf KEYState			; KEYState = ...

  ; DeBNC = ((~KEYState & TempKEY) | (KEYState & ~TempKEY)) & ~DeBNC
  ; KEYNum = ((~KEYState & TempKEY) | (KEYState & ~TempKEY)) & DeBNC
  movf PrevKS,W
  xorlw 0xFF		; ~KEYState
  andwf TempKEY,W	; ~KEYState & TempKEY
  movwf Temp
  movf TempKEY,W
  xorlw 0xFF		; ~TempKEY
  andwf PrevKS,W
  iorwf Temp,F		; (~KEYState & TempKEY) | (KEYState & ~TempKEY)

  movf DeBNC,W
  andwf Temp,W		; ((~KEYState & TempKEY) | (KEYState & ~TempKEY)) & DeBNC
  movwf KEYNum		; KEYNum = ...
  ; обработка флага нажатия/отжатия
  ; !!!!!!!!! перед изменением DeBNC
  bcf KEYNum,KEY_HIT
  bcf KEYNum,KEY_REL
  ; key pressed
  movf PrevKS,W
  xorlw 0xFF
  andwf TempKEY,W
  andwf DeBNC,W
  btfss STATUS,Z
  bsf KEYNum,KEY_HIT
  ; key released
  movf TempKEY,W
  xorlw 0xFF
  andwf PrevKS,W
  andwf DeBNC,W
  btfss STATUS,Z
  bsf KEYNum,KEY_REL
; !!!!!!!!! изменение DeBNC
  movf DeBNC,W
  xorlw 0xFF		; ~DeBNC
  andwf Temp,W		; ((~KEYState & TempKEY) | (KEYState & ~TempKEY)) & ~DeBNC
  movwf DeBNC		; DeBNC = ...
  return