#include <graphics.h>
#include <stdio.h>
#include <conio.h>
#include <stdlib.h>
#include <math.h>

#define STEPS 160	// CHANGE, count
#define step 0.02	// CHANGE, step
#define KTimeBegin -2	// CHANGE, first of all time

#define KTau 1.0
#define KTauSteps KTau/step
#define KH1 2
#define KH2 0.8
#define KH3 10000
#define KH4 0.17
#define KH5 0.5
#define KH6 10
#define KH7 0.12
#define KH8 8

enum TFormulaNum
	{
	EFirst = 0,
	E_DV = EFirst,
	E_DS,
	E_DF,
	E_DM,
	ELast = E_DM
	};

#define KFormulCount 1
typedef double TFunctionValues[KFormulCount];
typedef double (*TFormulaFunction)(TFunctionValues aValues, double aAddX,
				double aAddY, double aRetard, double aTime);
typedef TFormulaFunction TFormulArray[KFormulCount];

int DoIndex(double aIndex)
{
	if ( aIndex < 0 )
	{
		aIndex = 0;
	}
	int result = aIndex + 0.0001;
//	printf("index = %i ", result);
	return result;
}

double Runge_Kutt(TFunctionValues* aValues, TFormulArray aFormuls,
	int aCurrentIndex, TFormulaNum aFormulNum, double aTime)
{
//	printf("\ntime = %5.3f\t", aTime);
	double fi0, fi1, fi2, fi3;
	TFormulaFunction function = aFormuls[aFormulNum];
//	printf("\tfi0:\n");
	// fi0
	fi0 = function( aValues[aCurrentIndex], 0, 0,
		aValues[DoIndex(aCurrentIndex - KTauSteps)][aFormulNum], aTime );
	fi0 *= step;
//	printf("\tfi1:\n");
	// fi1
	fi1 = function( aValues[aCurrentIndex], step*0.5, fi0*0.5,
		(aValues[DoIndex(aCurrentIndex - KTauSteps + 0.5)][aFormulNum] +
		aValues[DoIndex(aCurrentIndex - KTauSteps + 0.5 + 1)][aFormulNum])*0.5,
		aTime );
	fi1 *= step;
//	printf("\tfi2:\n");
	// fi2
	fi2 = function( aValues[aCurrentIndex], step*0.5, fi1*0.5,
		(aValues[DoIndex(aCurrentIndex - KTauSteps + 0.5)][aFormulNum] +
		aValues[DoIndex(aCurrentIndex - KTauSteps + 0.5 + 1)][aFormulNum])*0.5,
		aTime );
	fi2 *= step;
//	printf("\tfi3:\n");
	// fi3
	fi3 = function( aValues[aCurrentIndex], step, fi2,
		aValues[DoIndex(aCurrentIndex - KTauSteps + 1)][aFormulNum], aTime );
	fi3 *= step;

/*	printf( "RK = %11.6f%11.6f%11.6f%11.6f%11.6f\n", fi0, fi1, fi2,
		fi3, aValues[DoIndex(aCurrentIndex - 0)][aFormulNum] );
*/
	double result = aValues[DoIndex(aCurrentIndex - 0)][aFormulNum];
/*	printf("index = %d time = %5.2f  Y = %10.5f",
		DoIndex(aCurrentIndex - 0), aTime, result);
*/	result = (fi0 + 2*fi1 + 2*fi2 + fi3)/6.0;
//	printf("  dY = %10.5f\n", result);
	result += aValues[DoIndex(aCurrentIndex - 0)][aFormulNum];
	return ( result );
}
double Formula_Z(double aM)
{
	double result = 1;
	if ( aM >= 0.1 && aM <= 1 )
		{
		result = (1 - aM)*(10/9);
		}
	return result;
}
// first	/ V
double Formula_V(TFunctionValues aValues, double aAddX, double /*aAddY*/,
				 double aRetard, double aTime)
{
	// current: x'(t) = 3*t + 2*x(t-1)
	return 3 * (aTime + aAddX) + 2 * aRetard;
//	return (KH1 - KH2 * aValues[E_DF]) * (aValues[E_DV] + aAddY);
}
// second	/ S
double Formula_S(TFunctionValues aValues, double /*aAddX*/, double aAddY,
				 TFunctionValues aRetard, double /*aTime*/)
{
	return (KH3 * Formula_Z(aValues[E_DM]) * aRetard[E_DF] *
		aRetard[E_DV] - KH5 * ((aValues[E_DS] + aAddY) - 1));
}
// third	/ F
double Formula_F(TFunctionValues aValues, double /*aAddX*/, double aAddY,
				 TFunctionValues /*aRetard*/, double /*aTime*/)
{
	return (KH4 * (aValues[E_DS] - (aValues[E_DF] + aAddY)) -
		KH8 * (aValues[E_DF] + aAddY) * aValues[E_DV]);
}
// fourth	/ M
double Formula_M(TFunctionValues aValues, double /*aAddX*/, double /*aAddY*/,
				 TFunctionValues /*aRetard*/, double /*aTime*/)
{
	return (KH6 * aValues[E_DV] - KH7 * aValues[E_DM]);
}

void graphics(double* mas, int count, int color)
{
	int /*srx,*/ sry, x0, y0, mx, my, i;
	sry = getmaxy() / 2;
	mx = (getmaxx()) / STEPS;
	my = (sry - 0) / 200;

	x0 = 0;
	y0 = (int)(sry-my*mas[0]+0.5);
	moveto(x0, y0);
	setcolor(color);
	for ( i = 0; i < count; i++ )
	{
		lineto((int)(i*mx+0.5), (int)(sry-my*mas[i]+0.5));
	}
}