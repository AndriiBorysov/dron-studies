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
void __fastcall TForm1::btnBalanceClick(TObject *Sender)
{
        if ( DateTimePicker1->Date > DateTimePicker2->Date )
        {
                TDateTime date = DateTimePicker2->Date;
                DateTimePicker2->Date = DateTimePicker1->Date;
                DateTimePicker1->Date = date;
        }
        EditBalance->Text = "0";
        Currency sum = 0;
        DateTimePicker1->Time = 0;
        DateTimePicker2->Time = 0;
        TDate beginDate = DateTimePicker1->DateTime,
        endDate = DateTimePicker2->DateTime;
        Table1->First();
        for(int i=0; i < Table1->RecordCount; i++)
        {
                if( beginDate <= Table1->Fields->Fields[3]->AsDateTime &&
                Table1->Fields->Fields[3]->AsDateTime <= endDate )
                        sum += Table1->Fields->Fields[1]->AsCurrency;
                Table1->Next();
        }
        Table1->First();
        EditBalance->Text = sum.operator AnsiString().c_str();
}
//---------------------------------------------------------------------------
void __fastcall TForm1::DateTimePicker2CloseUp(TObject *Sender)
{
        if ( DateTimePicker2->Date < DateTimePicker1->Date )
                DateTimePicker2->Date = DateTimePicker1->Date;
}
//---------------------------------------------------------------------------
void __fastcall TForm1::DateTimePicker1CloseUp(TObject *Sender)
{
        if ( DateTimePicker1->Date > DateTimePicker2->Date )
                DateTimePicker1->Date = DateTimePicker2->Date;
}
//---------------------------------------------------------------------------
void __fastcall TForm1::Table1BeforePost(TDataSet *DataSet)
{
        TDateTime date = 0;
        if ( DataSet->Fields->Fields[3]->AsDateTime == date)
        {
                date = TDateTime::CurrentDate();
                DataSet->Fields->Fields[3]->AsDateTime = date;
        }
}
//---------------------------------------------------------------------------
