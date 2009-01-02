  list p=16f870
  #include "p16f870.inc"
  #include "config.inc"
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
;ccp1H equ h'4E'
ccp1L equ h'0'
pr2v equ d'150'

tmr1Lv equ h'BF'
tmr1Hv equ h'63'

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

KeysNeedService equ 0x7A

key2 equ 0x7B

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
loop
  btfss KeysNeedService, 0
  goto loop
  bcf KeysNeedService, 0
  call ScanKeys
; ��������� ������� ������
  btfss KEYState,KEY_3
  goto hit
  banksel CCPR1L
  btfss KeysNeedService, 1
  goto k3else				; -> --
  incf CCPR1L,W				; ++
  btfss STATUS,Z
  goto toggle1
  bcf KeysNeedService, 1	; --
  decf CCPR1L,F
  goto hit
toggle1
  incf CCPR1L,F
  goto hit
k3else						; --
  decf CCPR1L,W
  btfss STATUS,Z
  goto toggle2
  bsf KeysNeedService, 1	; ++
  incf CCPR1L,F
  goto hit
toggle2
  decf CCPR1L,F

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
;  movlw 0x04
;  iorwf TempC,F
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
  goto loop

  btfss KEYNum,KEY_2
  goto relnext2
  bsf key2,1
;  movlw ~0x01
;  andwf TempC,F
relnext2
;  btfss KEYNum,KEY_3
;  goto relnext3
;  movlw ~0x04
;  andwf TempC,F
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
  clrf KeysNeedService

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
; ��������� ������� 1
  banksel TMR1L
;  clrf TMR1L
;  clrf TMR1H
  movlw tmr1Lv
  movwf TMR1L
  movlw tmr1Hv
  movwf TMR1H
;  bsf T1CON,T1CKPS0	; pre = 1:2
  banksel PIE1
  bsf PIE1, TMR1IE
  banksel T1CON
  bsf T1CON,TMR1ON	; ������ �������

; ��������� ������� 2
  banksel CCPR1L
  movlw ccp1L
  movwf CCPR1L

  banksel PR2
  movlw pr2v
  movwf PR2
  banksel PIE1
  bsf PIE1, TMR1IE
  banksel TMR2
  clrf TMR2
  banksel T2CON
  bsf T2CON, TMR2ON		; ������ �������
  bsf T2CON, T2CKPS1	; pre = 1:16

; ��������� CCP
  movlw b'00001111'	; PWM mode
  movwf CCP1CON
  banksel PIE1
  bsf PIE1,CCP1IE

  bsf INTCON,PEIE	; ������. ���������
  return
;----------------
interrupt			; ���������� ����������
  SaveContext

  banksel PIR1
  btfss PIR1,TMR1IF	; ���������� �� ������� 1 ?
  goto nextint1		; ���
  bcf PIR1, TMR1IF	; ��, ������
  movlw tmr1Lv
  movwf TMR1L
  movlw tmr1Hv
  movwf TMR1H
  bsf KeysNeedService, 0

nextint1
;  btfss PIR1,CCP1IF	; ���������� ccp1 ?
;  goto nextint2		; ���
;  bcf PIR1,CCP1IF	; ��, ������ � ������������� �����
;  movlw ccp1L
;  addwf CCPR1L,F
;  btfsc STATUS,C
;  incf CCPR1H,F
;  movlw ccp1H
;  addwf CCPR1H,F	; /

nextint2
intexit
  RestContext
  retfie
;------------
  #include "keypad.asm"		; ���������������� ����������
;------------
  end