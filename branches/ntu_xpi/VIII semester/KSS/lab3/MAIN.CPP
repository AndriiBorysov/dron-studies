#include <stdio.h>
#include <dos.h>
#include "rungkutt.cpp"

void main(void)
{
	TFormulArray functions;
	functions[E_N1] = &Formula_N1;
	functions[E_N2] = &Formula_N2;
	functions[E_N3] = &Formula_N3;

	TFunctionValues* totalValues = new TFunctionValues[STEPS];
	if ( !totalValues )
		{
		exit(1);
		}

	clrscr();
	for ( int index = 0; index < STEPS; index++ )
		{
		totalValues[index][E_N1] = 0;
		totalValues[index][E_N2] = 0;
		totalValues[index][E_N3] = 0;
		}
//	for ( int index = 0; index < STEPS; index++ )
		{
		totalValues[0][E_N1] = PEOPLE_COUNT;
		totalValues[0][E_N2] = ILL_PEOPLE;
		totalValues[0][E_N3] = 0;
		}
	printf("step\t\tn1\tn2\tn3\n");
	for ( index = 1; index < STEPS; index++ )
		{
		totalValues[index][E_N1] =
			Runge_Kutt( totalValues[index-1], functions, E_N1, index * step );
		totalValues[index][E_N2] =
			Runge_Kutt( totalValues[index-1], functions, E_N2, index * step );
		totalValues[index][E_N3] =
			Runge_Kutt( totalValues[index-1], functions, E_N3, index * step );
		printf("%d\t%11.4g%11.4g%11.4g\n",
		index, totalValues[index][E_N1], totalValues[index][E_N2],
		totalValues[index][E_N3]);
		}
	getch();

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

	double array[STEPS];
	for ( index = 0; index < STEPS; index++ )
		{
		array[index] = totalValues[index][E_N1];
		}
	graphics(array, STEPS, EGA_GREEN);
	for ( index = 0; index < STEPS; index++ )
		{
		array[index] = totalValues[index][E_N2];
		}
	graphics(array, STEPS, EGA_RED);

	for ( index = 0; index < STEPS; index++ )
		{
		array[index] = totalValues[index][E_N3];
		}
	graphics(array, STEPS, EGA_BLUE);

	setcolor(EGA_WHITE);
	outtextxy(20, getmaxy() - 20,
	"Green - zdorovie     Red - illnesses   Blue - immunitet");
	line(0, 20, 0, getmaxy() - 20);
	line(0, sry, getmaxx() - 20, sry);
	line(0, 40, 0, 20);
	line(getmaxx() - 50, sry - 5, getmaxx() - 20, sry);
	line(5, 40, 0, 20);
	line(getmaxx() - 50, sry +5, getmaxx() - 20, sry);
	getch();
//	closegraph();
	delete[] totalValues;
}