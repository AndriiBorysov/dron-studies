#include <htc.h>
//#include <pic.h>
#include "lcd.h"
#include <stdio.h>

__CONFIG(HS & WDTDIS & BORDIS & LVPDIS);


char PB_buf;

void putch(char c)
{
	LCD_putc(c);
}

void main(void)
{
	InitLCD();
	printf("");
	for(;;);
}