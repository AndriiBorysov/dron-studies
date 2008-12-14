//----------------------------------------------------------------------------
#ifndef Unit1H
#define Unit1H
//----------------------------------------------------------------------------
#include <SysUtils.hpp>
#include <Windows.hpp>
#include <Messages.hpp>
#include <Classes.hpp>
#include <Graphics.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include <DBCtrls.hpp>
#include <DB.hpp>
#include <DBGrids.hpp>
#include <DBTables.hpp>
#include <ExtCtrls.hpp>
#include <Grids.hpp>
#include <ComCtrls.hpp>
//----------------------------------------------------------------------------
class TForm1 : public TForm
{
__published:
	TAutoIncField *Table1ID;
	TCurrencyField *Table1Sum;
	TStringField *Table1Operation;
	TDBGrid *DBGrid1;
	TDBNavigator *DBNavigator;
	TPanel *Panel1;
	TDataSource *DataSource1;
	TPanel *Panel2;
	TTable *Table1;
        TGroupBox *grpBalance;
        TDateTimePicker *DateTimePicker1;
        TLabel *Label1;
        TDateTimePicker *DateTimePicker2;
        TLabel *Label2;
        TLabel *Label3;
        TButton *btnBalance;
        TLabeledEdit *EditBalance;
        TDateField *Table1Date;
	void __fastcall FormCreate(TObject *Sender);
        void __fastcall btnBalanceClick(TObject *Sender);
        void __fastcall DateTimePicker2CloseUp(TObject *Sender);
        void __fastcall DateTimePicker1CloseUp(TObject *Sender);
        void __fastcall Table1BeforePost(TDataSet *DataSet);
private:
	// private declarations
public:
	// public declarations
	__fastcall TForm1(TComponent *Owner);
};
//----------------------------------------------------------------------------
extern TForm1 *Form1;
//----------------------------------------------------------------------------
#endif
