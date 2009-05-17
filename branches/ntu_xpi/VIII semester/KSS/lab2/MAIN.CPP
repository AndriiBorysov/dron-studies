#include <stdio.h>
#include <dos.h>
#include "rungkutt.cpp"

void main(void)
{
	TFormulArray functions;
	functions[E_Y1] = &Formula_Y1;
	functions[E_Y2] = &Formula_Y2;
	functions[E_Y3] = &Formula_Y3;
	functions[E_I] = &Formula_I;
	functions[E_I1] = &Formula_I1;
	functions[E_X] = &Formula_X;

	TFunctionValues* totalValues = new TFunctionValues[STEPS];
	if ( !totalValues )
		{
		exit(1);
		}

	clrscr();
	for ( int index = 0; index < STEPS; index++ )
		{
		for ( int num = EFirst; num <= ELast; num++ )
			{
			totalValues[index][num] = 0;
			}
		}
	for ( index = 1; index < STEPS; index++ )
		{
		totalValues[index][E_Y1] =
			Runge_Kutt( totalValues[index-1], functions, E_Y1, index * step );
		totalValues[index][E_Y2] =
			Runge_Kutt( totalValues[index-1], functions, E_Y2, index * step );
		totalValues[index][E_Y3] =
			Runge_Kutt( totalValues[index-1], functions, E_Y3, index * step );
		totalValues[index][E_X] =
			Runge_Kutt( totalValues[index-1], functions, E_X, index * step );
		totalValues[index][E_I] =
			Runge_Kutt( totalValues[index-1], functions, E_I, index * step );
		totalValues[index][E_I1] =
			Runge_Kutt( totalValues[index-1], functions, E_I1, index * step );
		}
/*	printf("step\t\ty1\ty2\ti\tx\ty3\ti1\n");
	for ( index = 0; index < STEPS; index++ )
	{
		printf("%d\t%11.7f%11.7f%11.7f%11.7f%11.7f%11.7f\n",
		index, totalValues[index][E_Y1], totalValues[index][E_Y2], totalValues[index][E_I],
		totalValues[index][E_X], totalValues[index][E_Y3], totalValues[index][E_I1] );
		delay(100);
	}
	getch();
*/
	int gdriver = DETECT, gmode, errorcode;
	initgraph(&gdriver, &gmode, "f:\\programs\\bc31\\bgi");
	errorcode = graphresult();
	if (errorcode != grOk)
	{
		printf("Graphics error: %s\n", grapherrormsg(errorcode));
		printf("Press any key to exit...");
		getch();
		exit(1);
	}
	int sry;
	sry = getmaxy() / 2;
	line(0, 20, 0, getmaxy() - 20);
	line(0, sry, getmaxx() - 20, sry);
	line(0, 40, 0, 20);
	line(getmaxx() - 50, sry - 5, getmaxx() - 20, sry);
	line(5, 40, 0, 20);
	line(getmaxx() - 50, sry +5, getmaxx() - 20, sry);

	double array[STEPS];
	for ( index = 0; index < STEPS; index++ )
		{
		array[index] = totalValues[index][E_I1];
		}
	graphics(array, STEPS, EGA_GREEN);

	for ( index = 0; index < STEPS; index++ )
		{
		array[index] = totalValues[index][E_Y2];
		}
	graphics(array, STEPS, EGA_RED);

	for ( index = 0; index < STEPS; index++ )
		{
		array[index] = totalValues[index][E_Y3];
		}
	graphics(array, STEPS, EGA_BLUE);
	getch();
	//	closegraph();
	delete[] totalValues;
}