#include <htc.h>
//#include <pic.h>
#include "lcd.h"


void LCD_WriteByte()
{
	TRISB &= 0xF0;		// ��������� ����� ������ �� �����
	PB_buf &= 0xF0;
	PB_buf = ((PB_buf & 0xF0) | ((LcdData >> 4)&0x0F));
	LCD_RW = 0;
	asm("nop");
	PORTB = PB_buf;
	LCD_STROBE();
	PB_buf = ((PB_buf & 0xF0) | (LcdData&0x0F));
	PORTB = PB_buf;
	LCD_STROBE();
}
void LCD_ReadByte()
{
	TRISB |= 0x0F;
	LCD_RW = 1;
	asm("nop");
	LCD_EN = 1;
	asm("nop");
	LcdData  = (PORTB & 0x0F);		//DATA = D ----7654
	LCD_EN = 0;
	LCD_EN = 1;
	LcdData <<= 4;
	LcdData &= 0xF0;
	LcdData |= (PORTB & 0x0F);
//	LcdData = (((LcdData << 4) & 0xF0) | (PORTB & 0x0F));
	LCD_EN = 0;
	LCD_RW = 0;
}

void LCD_WaitBusyFlag(void)
{
	do
	{
		__delay_us(40);
		LCD_RS = 0;			// SetCommand
		LCD_ReadByte();
		asm("rlf _LcdData,W");
	}while(CARRY);
}

void InitLCD(void)
{
	PB_buf = 0;
	TRISB &= 0xF0;		// PORTB<3:0> OUT
	TRISC &= 0xC7;
	PORTB = 0;
	PORTC = 0;
// ������������� ��� 4������� ����������
	__delay_us(15000);					// 15ms
	PB_buf = ((PB_buf & 0xF0)|0x03);
	PORTB = PB_buf;					// PORTB = xxxx0011
// �� ���� ����:
// PORTC = 0000 0000
// PORTB = 0000 0011
	LCD_STROBE();
	__delay_us(5000);					// 5 ms
	LCD_STROBE();
	__delay_us(100);				// 100 us
	LCD_STROBE();
// ����� ������������� ��� 4������� ����������
// ��������� 4�, ����� �����, ������
	PB_buf = ((PB_buf & 0xF0) | 0x02);
	PORTB = PB_buf;
	LCD_STROBE();
	//4 ����, 2 ������, 5*8 �����
	LCD_FunctionSet( DataWidth_4bits|LinesCount_2|FontType_5x8 );
	//Display OFF
	LCD_Control( DisplayOff|CursorOff|BlinkingOff );
	//Display clear
	LCD_Clear();
	// Entry mode: AC++, Shift OFF
	LCD_EntryMode( MoveDirectionInc|DisplayShiftOff );
	// Display, UCur, BCur
	LCD_Control( DisplayOn|CursorOn|BlinkingOn );
}

void LCD_GotoXY(byte aXY)
{
	// aXY = 'xxxx000y'
	LCD_WaitBusyFlag();
//	LcdData = (0x80|((aXY&0xF0)>>4 + (aXY&0x01&0x40)));
	LcdData = 0x80 | aXY;
	LCD_RS = 0;
	LCD_WriteByte();
}


void LCD_Home(void)
{
	LCD_WaitBusyFlag();
	LCD_RS = 0;
	LcdData = 0x02;
	LCD_WriteByte();
}

void LCD_EntryMode(byte aParameters)
{
	LCD_WaitBusyFlag();
	LCD_RS = 0;
	LcdData = (0x04|(MoveDirectionInc&aParameters)|(DisplayShiftOn&aParameters));
	LCD_WriteByte();
}

void LCD_FunctionSet(byte aParameters)
{
	LCD_WaitBusyFlag();
	LCD_RS = 0;
	LcdData = (0x20|(DataWidth_8bits&aParameters)|(LinesCount_2&aParameters)|(FontType_5x11&aParameters));
	LCD_WriteByte();
}

void LCD_Clear(void)
{
	LCD_WaitBusyFlag();
	LCD_RS = 0;
	LcdData = 0x01;
	LCD_WriteByte();
}

void LCD_Control(byte aParameters)
{
	LCD_WaitBusyFlag();
	LCD_RS = 0;			// SetCommand
	LcdData = (0x08|(aParameters&DisplayOn)|(aParameters&CursorOn)|(aParameters&BlinkingOn));
	LCD_WriteByte();
}

byte LCD_getxy()
{
	LCD_WaitBusyFlag();
	LCD_RS = 0;			// SetCommand
	LCD_ReadByte();
	LcdData &= 0x7F;
	return (LcdData & 0x7F);
}
char LCD_getc(void)
{
	LCD_WaitBusyFlag();
	LCD_RS = 1;
	LCD_ReadByte();
	return LcdData;
}

void LCD_MoveUp(void)
{
// ����������� ������ ������ � �������,
// � ������ ��������
	// ���������� ������� �������
	byte /*saveAC,*/ symbol;
//	LCD_WaitBusyFlag();
//	LCD_RS = 0;
//	LCD_ReadByte();
//	saveAC = LcdData;
	
	for (int i = 0; i < 16; i++)
	{
		// ���������� ������� � �������
		LCD_GotoXY(0x40 + i);
		LCD_WaitBusyFlag();
		LCD_RS = 1;
		LCD_ReadByte();
		symbol = LcdData;
		// ������� �������
		LCD_GotoXY(0x40 + i);
		LCD_WaitBusyFlag();
		LcdData = 0x20;				// space
		LCD_RS = 1;
		LCD_WriteByte();
		// ������ ������� � �������
		LCD_GotoXY(i);
		LCD_WaitBusyFlag();
		LcdData = symbol;
		LCD_RS = 1;
		LCD_WriteByte();
	}
	// �������������� �������
//	LCD_GotoXY( saveAC );
}
byte LCD_convertRUS(byte aSym)
{
//#pragma switch direct
	switch (aSym)
	{
		case '�':
			return 'A';
		case '�':
			return 0xA0;
		case '�':
			return 'B';
		case '�':
			return 0xA1;
		case '�':
			return 0xE0;
		case '�':
			return 'E';
		case '�':
			return 0xA2;
		case '�':
			return 0xA3;
		case '�':
			return 0xA4;
		case '�':
			return 0xa5;
		case '�':
			return 0xa6;
		case '�':
			return 'K';
		case '�':
			return 0xa7;
		case '�':
			return 'M';
		case '�':
			return 'H';
		case '�':
			return 'O';
		case '�':
			return 0xa8;
		case '�':
			return 'P';
		case '�':
			return 'C';
		case '�':
			return 'T';
		case '�':
			return 0xa9;
		case '�':
			return 0xaa;
		case '�':
			return 'X';
		case '�':
			return 0xe1;
		case '�':
			return 0xab;
		case '�':
			return 0xac;
		case '�':
			return 0xe2;
		case '�':
			return 0xad;
		case '�':
			return 0xae;
		case '�':
			return 'b';
		case '�':
			return 0xaf;
		case '�':
			return 0xb0;
		case '�':
			return 0xb1;
		case '�':
			return 'a';
		case '�':
			return 0xb2;
		case '�':
			return 0xb3;
		case '�':
			return 0xb4;
		case '�':
			return 0xe3;
		case '�':
			return 'e';
		case '�':
			return 0xb5;
		case '�':
			return 0xb6;
		case '�':
			return 0xb7;
		case '�':
			return 0xb8;
		case '�':
			return 0xb9;
		case '�':
			return 0xba;
		case '�':
			return 0xbb;
		case '�':
			return 0xbc;
		case '�':
			return 0xbd;
		case '�':
			return 'o';
		case '�':
			return 0xbe;
		case '�':
			return 'p';
		case '�':
			return 'c';
		case '�':
			return 0xbf;
		case '�':
			return 'y';
		case '�':
			return 0xe4;
		case '�':
			return 'x';
		case '�':
			return 0xe5;
		case '�':
			return 0xc0;
		case '�':
			return 0xc1;
		case '�':
			return 0xe6;
		case '�':
			return 0xc3;
		case '�':
			return 0xc2;
		case '�':
			return 0xc4;
		case '�':
			return 0xc5;
		case '�':
			return 0xc6;
		case '�':
			return 0xc7;
		default:
			return aSym;
	}
//#pragma switch auto
}
void LCD_putc(byte aSym)
{
#define HT_SIZE 4
	byte xy;
	switch (aSym)
	{
		case 9:
			// �������������� ���������
			xy = LCD_getxy();
			xy += (HT_SIZE - (xy % HT_SIZE));
			LCD_GotoXY(xy);
			return;
		case 10:
			// ������� ������
			xy = LCD_getxy();
			if (xy >= 0x40)
				LCD_MoveUp();
			xy = 0x40;
			LCD_GotoXY(xy);
			return;
		case 13:
			// ������� �������
			xy = LCD_getxy();
			if (xy >= 0x40)
				xy = 0x40;
			else
				xy = 0;
			LCD_GotoXY(xy);
			return;
	}
	aSym = LCD_convertRUS(aSym);
	LCD_WaitBusyFlag();
	LCD_RS = 1;
	LcdData = aSym;
	LCD_WriteByte();
}