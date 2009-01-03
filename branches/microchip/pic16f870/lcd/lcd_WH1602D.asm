;---------------------------------------------------------
; определены функции для работы с WH1602D
;---------------------------------------------------------
  #include "p16f870.inc"
  EXTERN TempB

LCD_WH1602D_SharedData	UDATA_SHR
LcdDataOUT				res 1
LcdTemp					res 1		; хлам

LCD_WH1602D_Data		UDATA_SHR h'0020'
DelayI					res 1
DelayJ					res 1

LCD_WH1602D_Code		CODE
;----------------
; установка курсора в позицию 0
LCD_COMMAND_HOME macro
  WAIT_BF
  LCD_OUTPUT b'00000010',0
  endm
;----------------
; установка режима содержимого
LCD_COMMAND_ENTRY_MODE macro aMoveDir, aDispEnable
  local disp = b'00000100'
  if aMoveDir
disp |= b'00000010'
  endif
  if aDispEnable
disp |= b'00000001'
  endif
  WAIT_BF
  LCD_OUTPUT disp,0
  endm
;----------------
; установка функциональности
; шина, кол-во линий и шрифт
LCD_COMMAND_FUNC_SET macro aDataWidth, aLinesNum, aFont
  local disp = b'00100000'
  if aDataWidth
disp |= b'00010000'
  endif
  if aLinesNum
disp |= b'00001000'
  endif
  if aFont
disp |= b'00000100'
  endif
  WAIT_BF
  LCD_OUTPUT disp,0
  endm
;----------------
; очистка дисплея
LCD_COMMAND_DISP_CLEAR macro
  WAIT_BF
  LCD_OUTPUT b'00000001',0
  endm
;----------------
; усправление дисплеем и курсором
LCD_COMMAND_DISP_CONTROL macro aDispOn, aCurOn, aBlinkOn
  local disp = b'00001000'
  if aDispOn
disp |= b'00000100'
  endif
  if aCurOn
disp |= b'00000010'
  endif
  if aBlinkOn
disp |= b'00000001'
  endif
  WAIT_BF
  LCD_OUTPUT disp,0		; Display OFF
  endm
;----------------
; выполняет чтение экраном данных/команды с шины
DoSendCommand macro
  bsf PORTC,5			; E = 1
  bcf PORTC,5			; E = 0
  endm
;----------------
SetCommand macro
  bcf PORTC,3		; RS = 0
  endm
;----------------
SetData macro
  bsf PORTC,3		; RS = 1
  endm
;----------------
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
PauseIJ macro delI,delJ
  banksel DelayI
  movlw delJ
  movwf DelayJ
  movlw delI
  movwf DelayI
  call Delay
  endm
;----------------
; вывод в LCD значения info
; rs задает значение линии RS,
; с которым необходимо подать данные
LCD_OUTPUT macro info, rs
  movlw info
  movwf LcdDataOUT
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
  movf LcdDataOUT,W
  movwf LcdTemp			; LcdTemp = DATA
  movlw b'11110000'		; TempB = xxxx0000
  andwf TempB,F			;    /
  swapf LcdTemp,W		; W = D 3210 7654
  andlw b'00001111'		; W = D ---- 7654
  iorwf TempB,W
  banksel PORTC

  bcf PORTC,4			; R/~W = 0
  nop					; address setup time

  movwf PORTB			; PORTB = D xxxx7654
  DoSendCommand

  movlw b'11110000'		; TempB = xxxx0000
  andwf TempB,F			;    /

  movf LcdTemp,W		; W = D 7654 3210
  andlw b'00001111'		; W = D ---- 3210
  iorwf TempB,W

  movwf PORTB			; PORTB = D xxxx3210
  DoSendCommand

  bcf PORTC,4			; R/~W = 0
  return
;----------------
; функция чтения байта данных из LCD
; в аккумулятор по 4хбит шине
; STATUS.C --> RS
; return W
LCDRead
  SetDataPORT_IN		; установка порта данных на ввод
  clrf LcdTemp
  banksel PORTC
  bsf PORTC,4			; R/~W = 1
  nop					; address setup time
  bsf PORTC,5			; E = 1
  nop					; data delay time
  movf PORTB,W			; W = D xxxx7654
  andlw b'00001111'		; W = D ----7654
  movwf LcdTemp
  bcf PORTC,5			; E = 0
  bsf PORTC,5			; E = 1
  swapf LcdTemp,F		; LcdTemp = D 7654xxxx
  movf PORTB,W			; W = D xxxx3210
  bcf PORTC,5			; E = 0
  andlw b'00001111'		; W = D ----3210
  iorwf LcdTemp,W		; W = 76543210 - result
  bcf PORTC,4			; R/~W = 0
  return
;----------------
; W destroying
; return BF --> STATUS.C
GetBusyFlag
  SetCommand
  call LCDRead		; читаем весь байт информации
  movwf LcdTemp
  rlf LcdTemp,W		; D7 --> STATUS.C
  return
;----------------
; инициализация и первые команды для LCD
InitLCD
  GLOBAL InitLCD
  clrf TempB
  SetDataPORT_OUT		; установка порта данных на вывод
  movlw b'11000111'		; c5-3 out
  andwf TRISC,F
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
  DoSendCommand
  PauseIJ d'20',d'200'		; ~4.1ms
  DoSendCommand
  PauseIJ d'4', d'22'		; ~100us
  DoSendCommand
; конец инициализации для 4битного интерфейса
;--------------------
; установка 4б, числа строк, шрифта
  movlw b'11110010'		; com1 = 00 0010
  andwf TempB,F
  bsf TempB,1
  movf TempB,W
  movwf PORTB
  DoSendCommand

  LCD_COMMAND_FUNC_SET 0,1,0			; 4 бита, 2 строки, 5*8 точек
  LCD_COMMAND_DISP_CONTROL 0, 0, 0		; Display OFF
  LCD_COMMAND_DISP_CLEAR				; Display clear
  LCD_COMMAND_ENTRY_MODE 1, 0			; Entry mode: AC++, Shift OFF
  LCD_COMMAND_DISP_CONTROL 1, 1, 1		; Display ON, UCur ON, BCur ON
  LCD_COMMAND_HOME
  return
;----------------
; организация "активной" паузы
Delay
  banksel DelayI
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

  END