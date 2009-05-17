#include <stdio.h>
#include <dos.h>
#include "rungkutt.h"

void main(void)
{
	clrscr();
	TFormulArray functions;
	functions[E_DV] = &Formula_V;

	TFunctionValues* totalValues = new TFunctionValues[STEPS];
	for ( int index = 0; index < STEPS; index++ )
		{
		totalValues[index][E_DV] = 0;
		}
	if ( !totalValues )
		{
		delete totalValues;
		exit( 1 );
		}

	clrscr();
	// initial value on first interval
	double timeBegin = KTimeBegin + KTau;
	int indexBegin = KTau/step + 1;
	for ( index = 0; index <= indexBegin; index++ )
		{
		totalValues[index][E_DV] = 1;	// CHANGE
		}
	for ( index = indexBegin; index < STEPS; index++ )
		{
		totalValues[index + 1][E_DV] =
			Runge_Kutt( totalValues, functions,
			index, E_DV, KTimeBegin + index * step);

		}

	// analitic solving
	double t = KTimeBegin;
	double x = 0;
	for (int i = 0; i < STEPS; i++)
	{
		// CHANGE ALL
		if (t >= -2 && t < 1.0)
		{
			x = 1;
		}
		if (t >= -1 && t < 0.0)
		{
			x = 1.5*t*t + 2*t + 1.5;
		}
		if (t >= 0 && t < 1)
		{
			x = 1.5*t*t + (t-1)*(t-1)*(t-1) + 2*(t-1)*(t-1) + 3*t + 0.5;
		}
		printf("time(%10.6f) x(%10.6f)\n", t, x);
		t += step;
	}
	// digit solving print
	printf("step\t\tdv\n");
	for (i = 0; i < STEPS; i++)
	{
		printf("%d\t%10.6f\t%10.6f\n",
			i, KTimeBegin + i * step, totalValues[i][E_DV] );
	}
	getch();
	delete[] totalValues;
}