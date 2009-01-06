;---------------------------------------------------------
; ���������� ������� ��� ������ � ���
;---------------------------------------------------------
  IFNDEF __ADC_ASM__
  #define __ADC_ASM__
;  #include "p16f870.inc"
ADC_CODE			CODE
;-----------------
; ��������� �������� ����� ����� �������
PauseBeforeGo macro
  PauseIJ d'2', d'25'		; ~51us
  endm
;-----------------
; �������� ��������������
GoADC macro
  banksel ADCON0
  bsf ADCON0,ADON
  bsf ADCON0,GO_DONE
  endm
;-----------------
; ������������� � �����
InitADC
  banksel ADCON1
  clrf ADCON1
  movlw b'10000100'			; Right just., 3/0
  iorwf ADCON1,F
  banksel ADCON0
  clrf ADCON0
  movlw b'10001001'			; F/32, Ch1, ADON
  iorwf ADCON0,F
  banksel PIE1
  bsf PIE1,ADIE
  banksel PIR1
  bcf PIR1,ADIF
  bsf INTCON,PEIE
  PauseBeforeGo
  GoADC
  return


  ENDIF
