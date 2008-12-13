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
//----------------------------------------------------------------------------
class TForm1 : public TForm
{
__published:
	TAutoIncField *Table1ID;
	TCurrencyField *Table1Sum;
	TStringField *Table1Operation;
	TDateField *Table1Date;
	TDBGrid *DBGrid1;
	TDBNavigator *DBNavigator;
	TPanel *Panel1;
	TDataSource *DataSource1;
	TPanel *Panel2;
	TTable *Table1;
	void __fastcall FormCreate(TObject *Sender);
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
