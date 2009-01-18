#include <htc.h>
//#include <pic.h>
#include "lcd.h"


void LCD_WriteByte()
{
	TRISB &= 0xF0;		// установка порта данных на вывод
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
// инициализация для 4битного интерфейса
	__delay_us(15000);					// 15ms
	PB_buf = ((PB_buf & 0xF0)|0x03);
	PORTB = PB_buf;					// PORTB = xxxx0011
// на этом шаге:
// PORTC = 0000 0000
// PORTB = 0000 0011
	LCD_STROBE();
	__delay_us(5000);					// 5 ms
	LCD_STROBE();
	__delay_us(100);				// 100 us
	LCD_STROBE();
// конец инициализации для 4битного интерфейса
// установка 4б, числа строк, шрифта
	PB_buf = ((PB_buf & 0xF0) | 0x02);
	PORTB = PB_buf;
	LCD_STROBE();
	//4 бита, 2 строки, 5*8 точек
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
// скопировать нижнюю строку в верхнюю,
// а нижнюю очистить
	// сохранение позиции курсора
	byte /*saveAC,*/ symbol;
//	LCD_WaitBusyFlag();
//	LCD_RS = 0;
//	LCD_ReadByte();
//	saveAC = LcdData;
	
	for (int i = 0; i < 16; i++)
	{
		// сохранение символа в позиции
		LCD_GotoXY(0x40 + i);
		LCD_WaitBusyFlag();
		LCD_RS = 1;
		LCD_ReadByte();
		symbol = LcdData;
		// очистка позиции
		LCD_GotoXY(0x40 + i);
		LCD_WaitBusyFlag();
		LcdData = 0x20;				// space
		LCD_RS = 1;
		LCD_WriteByte();
		// запись символа в позицию
		LCD_GotoXY(i);
		LCD_WaitBusyFlag();
		LcdData = symbol;
		LCD_RS = 1;
		LCD_WriteByte();
	}
	// восстановление курсора
//	LCD_GotoXY( saveAC );
}
byte LCD_convertRUS(byte aSym)
{
//#pragma switch direct
	switch (aSym)
	{
		case 'А':
			return 'A';
		case 'Б':
			return 0xA0;
		case 'В':
			return 'B';
		case 'Г':
			return 0xA1;
		case 'Д':
			return 0xE0;
		case 'Е':
			return 'E';
		case 'Ё':
			return 0xA2;
		case 'Ж':
			return 0xA3;
		case 'З':
			return 0xA4;
		case 'И':
			return 0xa5;
		case 'Й':
			return 0xa6;
		case 'К':
			return 'K';
		case 'Л':
			return 0xa7;
		case 'М':
			return 'M';
		case 'Н':
			return 'H';
		case 'О':
			return 'O';
		case 'П':
			return 0xa8;
		case 'Р':
			return 'P';
		case 'С':
			return 'C';
		case 'Т':
			return 'T';
		case 'У':
			return 0xa9;
		case 'Ф':
			return 0xaa;
		case 'Х':
			return 'X';
		case 'Ц':
			return 0xe1;
		case 'Ч':
			return 0xab;
		case 'Ш':
			return 0xac;
		case 'Щ':
			return 0xe2;
		case 'Ъ':
			return 0xad;
		case 'Ы':
			return 0xae;
		case 'Ь':
			return 'b';
		case 'Э':
			return 0xaf;
		case 'Ю':
			return 0xb0;
		case 'Я':
			return 0xb1;
		case 'а':
			return 'a';
		case 'б':
			return 0xb2;
		case 'в':
			return 0xb3;
		case 'г':
			return 0xb4;
		case 'д':
			return 0xe3;
		case 'е':
			return 'e';
		case 'ё':
			return 0xb5;
		case 'ж':
			return 0xb6;
		case 'з':
			return 0xb7;
		case 'и':
			return 0xb8;
		case 'й':
			return 0xb9;
		case 'к':
			return 0xba;
		case 'л':
			return 0xbb;
		case 'м':
			return 0xbc;
		case 'н':
			return 0xbd;
		case 'о':
			return 'o';
		case 'п':
			return 0xbe;
		case 'р':
			return 'p';
		case 'с':
			return 'c';
		case 'т':
			return 0xbf;
		case 'у':
			return 'y';
		case 'ф':
			return 0xe4;
		case 'х':
			return 'x';
		case 'ц':
			return 0xe5;
		case 'ч':
			return 0xc0;
		case 'ш':
			return 0xc1;
		case 'щ':
			return 0xe6;
		case 'ы':
			return 0xc3;
		case 'ъ':
			return 0xc2;
		case 'ь':
			return 0xc4;
		case 'э':
			return 0xc5;
		case 'ю':
			return 0xc6;
		case 'я':
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
			// горизонтальная табуляция
			xy = LCD_getxy();
			xy += (HT_SIZE - (xy % HT_SIZE));
			LCD_GotoXY(xy);
			return;
		case 10:
			// перевод строки
			xy = LCD_getxy();
			if (xy >= 0x40)
				LCD_MoveUp();
			xy = 0x40;
			LCD_GotoXY(xy);
			return;
		case 13:
			// возврат каретки
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