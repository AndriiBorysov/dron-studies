//----------------------------------------------------------------------------
#include <vcl.h>
#pragma hdrstop

#include "Unit1.h"
//----------------------------------------------------------------------------
#pragma resource "*.dfm"
TForm1 *Form1;
//----------------------------------------------------------------------------
__fastcall TForm1::TForm1(TComponent *Owner)
	: TForm(Owner)
{
}
//----------------------------------------------------------------------------
void __fastcall TForm1::FormCreate(TObject *Sender)
{
	Table1->Open();
}
//---------------------------------------------------------------------------- 