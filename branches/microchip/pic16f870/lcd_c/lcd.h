/*
	”правление LCD WH1602D
*/
#ifndef __LCD_H_
#define __LCD_H_

typedef unsigned char byte;
//LCD_EntryMode
#define DisplayShiftOn		0x01
#define DisplayShiftOff		0
#define MoveDirectionInc	0x02
#define MoveDirectionDec	0
//LCD_FunctionSet
#define DataWidth_4bits		0
#define DataWidth_8bits		0x10
#define LinesCount_1		0
#define LinesCount_2		0x08
#define FontType_5x8		0
#define FontType_5x11		0x04
//LCD_Control
#define DisplayOn			0x04
#define DisplayOff			0
#define CursorOn			0x02
#define CursorOff			0
#define BlinkingOn			0x01
#define BlinkingOff			0

#define	LCD_RS			RC3
#define	LCD_RW			RC4
#define LCD_EN			RC5
#define LCD_DATA		PORTB
#define	LCD_STROBE()	((LCD_EN = 1),(LCD_EN=0))

#ifndef _XTAL_FREQ
#define _XTAL_FREQ		16000000		// 16MHz
#endif


extern char PB_buf;
unsigned char LcdData;

void InitLCD(void);
extern void LCD_GotoXY(byte aParameters);
extern void LCD_Home(void);
extern void LCD_EntryMode(byte aParameters);
extern void LCD_FunctionSet(byte aParameters);
extern void LCD_Clear(void);
extern void LCD_Control(byte aParameters);
char LCD_getc(void);
void LCD_putc(byte aSym);
void LCD_WriteByte();
void LCD_ReadByte();
void LCD_WaitBusyFlag(void);

#endif
