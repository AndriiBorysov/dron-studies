#include <stdio.h>
#include <math.h>
#include <conio.h>

double func_y1(double y1, double y2, double t);
double func_y2(double y1, double y2, double t);

const double step = 1;
const int start = 0;
const int count = start + 20;

void main(void)
{
	clrscr();
	double y1_0 = 50;
	double y2_0 = 60;
	double y1_new, y2_new;
	for (int i = start; i < count; i++)
	{
		if (y2_0 < 0)
			y2_0 = 0;
		if (y1_0 < 0)
			y1_0 = 0;
		printf("%3d: y1 = %3li y2 = %3li\n",
			i, (long)y1_0, (long)y2_0);
		if ((long)y1_0 <= 0 || (long)y2_0 <= 0)
		{
			printf("Population died!\n");
			break;
		}
		y1_new = y1_0 + step*func_y1(y1_0, y2_0, i*step);
		y2_new = y2_0 + step*func_y2(y1_0, y2_0, i*step);
		y1_0 = y1_new;
		y2_0 = y2_new;
	}
}

double func_y1(double y1, double y2, double t)
{
	return (0.7*y1 - 0.4*y2 + sin(t));
}
double func_y2(double y1, double y2, double t)
{
	return (0.4*y1 - 0.2*y2 + sin(t));
}
