#ifndef _RUNGEKUTT_H_
#define _RUNGEKUTT_H_

#include <graphics.h>
#include <stdio.h>
#include <conio.h>
#include <stdlib.h>

#define STEPS 200
#define step 0.001

#define ConstA1 1.333
#define ConstA2 0.352
#define ConstA3 0.128
#define ConstA4 0.133
#define ConstA5 3.556
#define ConstA6 0.0003
#define ConstA7 1.899
#define ConstA8 0.055

#define ConstB1 5.69
#define ConstB2 0.0016
#define ConstB3 0.069
#define ConstB4 0.552
#define ConstB5 0.0079
#define ConstB6 1.333

#define ConstC1 0.314
#define ConstC2 0.0033

#define ConstK1 58
#define ConstK2 40.2

#define ConstTau 0.005
#define ConstTau1 0.1
#define ConstG1 1333
#define ConstI1 250

enum TFormulaNum
	{
	EFirst = 0,
	E_Y1 = EFirst,
	E_Y2,
	E_I,
	E_X,
	E_Y3,
	E_I1,
	ELast = E_I1
	};

#define KFormulCount 6
typedef double TFunctionValues[KFormulCount];
typedef double (*TFormulaFunction)(TFunctionValues aValues, double aAddX, double aAddY, double aTime);
typedef TFormulaFunction TFormulArray[KFormulCount];


double Runge_Kutt(TFunctionValues aValues, TFormulArray aFormuls, TFormulaNum aFormulNum, double aTime)
{
	double fi0, fi1, fi2, fi3;
	TFormulaFunction function = aFormuls[aFormulNum];
	// fi0
	fi0 = function( aValues, 0, 0, aTime );
	fi0 *= step;
	// fi1
	fi1 = function( aValues, step*0.5, fi0*0.5, aTime );
	fi1 *= step;
	// fi2
	fi2 = function( aValues, step*0.5, fi1*0.5, aTime );
	fi2 *= step;
	// fi3
	fi3 = function( aValues, step, fi2, aTime );
	fi3 *= step;
	return ( aValues[aFormulNum] + (fi0 + 2*fi1 + 2*fi2 + fi3)/6.0 );
}
// first	/ y1
double Formula_Y1(TFunctionValues aValues, double aAddX, double aAddY, double aTime)
{
	double result = 0;
	aTime += aAddX;
	if ( aTime >= 0 && aTime <= ConstTau)
		result = ConstK1 * ConstG1;
	result -= ConstA1 * ( aValues[E_Y1] + aAddY );
	return result;
}
// second	/ y2
double Formula_Y2(TFunctionValues aValues, double aAddX, double aAddY, double aTime)
{
	return ( -ConstA2*((aValues[E_Y2] + aAddY) + aValues[E_Y3]) + ConstA3 * aValues[E_Y1] - 
		ConstB1 * aValues[E_I] + ConstC1 * aValues[E_X] - 
		ConstB4 * aValues[E_I1] - ConstA4 * (aValues[E_Y2] + aAddY) );
}
// third	/ i
double Formula_I(TFunctionValues aValues, double aAddX, double aAddY, double aTime)
{
	double result = 0;
	if ( aValues[E_Y2] + aValues[E_Y3] > 0 )
		{
		result = ConstA5 * (aValues[E_Y2] + aValues[E_Y3]);
		}
	result -= ConstB2 * ( aValues[E_I] + aAddY );
	return result;
}
// Fourth	/ x
double Formula_X(TFunctionValues aValues, double aAddX, double aAddY, double aTime)
{
	double result = 0;
	if ( aValues[E_Y2] + aValues[E_Y3] < 0 )
	{
		result = ConstA6 * (aValues[E_Y2] + aValues[E_Y3]);
	}
	result -= ConstC2 * ( aValues[E_X] + aAddY );
	return result;
}
// Fifth	/ y3
double Formula_Y3(TFunctionValues aValues, double aAddX, double aAddY, double aTime)
{
	return ( ConstA7*(aValues[E_Y2]) + ConstB3 * aValues[E_I] + ConstB5 * aValues[E_I1] - 
		ConstA8 * (aValues[E_Y3] + aAddY) ); 
}
// Sixth	/ i1
double Formula_I1(TFunctionValues aValues, double aAddX, double aAddY, double aTime)
{
	double result = 0;
	aTime += aAddX;
	if ( aTime >= 0 && aTime <= ConstTau1 )
		{
		result = ConstK2 * ConstI1;
		}
	result -= ConstB6 * ( aValues[E_I1] + aAddY );
	return result;
}

void graphics(double* mas, int count, int color)
{
	int /*srx,*/ sry, x0, y0, mx, my, i;
 //	srx = getmaxx() / 2;
	sry = getmaxy() / 2;
	x0 = 0;
	y0 = sry;
	mx = (getmaxx() - 20) / STEPS;
	my = (sry - 0) / 200;

	moveto(x0, y0);
	setcolor(color);
	for ( i = 0; i < count; i++ )
	{
		lineto((int)(i*mx+0.5), (int)(sry-my*mas[i]*0.5+0.5));
	}
}

#endif	// _RUNGEKUTT_H_