;---------------------------------------------------------
; ���������� ������� ��� ������ � WH1602D
;---------------------------------------------------------
  IFNDEF __LCD_WH1602D_ASM__
  #define __LCD_WH1602D_ASM__
;  #include "p16f870.inc"

LCD_WH1602D_SharedData	UDATA_SHR
LcdData					res 1

LCD_WH1602D_Code		CODE
;----------------
; �������� ������ CALL
GetHexTable
  andlw h'0F'			; ������� ��������� ������ �� �������� => 16
  addwf PCL,F
  retlw '0'
  retlw '1'
  retlw '2'
  retlw '3'
  retlw '4'
  retlw '5'
  retlw '6'
  retlw '7'
  retlw '8'
  retlw '9'
  retlw 'A'
  retlw 'B'
  retlw 'C'
  retlw 'D'
  retlw 'E'
  retlw 'F'
;----------------
; ������� �������� � hex �� ����� ��� ������
; aFlagFile = 0 -> aC1,aC2 - �����
LCD_PRINT_HEX_2W macro aX,aY,aC1,aC2,aFlagFile
  if (aX + 4) > 15
    error "lcd cursor is out of bounds in hex print"
  endif
  LCD_COMMAND_GOTOXY aX,aY
  local i = 1
  while i <= 2
  WAIT_BF
  SetData
  if aFlagFile == 0
  movlw aC#v(i)
  else
  banksel aC#v(i)
  movf aC#v(i),W
  endif
  movwf LcdData
  swapf LcdData,W
  andlw h'0F'
  call GetHexTable
  movwf LcdData
  call LCDWrite
  WAIT_BF
  SetData
  if aFlagFile == 0
  movlw aC#v(i)
  else
  banksel aC#v(i)
  movf aC#v(i),W
  endif
  andlw h'0F'
  call GetHexTable
  movwf LcdData
  call LCDWrite
i++
  endw
  endm
;----------------
; ������ ��������� ����� �� I � J
PauseIJ macro delI,delJ
  banksel DelayI
  movlw delJ
  movwf DelayJ
  movlw delI
  movwf DelayI
  call Delay
  endm
;----------------
; ��������� ������� � ������� 0
LCD_COMMAND_HOME macro
  WAIT_BF
  LCD_OUTPUT b'00000010',0
  endm
;----------------
; ��������� ������ �����������
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
; ��������� ����������������
; ����, ���-�� ����� � �����
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
; ������� �������
LCD_COMMAND_DISP_CLEAR macro
  WAIT_BF
  LCD_OUTPUT b'00000001',0
  endm
;----------------
; ����������� �������� � ��������
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
; ��������� �������/�������� ������ DDRAM
LCD_COMMAND_GOTOXY macro aX, aY
  if (aX < 0 && aX > 15) || (aY < 0 && aY > 1)
    error "lcd cursor is out of bounds"
  endif
  WAIT_BF
  LCD_OUTPUT ((aY*h'40' + aX)|0x80),0
  endm
;----------------
LCD_PRINT macro aX, aY, aCount, aC1, aC2, aC3, aC4, aC5, aC6, aC7, aC8, aC9, aC10, aC11, aC12, aC13, aC14, aC15, aC16
  if aCount <= 0 || (aCount + aX) > d'16'
    error "Bad count for printing string"
  endif
  LCD_COMMAND_GOTOXY aX, aY
  local i = 1
  while i <= aCount
  WAIT_BF
  LCD_OUTPUT aC#v(i),1
i++
  endw
  endm
;----------------
; ��������� ������ ������� ������/������� � ����
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
; ������ ��������� ����� ������ �� ����� (PORTB)
SetDataPORT_OUT macro
  movlw b'11110000'		; PORTB<3:0> OUT
  banksel TRISB
  andwf TRISB,F
  endm
;----------------
; ������ ��������� ����� ������ �� ���� (PORTB)
SetDataPORT_IN macro
  movlw b'00001111'		;PORTB<3:0> IN
  banksel TRISB
  iorwf TRISB,F
  endm
;----------------
; ����� � LCD �������� info
; rs ������ �������� ����� RS,
; � ������� ���������� ������ ������
LCD_OUTPUT macro info, rs
  movlw info
  movwf LcdData
  if rs == 0
    SetCommand
  else
    SetData
  endif
  call LCDWrite
  endm
;----------------
; ������ �������� ������������ ����� ���������
WAIT_BF macro
  call GetBusyFlag
  btfsc STATUS,C
  goto $-2				; retest BF
  endm
;----------------
; ������� �������� �������� �� ������������ � LCD
; �� 4���� ����
; LcdData --> LCD
LCDWrite
  SetDataPORT_OUT		; ��������� ����� ������ �� �����
  movlw b'11110000'		; PB_buf = xxxx0000
  andwf PB_buf,F		;    /
  swapf LcdData,W		; W = D 3210 7654
  andlw b'00001111'		; W = D ---- 7654
  iorwf PB_buf,W
  banksel PORTC

  bcf PORTC,4			; R/~W = 0
  nop					; address setup time

  movwf PORTB			; PORTB = D xxxx7654
  DoSendCommand

  movlw b'11110000'		; PB_buf = xxxx0000
  andwf PB_buf,F			;    /

  movf LcdData,W		; W = D 7654 3210
  andlw b'00001111'		; W = D ---- 3210
  iorwf PB_buf,W

  movwf PORTB			; PORTB = D xxxx3210
  DoSendCommand

  bcf PORTC,4			; R/~W = 0
  return
;----------------
; ������� ������ ����� ������ �� LCD
; � ����������� �� 4���� ����
; return LcdData
LCDRead
  SetDataPORT_IN		; ��������� ����� ������ �� ����
  banksel PORTC
  bsf PORTC,4			; R/~W = 1
  nop					; address setup time
  bsf PORTC,5			; E = 1
  nop					; data delay time
  movf PORTB,W			; W = D xxxx7654
  andlw b'00001111'		; DATA = D ----7654
  movwf LcdData
  bcf PORTC,5			; E = 0
  bsf PORTC,5			; E = 1
  swapf LcdData,F		; DATA = D 7654----
  movf PORTB,W			; W = D xxxx3210
  bcf PORTC,5			; E = 0
  andlw b'00001111'		; W = D ----3210
  iorwf LcdData,W		; W = 76543210 - result
  bcf PORTC,4			; R/~W = 0
  return
;----------------
; W destroying
; return BF --> STATUS.C
GetBusyFlag
  SetCommand
  call LCDRead		; ������ ���� ���� ����������
  rlf LcdData,W		; D7 --> STATUS.C
  return
;----------------
; ������������� � ������ ������� ��� LCD
InitLCD
  clrf PB_buf
  SetDataPORT_OUT		; ��������� ����� ������ �� �����
  movlw b'11000111'		; c5-3 out
  andwf TRISC,F
  banksel PORTB
  clrf PORTB
  clrf PORTC
; ������������� ��� 4������� ����������
  PauseIJ d'60',d'250'		; ~15ms
  bsf PORTB,0
  bsf PORTB,1			; PORTB = xxxx0011
; �� ���� ����:
; PORTC = 0000 0000
; PORTB = 0000 0011
  DoSendCommand
  PauseIJ d'20',d'200'		; ~4.1ms
  DoSendCommand
  PauseIJ d'4', d'22'		; ~100us
  DoSendCommand
; ����� ������������� ��� 4������� ����������
; ��������� 4�, ����� �����, ������
  movlw b'11110010'		; com1 = 00 0010
  andwf PB_buf,F
  bsf PB_buf,1
  movf PB_buf,W
  movwf PORTB
  DoSendCommand

  LCD_COMMAND_FUNC_SET 0,1,0			; 4 ����, 2 ������, 5*8 �����
  LCD_COMMAND_DISP_CONTROL 0, 0, 0		; Display OFF
  LCD_COMMAND_DISP_CLEAR				; Display clear
  LCD_COMMAND_ENTRY_MODE 1, 0			; Entry mode: AC++, Shift OFF
  LCD_COMMAND_DISP_CONTROL 1, 0, 0		; Display, UCur, BCur
;  LCD_COMMAND_HOME
  return
;----------------
; ����������� "��������" �����
; ~ i*(4*j + 3) cycles
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
;  GLOBAL InitLCD
  
  
  ENDIF