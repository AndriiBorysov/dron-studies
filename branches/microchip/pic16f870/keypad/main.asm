  list p=16f870
  #include "p16f870.inc"
  __config h'3F3A'

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
;ccp1L equ h'40'			; 20ms pre=1:2
;ccp1H equ h'9C'			;	/
ccp1L equ h'20'			; 10ms pre=1:2
ccp1H equ h'4E'			;	/

WBuf equ 0x70			; ���� W
StatBuf equ 0x71		; ���� STATUS

KEYNum equ 0x72			; ������ ������
DeBNC equ 0x73			; ������ �� �������
BMask equ 0x74
TempKEY equ 0x75		; ������� ��������� ������
Temp equ 0x76			; ����
TempC equ 0x77			; �������� �������� PORTC
KEYState equ 0x78		; ��������� ������
PrevKS equ 0x79			; ���������� ��������� ������

key2 equ 0x7A

KEY_HIT equ 0			; ����� �������, � KEYNum
KEY_1 equ 1
KEY_2 equ 2
KEY_3 equ 3
KEY_4 equ 4
KEY_5 equ 5
KEY_6 equ 6
KEY_REL equ 7			; ����� �������, � KEYNum
;------------------
  org h'00'
  nop
  org h'01'
  goto main

  org h'04'
  goto interrupt	; ��������� ����������
;---------------
main
  call InitLights	; ��������� �����������
  call Init_TMR1_CCP	; ��������� ������� � CCP
  call InitKey		; ��������� ����������
  bsf INTCON,GIE	; ��������� ����������
loop				;
; ��������� ������� ������
hit
  btfss KEYNum,KEY_HIT	; ������ ?
;  goto hit			; ���
  goto rel
; �� ������ ������� ��������� �������. ��������
; 1 - ����� ���			C=11uuu1u1= |0xC5
; 2 - ����. �����		C=uuuuuuu1= |0x01
; 3 - ��. ����. �����	C=uuuuu1uu= |0x04
; 4 - �����. �����		C=u1uuuuuu= |0x40
; 5 - ��. ���. �����	C=1uuuuuuu= |0x80
; 6 - ������ ���		C=00uuu0u0= &0x3A

  btfss KEYNum,KEY_1
  goto hitnext1
  movlw 0xC5
  iorwf TempC,F
hitnext1
  btfss KEYNum,KEY_2
  goto hitnext2
  btfss key2,1			; ���� ��������
  goto hitnext2			; ���
  bcf key2,1
  btfss key2,0			; ��������� �������
  goto light			; ������
dark
  movlw ~0x01
  andwf TempC,F
  bcf key2,0
  goto hitnext2
light
  movlw 0x01
  iorwf TempC,F
  bsf key2,0
hitnext2
  btfss KEYNum,KEY_3
  goto hitnext3
  movlw 0x04
  iorwf TempC,F
hitnext3
  btfss KEYNum,KEY_4
  goto hitnext4
  movlw 0x40
  iorwf TempC,F
hitnext4
  btfss KEYNum,KEY_5
  goto hitnext5
  movlw 0x80
  iorwf TempC,F
hitnext5
  btfss KEYNum,KEY_6
  goto hitnext6
  movlw 0x3A
  andwf TempC,F
hitnext6
  movf TempC,W		; ������� ��������� � ���� �
  movwf PORTC
; ��������� ������� ������
rel
  btfss KEYNum,KEY_REL	; ������ ?
;  goto rel
  goto hit

  btfss KEYNum,KEY_2
  goto relnext2
  bsf key2,1
;  movlw ~0x01
;  andwf TempC,F
relnext2
  btfss KEYNum,KEY_3
  goto relnext3
  movlw ~0x04
  andwf TempC,F
relnext3
  btfss KEYNum,KEY_4
  goto relnext4
  movlw ~0x40
  andwf TempC,F
relnext4
  btfss KEYNum,KEY_5
  goto relnext5
  movlw ~0x80
  andwf TempC,F
relnext5

  movf TempC,W		; ������� ��������� � ���� �
  movwf PORTC

  goto loop
;----------------
InitKey
  clrf DeBNC
  clrf KEYState
  clrf KEYNum

  banksel TRISB
  movlw b'11111000'	; RB2:0 - out
  movwf TRISB
  bcf OPTION_REG,NOT_RBPU	; enable pull-ups
  banksel PORTB
  clrf PORTB		; ����� �����
  movf PORTB,W		;   /
  return
;----------------
InitLights			; ��������� �����������
  banksel PORTC
  clrf PORTC		; PORTC = 0
  clrf TempC
  banksel TRISC
  movlw 0x3A		; C0,C2,C6,C7 - out
  andwf TRISC,F
  return
;----------------
Init_TMR1_CCP
; ��������� �������
  banksel TMR1L
  clrf TMR1L
  clrf TMR1H
  bsf T1CON,T1CKPS0	; pre = 1:2
  bsf T1CON,TMR1ON	; ������ �������
; ��������� CCP
  banksel CCPR1L
  movlw ccp1L		; ��������� ���� CCP H:L
  movwf CCPR1L
  movlw ccp1H
  movwf CCPR1H
  movlw b'00001010'	; Compare, generate soft int
  movwf CCP1CON
  banksel PIE1
  bsf PIE1,CCP1IE

  bsf INTCON,PEIE	; ������. ���������
  return
;----------------
interrupt			; ���������� ����������
  SaveContext
  banksel PIR1
  btfss PIR1,CCP1IF	; ���������� ccp1 ?
  goto intexit		; ���
  bcf PIR1,CCP1IF	; ��, ������ � ������������� �����
  movlw ccp1L
  addwf CCPR1L,F
  btfsc STATUS,C
  incf CCPR1H,F
  movlw ccp1H
  addwf CCPR1H,F	; /

; SCAN--
  ; �������� �������� � ���
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
  ; � TempKEY ������� ��������� ������
; SCAN--
  ; ��������� ������ � ������
  movf KEYState,W	; ��������� KEYState ��� ������������ �������������
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
  ; ��������� ����� �������/�������
  ; !!!!!!!!! ����� ���������� DeBNC
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
; !!!!!!!!! ��������� DeBNC
  movf DeBNC,W
  xorlw 0xFF		; ~DeBNC
  andwf Temp,W		; ((~KEYState & TempKEY) | (KEYState & ~TempKEY)) & ~DeBNC
  movwf DeBNC		; DeBNC = ...


intexit
  RestContext
  retfie

  end