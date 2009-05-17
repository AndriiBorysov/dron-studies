#include <graphics.h>
#include <stdio.h>
#include <conio.h>
#include <stdlib.h>
#include <math.h>

#define STEPS 100
#define step 0.01

#define PEOPLE_COUNT 100
#define ILL_PEOPLE 1

#define ConstMU 5	/*new incomings*/
#define ConstB 0.8	/*freq communication between groups*/
#define ConstJ 10      /* = coef ~1/(avrg time healthing)*/

enum TFormulaNum
	{
	EFirst = 0,
	E_N1 = EFirst,
	E_N2,
	E_N3,
	ELast = E_N3
	};

#define KFormulCount 3
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
// first	/ n1
double Formula_N1(TFunctionValues aValues, double aAddX, double aAddY, double aTime)
{
	return (ConstMU - ConstB * (aValues[E_N1] + aAddY) * aValues[E_N2]);
}
// second	/ n2
double Formula_N2(TFunctionValues aValues, double aAddX, double aAddY, double aTime)
{
	return (ConstB * aValues[E_N1] * (aValues[E_N2] + aAddY) -
	ConstJ * (aValues[E_N2] + aAddY));
}
// third	/ n3
double Formula_N3(TFunctionValues aValues, double aAddX, double aAddY, double aTime)
{
	return (ConstJ * aValues[E_N2]);
}

void graphics(double* mas, int count, int color)
{
	int /*srx,*/ sry, x0, y0, mx, my, i;
	sry = getmaxy() / 2;
	mx = (getmaxx()) / STEPS;
	my = (sry - 0) / 200;

	x0 = 0;
	y0 = (int)(sry-my*mas[0]*0.5+0.5);
	moveto(x0, y0);
	setcolor(color);
	for ( i = 0; i < count; i++ )
	{
		lineto((int)(i*mx+0.5), (int)(sry-my*mas[i]*0.5+0.5));
	}
}