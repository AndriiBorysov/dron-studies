#include "main.h"

void main(void)
{
using namespace std;
char *szQueryString;
char *szMethod,*szString;
int size;
// ����� HTTP-���������:
printf("Content-type:text/html;charset=windows-1251\n\n"); 
// ������������ ������������ Web-��������:
printf("<H1 style=\"text-align:center;font:italic bold 10mm;color:red\">\
�������� ������������ CGI ����������� </H1>");
szMethod=getenv("REQUEST_METHOD"); 
printf("<P style=\"text-align:center; font-size:7mm; color:green\"> ����� �������� ������: %s</p>",szMethod);
size=atoi(getenv("CONTENT_lENGTH")); 
szString=(char*)malloc(size*sizeof(char)); 
fread(szString,size,1,stdin); 
szString[size]='\0'; 
printf("<P>����� ���������� ����������: %d</p>",size); 
printf("<P>���������, ���������� �� �����: %s</p>",szString);
for(int i=0;i<size;i++)
{
if(szString[i]=='+')
szString[i]=' ';
}
char *token;
char seps[]   = "=&";
 token = strtok(szString,seps); 
   while( token != NULL )
   {
	   printf( "<P> %s:", token );
      token = strtok( NULL, seps ); 
	  printf( "%s</p>", token );
      token = strtok( NULL, seps );
   }

}