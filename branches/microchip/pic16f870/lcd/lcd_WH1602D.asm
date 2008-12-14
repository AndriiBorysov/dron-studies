; определены функции для работы с WH1602D
;----------------
SetCommand macro
  bcf PORTC,3		; RS = 0
  endm
SetData macro
  bsf PORTC,3		; RS = 1
  endm
; макрос установки порта данных на ВЫВОД (PORTB)
SetDataPORT_OUT macro
  movlw b'11110000'		; PORTB<3:0> OUT
  banksel TRISB
  andwf TRISB,F
  endm
;----------------
; макрос установки порта данных на ВВОД (PORTB)
SetDataPORT_IN macro
  movlw b'00001111'		;PORTB<3:0> IN
  banksel TRISB
  iorwf TRISB,F
  endm
;----------------
; макрос настройки паузы на I и J
PauseIJ macro dI,dJ
  movlw dJ
  movwf DelayJ
  movlw dI
  movwf DelayI
  call Delay
  endm
;----------------
; вывод в LCD значения info
; rs задает значение линии RS,
; с которым необходимо подать данные
LCD_OUTPUT macro info, rs
  movlw info
  movwf DataOUT
  if rs == 0
    SetCommand
  else
    SetData
  endif
  call LCDWrite
  endm
;----------------
; макрос ожидания освобождения флага занятости
WAIT_BF macro
  call GetBusyFlag
  btfsc STATUS,C
  goto $-2				; retest BF
  endm
;----------------
; функция передачи значения из аккумулятора в LCD
; по 4хбит шине
; STATUS.C --> RS
; W --> LCD
LCDWrite
  SetDataPORT_OUT		; установка порта данных на вывод
  movf DataOUT,W
  movwf Temp			; Temp = DATA
  movlw b'11110000'		; TempB = xxxx0000
  andwf TempB,F			;    /
  swapf Temp,W			; W = D 3210 7654
  andlw b'00001111'		; W = D ---- 7654
  iorwf TempB,W
  banksel PORTC
  ;bcf PORTC,3			; RS = 0
  ;btfsc STATUS,C		; if STATUS.C == 1
  ;bsf PORTC,3			; then RS = 1

  bcf PORTC,4			; R/~W = 0
  nop					; address setup time

  movwf PORTB			; PORTB = D xxxx7654
  bsf PORTC,5			; E = 1
  nop					; data setup time
  bcf PORTC,5			; E = 0

  movlw b'11110000'		; TempB = xxxx0000
  andwf TempB,F			;    /

  movf Temp,W			; W = D 7654 3210
  andlw b'00001111'		; W = D ---- 3210
  iorwf TempB,W

  movwf PORTB			; PORTB = D xxxx3210
  bsf PORTC,5			; E = 1
  nop					; data setup time
  bcf PORTC,5			; E = 0

  bcf PORTC,4			; R/~W = 0
  return
;----------------
; функция чтения байта данных из LCD
; в аккумулятор по 4хбит шине
; STATUS.C --> RS
; return W
LCDRead
  SetDataPORT_IN		; установка порта данных на ввод
  clrf Temp
  banksel PORTC
  ;bcf PORTC,3			; RS = 0
  ;btfsc STATUS,C		; if STATUS.C == 1
  ;bsf PORTC,3			; then RS = 1
  bsf PORTC,4			; R/~W = 1
  nop					; address setup time
  bsf PORTC,5			; E = 1
  nop					; data delay time
  movf PORTB,W			; W = D xxxx7654
  andlw b'00001111'		; W = D ----7654
  movwf Temp
  bcf PORTC,5			; E = 0
  bsf PORTC,5			; E = 1
  swapf Temp,F			; Temp = D 7654xxxx
  movf PORTB,W			; W = D xxxx3210
  bcf PORTC,5			; E = 0
  andlw b'00001111'		; W = D ----3210
  iorwf Temp,W			; W = 76543210 - result
  bcf PORTC,4			; R/~W = 0
  return
;----------------
; W destroying
; return BF --> STATUS.C
GetBusyFlag
  ;bcf STATUS,C		; RS = 0
  SetCommand
  call LCDRead		; читаем весь байт информации
  movwf Temp
  rlf Temp,W		; D7 --> STATUS.C
  return
;----------------
; инициализация и первые команды для LCD
InitLCD
  clrf TempC
  clrf TempB

  SetDataPORT_OUT		; установка порта данных на вывод
  ;andwf TempB,F			; b3-0 = 0

  movlw b'11000111'		; c5-3 out
  andwf TRISC,F
  ;andwf TempC,F			; c5-3 = 0

  banksel PORTB
  clrf PORTB 
  clrf PORTC
;--------------------
; инициализация для 4битного интерфейса
  PauseIJ d'60',d'250'		; ~15ms
  bsf PORTB,0
  bsf PORTB,1			; PORTB = xxxx0011
; на этом шаге:
; PORTC = 0000 0000
; PORTB = 0000 0011
  bsf PORTC,5			; E = 1
  nop
  bcf PORTC,5			; E = 0

  PauseIJ d'20',d'200'		; ~4.1ms
  bsf PORTC,5		; E = 1
  nop
  bcf PORTC,5		; E = 0

  PauseIJ d'4', d'22'		; ~100us
  bsf PORTC,5		; E = 1
  nop
  bcf PORTC,5		; E = 0
; конец инициализации для 4битного интерфейса
;--------------------
; установка 4б, числа строк, шрифта
  movlw b'11110010'		; com1 = 00 0010
  andwf TempB,F
  bsf TempB,1
  movf TempB,W
  movwf PORTB
  bsf PORTC,5		; E = 1
  nop
  nop
  nop
  bcf PORTC,5		; E = 0
  bsf PORTC,5		; E = 1
  nop
  nop
  nop
  bcf PORTC,5		; E = 0

  WAIT_BF
  LCD_OUTPUT b'00101000',0		; 2 строки, 5*8 точек
  WAIT_BF
  LCD_OUTPUT b'00001000',0		; Display OFF
  WAIT_BF
  LCD_OUTPUT b'00000001',0		; Display clear
  WAIT_BF
  LCD_OUTPUT b'00000110',0		; Entry mode: AC++, Shift OFF
  WAIT_BF
  LCD_OUTPUT b'00001111',0		; Display ON, UCur ON, BCur ON
  WAIT_BF
  LCD_OUTPUT b'00000010',0
  WAIT_BF

  variable i
i=0
  while i<10
  WAIT_BF
  LCD_OUTPUT 0x30+i,1
i+=1
  endw


;  bcf STATUS,C		; RS = 0
;  call LCDRead		; читаем весь байт информации
;  nop
;  WAIT_BF
;  LCD_OUTPUT b'10000000',0
;  WAIT_BF
;  bcf STATUS,C		; RS = 0
;  call LCDRead		; читаем весь байт информации
;  nop
;  WAIT_BF
;  LCD_OUTPUT b'10000001',0
;  WAIT_BF
;  bcf STATUS,C		; RS = 0
;  call LCDRead		; читаем весь байт информации
;  nop
;  WAIT_BF
;  LCD_OUTPUT b'10000010',0
;  WAIT_BF
;  bcf STATUS,C		; RS = 0
;  call LCDRead		; читаем весь байт информации
;  nop
;  WAIT_BF
;  LCD_OUTPUT h'FF',1

  nop
  return
;----------------
Delay
  movf DelayJ,W
delay1
  movwf DelayJ
delay2
  nop
  decfsz DelayJ,F
  goto delay2
  decfsz DelayI,F
  goto delay1
  return
