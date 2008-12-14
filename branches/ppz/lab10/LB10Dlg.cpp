// LB10Dlg.cpp : implementation file
//

#include "stdafx.h"
#include "LB10.h"
#include "LB10Dlg.h"
#include <math.h>
        
        


#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// CLB10Dlg dialog




CLB10Dlg::CLB10Dlg(CWnd* pParent /*=NULL*/)
	: CDialog(CLB10Dlg::IDD, pParent)
{
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
}

void CLB10Dlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_RICHEDIT21, richEdit);
	DDX_Control(pDX, IDC_EDITBALANCE, editBalance);
	DDX_Control(pDX, IDC_DATETIMEPICKERBEGIN, fileTimeBegin );
	DDX_Control(pDX, IDC_DATETIMEPICKEREND, fileTimeEnd );
}

BEGIN_MESSAGE_MAP(CLB10Dlg, CDialog)

	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	//}}AFX_MSG_MAP
	
	ON_BN_CLICKED(IDC_BUTTON1, &CLB10Dlg::OnBnClickedButton1)
	ON_NOTIFY(DTN_CLOSEUP, IDC_DATETIMEPICKERBEGIN, &CLB10Dlg::OnDtnCloseupDatetimepickerbegin)
	ON_NOTIFY(DTN_CLOSEUP, IDC_DATETIMEPICKEREND, &CLB10Dlg::OnDtnCloseupDatetimepickerend)
END_MESSAGE_MAP()


// CLB10Dlg message handlers

BOOL CLB10Dlg::OnInitDialog()
{
	CDialog::OnInitDialog();
	SetIcon(m_hIcon, TRUE);			// Set big icon
	SetIcon(m_hIcon, FALSE);		// Set small icon

    richEdit.SetFocus();
	richEdit.SetWindowText("");

	DAODBEngine* daoDb; 
	CDaoDatabase *database; 
	AfxDaoInit();  
	daoDb = AfxDaoGetEngine(); 
	ASSERT( daoDb != NULL );
	CString tableText="ID\tSum\t\tOperation\tDate\n";

	database=new CDaoDatabase(); 
	database->Open("c:\\db1.mdb");  
	if(!database->IsOpen())
	{
		MessageBox("Ошибка открытия",0,0);	
		exit(0);
	}
	else
	{
	    CDaoRecordset *record; 
		record=new CDaoRecordset(database);  
		CDaoTableDef *table; 
		table=new CDaoTableDef(database); 
		table->Open("table1"); 
		record->Open(table); 
		int recordCount = record->GetRecordCount(); 

		for (int i=0; i < recordCount;i++) 
		{  
			COleVariant temp; 
			record->GetFieldValue("ID",temp); 
			CString id = temp;
			record->GetFieldValue("Sum",temp); 
			CString sum  = temp;
			record->GetFieldValue("Operation",temp);
			CString operation = temp.pcVal;
			record->GetFieldValue("Date",temp);
			CString date = temp;
			tableText=tableText+id+"\t"+sum+"\t\t"+operation+"\t\t"+date+"\n";
			record->MoveNext();
		}
		richEdit.SetWindowText(tableText);
	}
	return TRUE;  // return TRUE  unless you set the focus to a control
}

void CLB10Dlg::OnPaint()
{
	if (IsIconic())
	{
		CPaintDC dc(this); // device context for painting

		SendMessage(WM_ICONERASEBKGND, reinterpret_cast<WPARAM>(dc.GetSafeHdc()), 0);

		// Center icon in client rectangle
		int cxIcon = GetSystemMetrics(SM_CXICON);
		int cyIcon = GetSystemMetrics(SM_CYICON);
		CRect rect;
		GetClientRect(&rect);
		int x = (rect.Width() - cxIcon + 1) / 2;
		int y = (rect.Height() - cyIcon + 1) / 2;

		// Draw the icon
		dc.DrawIcon(x, y, m_hIcon);
	}
	else
	{
		CDialog::OnPaint();
	}
}

// The system calls this function to obtain the cursor to display while the user drags
//  the minimized window.
HCURSOR CLB10Dlg::OnQueryDragIcon()
{
	return static_cast<HCURSOR>(m_hIcon);
}

void CLB10Dlg::OnBnClickedButton1()
{
	SYSTEMTIME stBegin, stEnd;
	fileTimeBegin.GetTime( &stBegin );
	fileTimeEnd.GetTime( &stEnd );
	// null unneeded values for better comparison
	stBegin.wHour = stBegin.wMilliseconds = stBegin.wMinute = stBegin.wSecond = 0;
	stEnd.wHour = stEnd.wMilliseconds = stEnd.wMinute = stEnd.wSecond = 0;
	
	COleDateTime begin( stBegin );
	COleDateTime end( stEnd );


	//float tmp=0;
	DAODBEngine* daoDb; 
	CDaoDatabase *myDatabase; 
	AfxDaoInit();  
	daoDb = AfxDaoGetEngine(); 
	ASSERT( daoDb != NULL );
	CString tableHeader="Номер\tФамилия\t\tДата\t\tДеньги\n";

	myDatabase=new CDaoDatabase(); 
	myDatabase->Open("c:\\db1.mdb");

	CDaoRecordset *record; 
	record=new CDaoRecordset(myDatabase);  

	CDaoTableDef *table; 
	table=new CDaoTableDef(myDatabase); 

	table->Open("table1");
	record->Open(table); 
	int recordCount = record->GetRecordCount();

	CDataExchange exchg(this,true);
	double sum = 0;
	for (int i=0; i < recordCount; i++) 
	{
		COleVariant temp;
		record->GetFieldValue("Date",temp); 
		DATE dateField  = temp.date;
		record->GetFieldValue("Sum",temp);
		double sumField = temp.dblVal;
		COleDateTime dateTime( dateField );
		if ( dateTime >= begin && dateTime <= end )
			{
				sum += sumField;
			}
		record->MoveNext();
	}
	char buffer[_CVTBUFSIZE];
	_gcvt( sum, 12, buffer );
	editBalance.SetWindowText( buffer );
}

void CLB10Dlg::OnDtnCloseupDatetimepickerbegin(NMHDR *pNMHDR, LRESULT *pResult)
{
	CTime begin, end;
	fileTimeBegin.GetTime( begin );
	fileTimeEnd.GetTime( end );
	if ( begin > end )
	{
		fileTimeBegin.SetTime( &end );
	}
	*pResult = 0;
}

void CLB10Dlg::OnDtnCloseupDatetimepickerend(NMHDR *pNMHDR, LRESULT *pResult)
{
	CTime begin, end;
	fileTimeBegin.GetTime( begin );
	fileTimeEnd.GetTime( end );
	if ( end < begin )
	{
		fileTimeEnd.SetTime( &begin );
	}
	*pResult = 0;
}
